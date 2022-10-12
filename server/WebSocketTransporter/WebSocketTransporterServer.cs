using GameBase;
using System;
using System.Collections.Generic;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebSocketTransporter
{
	public class Minesweeper : WebSocketBehavior
	{
		public event Action<WebSocketPeer, byte[]> OnMessageReceived;
		public event Action<WebSocketPeer, Minesweeper> OnClosed;
		public event Action<WebSocketPeer> OnOpened;

		private WebSocketPeer _myPeer;

		public Minesweeper(WebSocketPeer myPeer)
		{
			_myPeer = myPeer;
		}

		protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
		{
			base.OnMessage(e);
			OnMessageReceived?.Invoke(_myPeer, e.RawData);
		}

		protected override void OnOpen()
		{
			base.OnOpen();

			_myPeer.OnSend += MyPeer_OnSend;

			OnOpened?.Invoke(_myPeer);
		}

		protected override void OnError(ErrorEventArgs e)
		{
			base.OnError(e);

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"ERROR: {e.Message}");
			Console.ResetColor();
		}

		private void MyPeer_OnSend(byte[] data)
		{
			Send(data);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			base.OnClose(e);
			_myPeer.OnSend -= MyPeer_OnSend;
			OnClosed?.Invoke(_myPeer, this);
		}

		public void BroadcastSend(byte[] serializedMessage)
		{
			Sessions.Broadcast(serializedMessage);
		}
	}

	public class WebSocketTransporterServer : BaseTransporter
	{
		readonly WebSocketServer _webSocketServer;

		private readonly List<Minesweeper> _peers = new List<Minesweeper>();

		public WebSocketTransporterServer(IMessageSerializer serializer) : base(serializer)
		{
			_webSocketServer = new WebSocketServer("ws://localhost:7070");

			_webSocketServer.WaitTime = TimeSpan.FromSeconds(30);
			_webSocketServer.KeepClean = true;
			_webSocketServer.Log.Disable();

			_webSocketServer.AddWebSocketService<Minesweeper>("/minesweeper", () =>
			{
				var newPeer = new WebSocketPeer();

				var minesweeperServiceInstance = new Minesweeper(newPeer);

				minesweeperServiceInstance.OnMessageReceived += Peer_OnMessageReceived;
				minesweeperServiceInstance.OnClosed += Peer_OnClosed;
				minesweeperServiceInstance.OnOpened += Peer_OnOpened;

				_peers.Add(minesweeperServiceInstance);

				return minesweeperServiceInstance;
			});

			_webSocketServer.Start();
		}

		private void Peer_OnOpened(WebSocketPeer newPeer)
		{
			OnConnectHandle(newPeer);
		}

		private void Peer_OnClosed(WebSocketPeer peer, Minesweeper minesweeperServiceInstance)
		{
			minesweeperServiceInstance.OnMessageReceived -= Peer_OnMessageReceived;
			minesweeperServiceInstance.OnClosed -= Peer_OnClosed;

			OnDisconnectHandle(peer);

			_peers.Remove(minesweeperServiceInstance);
		}

		private void Peer_OnMessageReceived(WebSocketPeer peer, byte[] data)
		{
			Received(peer, data);
		}

		protected override void OnSend(byte[] serializedMessage)
		{
			if (_webSocketServer.WebSocketServices.TryGetServiceHost("/minesweeper", out var host))
			{
				host.Sessions.Broadcast(serializedMessage);
			}
		}
	}

	public static class LoggerExtensions
	{
		public static void Disable(this Logger logger)
		{
			var field = logger.GetType().GetField("_output", BindingFlags.NonPublic | BindingFlags.Instance);
			field?.SetValue(logger, new Action<LogData, string>((d, s) => { }));
		}
	}
}
