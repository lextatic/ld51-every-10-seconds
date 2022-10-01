namespace GameEntities.Messages
{
	//// TODO: unificar com UpdateOwnerTransformMessage
	//[Serializable]
	//public struct UpdateProxyTransformMessage : IBaseMessage
	//{
	//	public long ID;
	//	public int Health;
	//	public Vector3 Position;
	//	public Vector3 Rotation;

	//	public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
	//	{
	//		var avatar = gameState.Get<Avatar>(ID);

	//		if (!gameState.MyEntities.Contains(avatar.Id))
	//		{
	//			avatar.Health = Health;
	//			avatar.Position = Position;
	//			avatar.Rotation = Rotation;
	//		}
	//	}
	//}
}
