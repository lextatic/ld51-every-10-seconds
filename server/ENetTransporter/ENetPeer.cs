using ENet;
using GameBase;

namespace ENetTransporter
{
	public struct ENetPeer : IBasePeer
	{
		public int Version;
		public ENet.Peer Peer;

		public void OnConnect(Peer peer)
		{
			Peer = peer;

			if (Version < 0)
			{
				Version = ~Version + 1;
			}
			else
			{
				Version = 1;
			}
		}

		public void OnDisconnect()
		{
			Version = ~Version;
		}

		public void Send(byte[] serializedMessage)
		{
			ENet.Packet packet = default;
			packet.Create(serializedMessage);
			Peer.Send(0, ref packet);
		}

		public override string ToString()
		{
			return Peer.ID.ToString();
		}
	}
}
