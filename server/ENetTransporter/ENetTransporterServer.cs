using GameBase;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ENetTransporter
{
	public class ENetTransporterServer : ENetTransporter
	{
		private ENetPeer[] _peers;

		public ENetTransporterServer(IMessageSerializer serializer) : base(serializer)
		{
			Host = new global::ENet.Host();
			_peers = new ENetPeer[ENet.Library.maxPeers];
		}

		public void Start(ENetTransporterConfigure configure)
		{
			Host.Create(new ENet.Address
			{
				Port = configure.Port
			}, configure.PeerLimit, configure.ChannelLimit);

			IsRunning = true;
			Task = Task.Factory.StartNew(NetworkMain, TaskCreationOptions.LongRunning);
		}

		public void Stop()
		{
			Host.Flush();
		}

		protected override void Receive()
		{
			while (IsRunning && CheckEvents(out var @event))
			{
				switch (@event.Type)
				{
					case ENet.EventType.Connect:
						OnPeerConnect(@event);
						break;
					case ENet.EventType.Disconnect:
						OnPeerDisconnect(@event);
						break;
					case ENet.EventType.Timeout:
						OnPeerTimeout(@event);
						break;
					case ENet.EventType.Receive:
						byte[] managedArray = new byte[@event.Packet.Length];
						Marshal.Copy(@event.Packet.Data, managedArray, 0, @event.Packet.Length);
						Received(_peers[@event.Peer.ID], managedArray);
						@event.Packet.Dispose();
						break;
				}
			}
		}

		private void OnPeerConnect(ENet.Event @event)
		{
			var index = @event.Peer.ID;
			ref var peerData = ref _peers[index];

			peerData.OnConnect(@event.Peer);

			OnConnectHandle(_peers[index]);
		}

		private void OnPeerDisconnect(ENet.Event @event)
		{
			var index = @event.Peer.ID;
			OnDisconnectHandle(_peers[index]);

			_peers[index].OnDisconnect();
		}

		private void OnPeerTimeout(ENet.Event @event)
		{
			OnPeerDisconnect(@event);
		}

		protected override void OnSend(byte[] serializedMessage)
		{
			ENet.Packet packet = default;
			packet.Create(serializedMessage);
			Host.Broadcast(0, ref packet);
		}
	}
}
