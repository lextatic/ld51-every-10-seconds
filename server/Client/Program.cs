using Autofac;
using CustomSerializer;
using ENetTransporter;
using GameBase;
using GameEntities;
using GameEntities.Messages;
using System;
using System.Linq;
using System.Threading;

public class Program
{
	static void Main(string[] args)
	{
		ENet.Library.Initialize();

		var builder = new ContainerBuilder();

		builder.RegisterType<CustomJsonSerialize>()
			.As<IMessageSerializer>();

		builder.RegisterType<ENetTransporterClient>()
			.AsSelf()
			.As<BaseTransporter>();

		builder.RegisterType<EventManager>()
			.As<BaseEventManager>();

		builder.RegisterType<GameState>()
			.As<BaseGameState>();

		var container = builder.Build();

		using (var scope = container.BeginLifetimeScope())
		{
			var transporter = scope.Resolve<ENetTransporterClient>();
			var gameState = scope.Resolve<BaseGameState>();
			var eventManager = scope.Resolve<BaseEventManager>();

			TypeManager.TypeManager.RegisterClass<GameUpdateMessage>();
			TypeManager.TypeManager.RegisterClass<GameStateMessage>();
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

			transporter.Start(new ENetTransporterConfigure
			{
				Host = "localhost",
				Port = 7070
			});

			SpinWait.SpinUntil(() => MenuLoop(gameState, transporter));

			transporter.Stop();
			transporter.Dispose();
		}

		Console.WriteLine("Client is closing.");
		ENet.Library.Deinitialize();
	}

	static bool MenuLoop(BaseGameState gameState, BaseTransporter transporter)
	{
		DumpBallsState(gameState);

		var keyInfo = Console.ReadKey(true);

		switch (keyInfo.Key)
		{
			case ConsoleKey.D1:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Mark 0");
				Console.ResetColor();
				transporter.Send(new MarkMessage { GameId = gameState.MyEntities.ElementAt(0), Index = 0 });
				return false;
			case ConsoleKey.D2:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Play 50");
				Console.ResetColor();
				transporter.Send(new PlayMessage { GameId = gameState.MyEntities.ElementAt(0), Index = 50 });
				return false;
			case ConsoleKey.D3:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Mark 99");
				Console.ResetColor();
				transporter.Send(new MarkMessage { GameId = gameState.MyEntities.ElementAt(0), Index = 99 });
				return false;
			case ConsoleKey.C:
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Closing");
				Console.ResetColor();
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

		Console.WriteLine("Pressione:");
		WriteMenuEntry('1', "Selecionar a bola #1");
		WriteMenuEntry('2', "Selecionar a bola #2");
		WriteMenuEntry('3', "Selecionar a bola #3");
		WriteMenuEntry('K', "Attack");
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