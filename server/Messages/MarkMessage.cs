using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct MarkMessage : IBaseMessage
	{
		public long GameID;
		public int Index;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			eventManager.Dispatch("MarkMessage", this);

			// Check if peer is still owner of the game

			var targetGame = gameState.Get<MinesweeperGame>(GameID);

			targetGame.Mark(Index);

			transporter.Send(new GameUpdateMessage
			{
				GameID = GameID,
				Values = targetGame.PlayerCells
			}, sender);
		}
	}
}
