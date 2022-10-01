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

			//TypeManager.TypeManager.RegisterClass<RequestCreateAvatarMessage>();
			//TypeManager.TypeManager.RegisterClass<RequestBallColorMessage>();
			TypeManager.TypeManager.RegisterClass<RequestGameStateMessage>();
			TypeManager.TypeManager.RegisterClass<MarkMessage>();
			TypeManager.TypeManager.RegisterClass<PlayMessage>();
			//TypeManager.TypeManager.RegisterClass<UpdateOwnerTransformMessage>();
			//TypeManager.TypeManager.RegisterClass<BeginAttackMessage>();
			//TypeManager.TypeManager.RegisterClass<AttackMessage>();

			// TODO: Improve generate entity ID
			var rand = new Random(DateTime.Now.Millisecond);

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
				e.Message.Execute(e.Sender, transporter, gameState, eventManager);
			};

			transporter.ConnectHandle += (sender, peer) =>
			{
				var newID = rand.Next();

				var minesweeperGame = new MinesweeperGame(10, 10)
				{
					Id = newID
				};

				gameState.Add(minesweeperGame);

				gameState.TryAssignOwner(newID, peer);

				transporter.Send(new OwnerMessage
				{
					ID = newID
				}, peer);
			};

			transporter.DisconnectHandle += (sender, peer) =>
			{
				if (gameState.EntityOwnerMap.TryRemove(peer, out var gameID))
				{
					var avatar = gameState.Get<BaseEntity>(gameID);
					gameState.Remove(avatar);
				}
			};

			gameState.OnAdd += (sender, entity) =>
			{
				//if (entity.GetType() == typeof(Avatar))
				//{
				//	var avatar = (Avatar)entity;
				//	transporter.Send(new RequestCreateAvatarMessage
				//	{
				//		ID = avatar.Id,
				//		Health = avatar.Health,
				//		Position = avatar.Position
				//	});
				//}
			};

			gameState.OnRemove += (sender, entity) =>
			{
				//transporter.Send(new RequestDestroyAvatarMessage
				//{
				//	ID = entity.Id,
				//});
			};

			transporter.Start(new ENetTransporterConfigure
			{
				Port = 7070
			});

			SpinWait.SpinUntil(() => MenuLoop(gameState, statistics));

			transporter.Stop();
			transporter.Dispose();
		}
		Console.WriteLine("Stopping...");

		ENet.Library.Deinitialize();
	}

	static bool MenuLoop(BaseGameState gameState, ServerStatistics statistics)
	{
		DumpBallsState(statistics, gameState);

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

	static void DumpBallsState(ServerStatistics statistics, BaseGameState gameState)
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