using GameBase;
using System;
using System.Net;
using System.Net.Sockets;

namespace SocketTransporter
{
	public class SocketTransporterServer : BaseTransporter
	{
		System.Threading.Thread SocketThread;
		UdpClient udpServer;
		IPEndPoint remoteEP;

		public SocketTransporterServer(IMessageSerializer serializer) : base(serializer)
		{
			StartServer();
		}

		private void StartServer()
		{
			SocketThread = new System.Threading.Thread(networkCode);
			SocketThread.IsBackground = true;
			SocketThread.Start();
		}

		void networkCode()
		{
			udpServer = new UdpClient(11000);
			remoteEP = new IPEndPoint(IPAddress.Any, 11000);

			while (true)
			{
				var data = udpServer.Receive(ref remoteEP); // listen on port 11000
				Console.Write("receive data from " + remoteEP.ToString());

				Received(new SocketPeer(udpServer, remoteEP), data);
			}
		}

		protected override void OnSend(byte[] serializedMessage)
		{
			udpServer.Send(serializedMessage, serializedMessage.Length, remoteEP); // reply back
		}
	}
}
