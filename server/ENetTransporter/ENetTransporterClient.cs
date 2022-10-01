using GameBase;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ENetTransporter
{
	public class ENetTransporterClient : ENetTransporter
	{
		private ENetPeer _peerData;

		public ENetTransporterClient(IMessageSerializer serializer) : base(serializer)
		{

		}

		public void Start(ENetTransporterConfigure configure)
		{
			var address = new ENet.Address
			{
				Port = configure.Port,
			};
			address.SetHost(configure.Host);

			Host = new ENet.Host();
			Host.Create();
			Host.Connect(address, configure.ChannelLimit);

			IsRunning = true;
			Task = Task.Factory.StartNew(NetworkMain, TaskCreationOptions.LongRunning);
		}

		public void Stop()
		{
			_peerData.Peer.Disconnect(0);
			Host.Flush();
		}

		protected override void Receive()
		{
			while (IsRunning && CheckEvents(out var @event))
			{
				switch (@event.Type)
				{
					case ENet.EventType.Connect:
						_peerData = new ENetPeer();
						_peerData.OnConnect(@event.Peer);
						OnConnectHandle(_peerData);
						break;
					case ENet.EventType.Disconnect:
					case ENet.EventType.Timeout:
						OnDisconnectHandle(_peerData);
						break;
					case ENet.EventType.Receive:
						byte[] managedArray = new byte[@event.Packet.Length];
						Marshal.Copy(@event.Packet.Data, managedArray, 0, @event.Packet.Length);
						Received(_peerData, managedArray);
						@event.Packet.Dispose();
						break;
				}
			}
		}

		protected override void OnSend(byte[] serializedMessage)
		{
			ENet.Packet packet = default;
			packet.Create(serializedMessage);
			_peerData.Peer.Send(0, ref packet);
		}
	}
}
