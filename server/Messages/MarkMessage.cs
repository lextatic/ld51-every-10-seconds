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
			var serverGameState = (GameState)gameState;

			if (!serverGameState.OwnerToGameMap.ContainsKey(sender))
			{
				Console.WriteLine("Peer doesn't have a game yet.");
				return;
			}

			if (serverGameState.OwnerToGameMap[sender] != GameID)
			{
				Console.WriteLine("Invalid gameID, message is probably late.");
				return;
			}

			var targetGame = gameState.Get<MinesweeperGame>(GameID);

			if (targetGame == null)
			{
				Console.WriteLine("Game doesn't exist.");
				return;
			}

			eventManager.Dispatch("MarkMessage", this);

			targetGame.Mark(Index);

			transporter.Send(new GameUpdateMessage
			{
				GameID = GameID,
				Values = targetGame.PlayerCells,
				IsGameOver = false
			}, sender);
		}
	}
}
