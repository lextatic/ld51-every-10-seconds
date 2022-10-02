using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct AvatarUpdateMessage : IBaseMessage
	{
		public long AvatarID;
		public uint Score;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			eventManager.Dispatch("AvatarUpdateMessage", this);

			var avatar = gameState.Get<Avatar>(AvatarID);

			avatar.Score = Score;

			Console.WriteLine($"{avatar.Name}'s new score is {avatar.Score}!");
		}
	}
}
