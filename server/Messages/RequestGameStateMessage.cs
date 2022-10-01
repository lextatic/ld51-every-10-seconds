using GameBase;
using GameEntities.Entities;
using System;
using System.Collections.Generic;

namespace GameEntities.Messages
{
	[Serializable]
	public struct RequestGameStateMessage : IBaseMessage
	{
		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			var games = new List<MinesweeperGame>();

			foreach (var entity in gameState.Entities)
			{
				if (entity.GetType() == typeof(MinesweeperGame))
				{
					games.Add((MinesweeperGame)entity);
				}
			}

			transporter.Send(new GameStateMessage
			{
				Games = games
			}, sender);
		}
	}
}
