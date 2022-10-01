using GameBase;
using GameEntities;
using System.Net;
using System.Net.Sockets;

namespace SocketTransporter
{
	public class SocketPeer : IBasePeer
	{
		public UdpClient udpServer;
		public IPEndPoint remoteEP;

		public SocketPeer(UdpClient udpServer, IPEndPoint remoteEP)
		{
			this.udpServer = udpServer;
			this.remoteEP = remoteEP;
		}

		public void Send(byte[] serializedMessage)
		{
			udpServer.Send(serializedMessage, serializedMessage.Length, remoteEP);
		}
	}
}
