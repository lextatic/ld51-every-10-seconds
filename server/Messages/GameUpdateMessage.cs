using GameBase;
using GameEntities.Entities;
using System;

namespace GameEntities.Messages
{
	[Serializable]
	public struct GameUpdateMessage : IBaseMessage
	{
		public long GameId;
		public sbyte[] Values;

		public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
		{
			eventManager.Dispatch("GameUpdateMessage", this);

			var target = gameState.Get<MinesweeperGame>(GameId);
			target.PlayerCells = Values;

			Console.WriteLine(target.ToStringPlayer());
		}
	}
}
