using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct CreateAvatarMessage : IBaseMessage
	{
		public long ID;
		public string Name;
		public uint Score;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			gameState.Add(new Avatar
			{
				Name = Name,
				ID = ID,
				Score = 0
			});
		}
	}
}
