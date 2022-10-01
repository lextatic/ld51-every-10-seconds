using System;

namespace ENet
{
	partial struct Peer : IEquatable<Peer>
	{
		public bool Equals(Peer other)
		{
			return nativePeer == other.nativePeer;
		}
	}
}
