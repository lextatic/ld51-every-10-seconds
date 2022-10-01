using GameBase;
using System;

namespace GameEntities.Entities
{
	[Serializable]
	public class Avatar : BaseEntity
	{
		public string Name { get; set; }
		public uint Score { get; set; }
	}
}
