namespace ENetTransporter
{
	public class ENetTransporterConfigure
	{
		public ushort Port { get; set; }
		public int ChannelLimit { get; set; } = (int)ENet.Library.maxChannelCount;
		public int PeerLimit { get; set; } = (int)ENet.Library.maxPeers;
		public string Host { get; set; }
	}
}

