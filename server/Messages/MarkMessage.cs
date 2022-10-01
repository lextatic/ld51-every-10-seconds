﻿using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct MarkMessage : IBaseMessage
	{
		public long GameId;
		public int Index;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			eventManager.Dispatch("MarkMessage", this);
			var targetGame = gameState.Get<MinesweeperGame>(GameId);

			targetGame.Mark(Index);

			transporter.Send(new GameUpdateMessage
			{
				GameId = GameId,
				Values = targetGame.PlayerCells
			});
		}
	}
}