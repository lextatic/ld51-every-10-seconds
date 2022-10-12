using GameBase;
using System;
using WebSocketSharp;
using MessageEventArgs = WebSocketSharp.MessageEventArgs;

namespace WebSocketTransporter
{
	public class WebSocketTransporterClient : BaseTransporter
	{
		private readonly WebSocket _webSocket;
		private WebSocketPeer _peer;

		public WebSocketTransporterClient(IMessageSerializer serializer) : base(serializer)
		{
			_webSocket = new WebSocket("wss://lextatic.com:7070/minesweeper");

			_webSocket.OnMessage += WebSocket_OnMessage;
			_webSocket.OnOpen += WebSocket_OnOpen;
		}

		public void Connect()
		{
			_webSocket.Connect();
		}

		public void Disconnect()
		{
			_webSocket.Close();
		}

		protected override void OnSend(byte[] serializedMessage)
		{
			_webSocket.Send(serializedMessage);
		}

		private void WebSocket_OnOpen(object sender, EventArgs e)
		{
			_peer = new WebSocketPeer();
			_peer.OnSend += Peer_OnSend;
			OnConnectHandle(_peer);
		}

		private void Peer_OnSend(byte[] obj)
		{
			OnSend(obj);
		}

		private void WebSocket_OnMessage(object sender, MessageEventArgs e)
		{
			Received(_peer, e.RawData);
		}

		public override void Dispose()
		{
			base.Dispose();

			_webSocket.Close();
		}
	}
}
