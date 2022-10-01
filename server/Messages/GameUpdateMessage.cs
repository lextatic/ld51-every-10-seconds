using GameBase;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct GameUpdateMessage : IBaseMessage
	{
		public long GameID;
		public sbyte[] Values;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			eventManager.Dispatch("GameUpdateMessage", this);

			var clientGameState = (GameState)gameState;

			clientGameState.MyGame.ID = GameID;
			clientGameState.MyGame.PlayerCells = Values;

			Console.WriteLine(clientGameState.MyGame.ToStringPlayer());
		}
	}
}
