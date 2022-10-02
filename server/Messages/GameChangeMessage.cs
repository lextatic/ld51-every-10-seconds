using GameBase;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct GameChangeMessage : IBaseMessage
	{
		public long GameID;
		public sbyte[] Values;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			var clientGameState = (GameState)gameState;

			// Só aceita se for ID diferente
			if (clientGameState.MyGame.ID == GameID)
			{
				Console.WriteLine("Aborting GameChange, already same ID.");
				return;
			}

			eventManager.Dispatch("GameChangeMessage", this);

			clientGameState.MyGame.ID = GameID;
			clientGameState.MyGame.PlayerCells = Values;

			Console.WriteLine(clientGameState.MyGame.ToStringPlayer());
		}
	}
}
