using GameBase;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct OwnerMessage : IBaseMessage
	{
		public long ID;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			gameState.TryAssignOwner(ID);
		}
	}
}
