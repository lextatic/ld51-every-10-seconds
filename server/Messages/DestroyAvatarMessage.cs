using GameBase;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct DestroyAvatarMessage : IBaseMessage
	{
		public long ID;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			var avatar = gameState.Get<BaseEntity>(ID);
			gameState.Remove(avatar);
		}
	}
}
