namespace GameEntities.Messages
{
	//[Serializable]
	//public struct RequestBallColorMessage : IBaseMessage
	//{
	//	public long EntityID;

	//	private static readonly Random _random = new Random(DateTime.Now.Millisecond);

	//	public void Execute(IBasePeer sender, BaseTransporter transport, BaseGameState gameState, BaseEventManager eventManager)
	//	{
	//		var color = (byte)_random.Next(0, 16);

	//		var ball = gameState.Get<Ball>(EntityID);
	//		if(ball != null)
	//		{
	//			ball.Color = color;

	//			transport.Send(new ChangeBallColorMessage { EntityID = EntityID, Color = color });
	//		}
	//	}
	//}
}
