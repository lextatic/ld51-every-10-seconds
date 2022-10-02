using GameBase;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct GameUpdateMessage : IBaseMessage
	{
		public long GameID;
		public sbyte[] Values;
		public bool IsGameOver;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			var clientGameState = (GameState)gameState;

			// Só aceita se for ID igual
			if (clientGameState.MyGame.ID != GameID)
			{
				Console.WriteLine("Aborting GameUpdate, different ID.");
				return;
			}

			eventManager.Dispatch("GameUpdateMessage", this);

			//clientGameState.MyGame.ID = GameID;
			clientGameState.MyGame.PlayerCells = Values;

			Console.WriteLine(clientGameState.MyGame.ToStringPlayer());
		}
	}
}
