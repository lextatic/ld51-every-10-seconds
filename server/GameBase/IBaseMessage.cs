using System;

namespace GameBase
{
	public interface IBaseMessage
	{
		void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager);
	}
}