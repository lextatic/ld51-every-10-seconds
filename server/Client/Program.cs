using Autofac;
using CustomSerializer;
using GameBase;
using GameEntities;
using GameEntities.Messages;
using System;
using System.Threading;
using WebSocketTransporter;

public class Program
{
	static void Main(string[] args)
	{
		//ENet.Library.Initialize();

		var builder = new ContainerBuilder();

		builder.RegisterType<CustomJsonSerialize>()
			.As<IMessageSerializer>();

		//builder.RegisterType<ENetTransporterClient>()
		//	.AsSelf()
		//	.As<BaseTransporter>();

		builder.RegisterType<WebSocketTransporterClient>()
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
			var transporter = scope.Resolve<WebSocketTransporterClient>();
			var gameState = scope.Resolve<GameState>();
			var eventManager = scope.Resolve<BaseEventManager>();

			TypeManager.TypeManager.RegisterClass<GameUpdateMessage>();
			TypeManager.TypeManager.RegisterClass<AvatarUpdateMessage>();
			TypeManager.TypeManager.RegisterClass<GameChangeMessage>();
			TypeManager.TypeManager.RegisterClass<AvatarStateMessage>();
			TypeManager.TypeManager.RegisterClass<OwnerMessage>();
			TypeManager.TypeManager.RegisterClass<CreateAvatarMessage>();
			TypeManager.TypeManager.RegisterClass<DestroyAvatarMessage>();

			transporter.MessageReceived += (sender, e) =>
			{
				e.Message.Execute(e.Sender, transporter, gameState, eventManager);
			};

			transporter.ConnectHandle += (sender, e) =>
			{
				transporter.Send(new RequestCreateAvatarMessage
				{
					Name = "Lex"
				});
			};

			//transporter.Start(new ENetTransporterConfigure
			//{
			//	Host = "localhost",
			//	Port = 7070
			//});

			transporter.Connect();

			SpinWait.SpinUntil(() => MenuLoop(gameState, transporter));

			//transporter.Stop();
			transporter.Dispose();
		}

		Console.WriteLine("Client is closing.");
		//ENet.Library.Deinitialize();
	}

	static bool MenuLoop(GameState gameState, WebSocketTransporterClient transporter)
	{
		DumpBallsState(gameState);

		var keyInfo = Console.ReadKey(true);

		switch (keyInfo.Key)
		{
			case ConsoleKey.D1:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Mark 0");
				Console.ResetColor();
				transporter.Send(new MarkMessage { GameID = gameState.MyGame.ID, Index = 0 });
				return false;
			case ConsoleKey.D2:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Play 50");
				Console.ResetColor();
				transporter.Send(new PlayMessage { GameID = gameState.MyGame.ID, Index = 50 });
				return false;
			case ConsoleKey.C:
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Closing");
				Console.ResetColor();
				transporter.Disconnect();
				return true;
			default:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Invalid option!\r");
				Console.ResetColor();
				return false;
		}
	}

	static void DumpBallsState(BaseGameState gameState)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"\n{gameState}");
		Console.ResetColor();

		Console.WriteLine("Press:");
		WriteMenuEntry('1', "Mark #0");
		WriteMenuEntry('2', "Play #50");
		WriteMenuEntry('C', "Disconnect");
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