using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct RequestCreateAvatarMessage : IBaseMessage
	{
		public string Name;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			var rand = new Random((int)DateTime.Now.Ticks);

			var avatarID = rand.Next();
			var avatar = new Avatar
			{
				ID = avatarID,
				Name = Name,
				Score = 0
			};

			gameState.Add(avatar);
			gameState.TryAssignOwner(avatarID, sender);

			transporter.Send(new OwnerMessage
			{
				ID = avatarID,
			}, sender);

			//var games = new List<MinesweeperGame>();

			//foreach (var entity in gameState.Entities)
			//{
			//	if (entity.GetType() == typeof(MinesweeperGame))
			//	{
			//		games.Add((MinesweeperGame)entity);
			//	}
			//}

			//transporter.Send(new GameStateMessage
			//{
			//	Games = games
			//}, sender);
		}
	}
}
