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

		private IBasePeer _sender;
		private GameState _serverGameState;
		private BaseTransporter _transporter;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			_serverGameState = (GameState)gameState;
			_transporter = transporter;
			_sender = sender;

			if (!_serverGameState.OwnerToGameMap.ContainsKey(sender))
			{
				Console.WriteLine("Peer doesn't have a game yet.");
				return;
			}

			if (_serverGameState.OwnerToGameMap[sender] != GameID)
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

			targetGame.OnVictory = GameOverHandler;
			targetGame.OnGameOver = GameOverHandler;
			targetGame.Play(Index);
			// TODO: Oh boy... I prefer to use events, but it was bugged.
			targetGame.OnVictory = null;
			targetGame.OnGameOver = null;

			transporter.Send(new GameUpdateMessage
			{
				GameID = GameID,
				Values = targetGame.PlayerCells
			}, sender);

			targetGame = null;
			_serverGameState = null;
			_sender = null;
			_transporter = null;
			GC.Collect();
		}

		private void GameOverHandler()
		{
			if (_serverGameState.OwnerToGameMap.TryRemove(_sender, out var gameID))
			{
				var game = _serverGameState.Get<BaseEntity>(gameID);
				_serverGameState.Remove(game);
			}
		}
	}
}
