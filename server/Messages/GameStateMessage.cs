using GameBase;
using GameEntities.Entities;
using System;
using System.Collections.Generic;

namespace GameEntities.Messages
{
	[Serializable]
	public struct GameStateMessage : IBaseMessage
	{
		public List<MinesweeperGame> Games;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			foreach (var game in Games)
			{
				gameState.Add(game);
			}
		}
	}
}
