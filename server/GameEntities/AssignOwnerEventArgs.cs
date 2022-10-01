using GameBase;
using System;

namespace GameEntities
{
	public class AssignOwnerEventArgs : EventArgs
	{
		public IBasePeer Sender { get; }

		public long EntityID{ get; }

		public AssignOwnerEventArgs(IBasePeer sender, long entityID)
		{
			Sender = sender;
			EntityID = entityID;
		}
	}
}
