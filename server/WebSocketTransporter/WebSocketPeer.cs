using GameBase;
using System;

namespace WebSocketTransporter
{
	public class WebSocketPeer : IBasePeer
	{
		public event Action<byte[]> OnSend;

		public void Send(byte[] serializedMessage)
		{
			OnSend?.Invoke(serializedMessage);
		}
	}
}
