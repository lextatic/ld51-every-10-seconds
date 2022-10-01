namespace GameBase
{
	public interface IBasePeer
	{
		void Send(byte[] serializedMessage);
	}
}