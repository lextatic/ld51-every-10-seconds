using Autofac;
using CustomSerializer;
using ENetTransporter;
using GameBase;
using GameEntities;
using GameEntities.Entities;
using GameEntities.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
	public class ServerStatistics
	{
		public int ReceiveCount = 0;
		public Dictionary<Type, int> CountMessageByType = new Dictionary<Type, int>();

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine($"STATISTICS\n");
			builder.AppendLine($"ReceiveCount: {ReceiveCount}\n");
			builder.AppendLine($"ReceiveMessageType");

			foreach (var kv in CountMessageByType)
			{
				builder.AppendLine($" - {kv.Key} = {kv.Value}");
			}

			return builder.ToString();
		}
	}

	// TODO: Is there a way to make it thread safe without using a lock?
	private static readonly object _gameLock = new object();

	static void Main(string[] args)
	{
		var statistics = new ServerStatistics();

		ENet.Library.Initialize();

		var builder = new ContainerBuilder();

		builder.RegisterType<CustomJsonSerialize>()
			.As<IMessageSerializer>();

		builder.RegisterType<ENetTransporterServer>()
			.AsSelf()
			.As<BaseTransporter>();

		builder.RegisterType<EventManager>()
			.As<BaseEventManager>();

		builder.RegisterType<GameState>()
			.AsSelf()
			.As<BaseGameState>();

		var container = builder.Build();

		using (var scope = container.BeginLifetimeScope())
		{
			var eventManager = scope.Resolve<BaseEventManager>();
			var transporter = scope.Resolve<ENetTransporterServer>();
			var gameState = scope.Resolve<GameState>();

			TypeManager.TypeManager.RegisterClass<RequestCreateAvatarMessage>();
			TypeManager.TypeManager.RegisterClass<MarkMessage>();
			TypeManager.TypeManager.RegisterClass<PlayMessage>();

			// TODO: Improve generate entity ID
			var rand = new Random((int)DateTime.Now.Ticks);

			transporter.MessageReceived += (sender, e) =>
			{
				statistics.ReceiveCount++;
				var messageType = e.Message.GetType();
				if (statistics.CountMessageByType.ContainsKey(messageType))
				{
					statistics.CountMessageByType[messageType] = statistics.CountMessageByType[messageType] + 1;
				}
				else
				{
					statistics.CountMessageByType.TryAdd(messageType, 1);
				}

				lock (_gameLock)
				{
					e.Message.Execute(e.Sender, transporter, gameState, eventManager);
				}
			};

			transporter.ConnectHandle += (sender, peer) =>
			{
				var avatars = new List<Avatar>();

				foreach (var entity in gameState.Entities)
				{
					if (entity is Avatar avatar)
					{
						avatars.Add((Avatar)entity);
					}
				}

				transporter.Send(new AvatarStateMessage
				{
					Avatars = avatars
				}, peer);
			};

			transporter.DisconnectHandle += (sender, peer) =>
			{
				lock (_gameLock)
				{
					if (gameState.EntityOwnerMap.TryRemove(peer, out var avatarID))
					{
						var avatar = gameState.Get<BaseEntity>(avatarID);
						gameState.Remove(avatar);
					}

					if (gameState.OwnerToGameMap.TryRemove(peer, out var gameID))
					{
						var game = gameState.Get<BaseEntity>(gameID);

						// I'm not sure about this check
						//if (game != null)
						{
							gameState.Remove(game);
						}
						//else
						//{
						//Console.WriteLine("Happened again.");
						//}
					}
				}
			};

			gameState.OnAdd += (sender, entity) =>
			{
				if (entity is Avatar avatar)
				{
					transporter.Send(new CreateAvatarMessage
					{
						ID = avatar.ID,
						Name = avatar.Name,
						Score = avatar.Score
					});
				}
			};

			gameState.OnRemove += (sender, entity) =>
			{
				if (entity is Avatar)
				{
					transporter.Send(new DestroyAvatarMessage
					{
						ID = entity.ID,
					});
				}
			};

			transporter.Start(new ENetTransporterConfigure
			{
				Port = 7070
			});


			MainAsync(gameState, statistics, transporter).GetAwaiter().GetResult();

			// not being called
			SpinWait.SpinUntil(() => MenuLoop(gameState, statistics));

			transporter.Stop();
			transporter.Dispose();
		}
		Console.WriteLine("Stopping...");

		ENet.Library.Deinitialize();
	}

	private static async Task MainAsync(GameState gameState, ServerStatistics statistics, ENetTransporterServer transporter)
	{
		var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
		while (await timer.WaitForNextTickAsync())
		{
			Console.WriteLine("10 seconds...");

			CreateNewGames(gameState, transporter);

			DumpGameState(statistics, gameState);
		}
	}

	private static void CreateNewGames(GameState gameState, ENetTransporterServer transporter)
	{
		var rand = new Random((int)DateTime.Now.Ticks);

		IBasePeer? firstPeer = null;
		IBasePeer? previousPeer;
		long firstGameID = 0;
		long previousGameID = 0;

		lock (_gameLock)
		{
			foreach (var peer in gameState.EntityOwnerMap.Keys)
			{
				if (gameState.OwnerToGameMap.TryGetValue(peer, out var currentGameID))
				{
					//var currentGameID = gameState.OwnerToGameMap[peer];

					if (firstPeer == null)
					{
						firstPeer = peer;
						firstGameID = currentGameID;

						Console.WriteLine($"firstPeer was: {currentGameID} - peer: {peer}");
					}
					else// if (gameState.Get<MinesweeperGame>(previousGameID) != null) // Why check this if? I don't know yet.
					{
						if (gameState.OwnerToGameMap.TryUpdate(peer, previousGameID, currentGameID))
						{
							Console.WriteLine($"currentGameID: {currentGameID} - previousGameID: {previousGameID} - peer: {peer}");

							transporter.Send(new GameChangeMessage
							{
								GameID = previousGameID,
								Values = gameState.Get<MinesweeperGame>(previousGameID).PlayerCells
							}, peer);
						}
						else
						{
							Console.WriteLine($"Game could not be updated.");
						}
					}

					previousPeer = peer;
					previousGameID = currentGameID;
				}
				else
				{
					var newID = rand.Next();
					Console.WriteLine($"NewId: {newID} - peer: {peer}");

					var newGame = new MinesweeperGame(10, 10)
					{
						ID = newID
					};

					if (gameState.OwnerToGameMap.TryAdd(peer, newGame.ID))
					{
						gameState.Add(newGame);

						transporter.Send(new GameChangeMessage
						{
							GameID = newID,
							Values = newGame.PlayerCells
						}, peer);
					}
					else
					{
						Console.WriteLine($"Game could not be added. Ignoring.");
					}
				}
			}

			if (firstPeer != null)
			{
				if (gameState.OwnerToGameMap.TryUpdate(firstPeer, previousGameID, firstGameID))
				{
					Console.WriteLine($"applyFirst: {previousGameID} - peer: {firstPeer}");

					transporter.Send(new GameChangeMessage
					{
						GameID = previousGameID,
						Values = gameState.Get<MinesweeperGame>(previousGameID).PlayerCells
					}, firstPeer);
				}
				else
				{
					Console.WriteLine($"Game could not be updated.");
				}
			}
		}
	}

	static bool MenuLoop(BaseGameState gameState, ServerStatistics statistics)
	{
		DumpGameState(statistics, gameState);

		var keyInfo = Console.ReadKey(true);

		switch (keyInfo.Key)
		{
			case ConsoleKey.C:
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Closing...");
				Console.ResetColor();
				return true;
			default:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Invalid command!\r");
				Console.ResetColor();
				return false;
		}
	}

	static void DumpGameState(ServerStatistics statistics, BaseGameState gameState)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"\n{statistics}");
		Console.WriteLine($"\n{gameState}");
		Console.ResetColor();

		Console.WriteLine("Pressione:");
		WriteMenuEntry('1', "Update");
		WriteMenuEntry('C', "Sair");
		Console.ResetColor();
		Console.WriteLine();
	}

	static void WriteMenuEntry(char key, string text)
	{
		Console.ForegroundColor = ConsoleColor.Black;
		Console.BackgroundColor = ConsoleColor.White;
		Console.Write($"[{key}]");
		Console.ResetColor();
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($" - {text}");
	}
}