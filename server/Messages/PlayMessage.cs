using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct PlayMessage : IBaseMessage
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

			eventManager.Dispatch("PlayMessage", this);

			var result = targetGame.Play(Index);

			if (result > 0) // victory
			{
				if (serverGameState.OwnerToGameMap.TryRemove(sender, out var gameID))
				{
					var game = serverGameState.Get<BaseEntity>(gameID);
					serverGameState.Remove(game);
				}

				var avatar = serverGameState.Get<Avatar>(serverGameState.EntityOwnerMap[sender]);
				avatar.Score += 10;

				transporter.Send(new AvatarUpdateMessage
				{
					AvatarID = avatar.ID,
					Score = avatar.Score
				});
			}
			else if (result < 0) // defeat
			{
				if (serverGameState.OwnerToGameMap.TryRemove(sender, out var gameID))
				{
					var game = serverGameState.Get<BaseEntity>(gameID);
					serverGameState.Remove(game);
				}

				var avatar = serverGameState.Get<Avatar>(serverGameState.EntityOwnerMap[sender]);

				// Would be easier to use checked/unchecked, but I don't remember how to use them right now and the
				// deadline is upon us.
				if (avatar.Score < 10)
				{
					avatar.Score = 0;
				}
				else
				{
					avatar.Score -= 10;
				}

				transporter.Send(new AvatarUpdateMessage
				{
					AvatarID = avatar.ID,
					Score = avatar.Score
				});
			}

			transporter.Send(new GameUpdateMessage
			{
				GameID = GameID,
				Values = targetGame.PlayerCells,
				IsGameOver = result != 0
			}, sender);
		}
	}
}
