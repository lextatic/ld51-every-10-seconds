using GameBase;
using HybridWebSocket;
using WebSocketTransporter;
using WebSocket = HybridWebSocket.WebSocket;

public class WebGlTransporterClient : BaseTransporter
{
	private readonly WebSocket _webSocket;
	private WebSocketPeer _peer;

	public WebGlTransporterClient(IMessageSerializer serializer) : base(serializer)
	{
		// Create WebSocket instance
		_webSocket = WebSocketFactory.CreateInstance("ws://127.0.0.1:7070/minesweeper");

		// Add OnOpen event listener
		_webSocket.OnOpen += () =>
		{
			_peer = new WebSocketPeer();
			_peer.OnSend += Peer_OnSend;
			OnConnectHandle(_peer);
		};

		// Add OnMessage event listener
		_webSocket.OnMessage += (byte[] msg) =>
		{
			Received(_peer, msg);
		};
	}

	private void Peer_OnSend(byte[] obj)
	{
		OnSend(obj);
	}

	protected override void OnSend(byte[] serializedMessage)
	{
		_webSocket.Send(serializedMessage);
	}

	public void Connect()
	{
		// Connect to the server
		_webSocket.Connect();
	}

	public override void Dispose()
	{
		base.Dispose();

		_webSocket.Close();
	}
}
