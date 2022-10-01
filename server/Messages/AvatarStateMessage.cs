using GameBase;
using GameEntities.Entities;
using System;
using System.Collections.Generic;

namespace GameEntities.Messages
{
	[Serializable]
	public struct AvatarStateMessage : IBaseMessage
	{
		public List<Avatar> Avatars;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			foreach (var avatar in Avatars)
			{
				gameState.Add(avatar);
			}
		}
	}
}
