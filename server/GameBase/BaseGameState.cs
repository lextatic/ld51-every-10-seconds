using System.Collections;
using System.Collections.Generic;

namespace GameBase
{
	public abstract class BaseGameState
	{
		public abstract ICollection<BaseEntity> Entities { get; }
		public abstract ICollection<long> MyEntities { get; }

		public abstract void Add(BaseEntity entity);
		public abstract void Remove(BaseEntity entity);
		public abstract T Get<T>(long entityID) where T : BaseEntity;
		public abstract bool TryAssignOwner(long entityID, IBasePeer owner = null);
	}
}
