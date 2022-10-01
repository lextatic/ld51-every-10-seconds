namespace GameEntities.Messages
{
	//// TODO: unificar com UpdateProxyTransformMessage
	//[Serializable]
	//public struct UpdateOwnerTransformMessage : IBaseMessage
	//{
	//	public long ID;
	//	public Vector3 Position;
	//	public Vector3 Rotation;

	//	public void Execute(IBasePeer sender, BaseTransporter transporter, BaseGameState gameState, BaseEventManager eventManager)
	//	{
	//		var avatar = gameState.Get<Avatar>(ID);
	//		if (avatar != null)
	//		{
	//			avatar.Position = Position;
	//			avatar.Rotation = Rotation;
	//		}
	//		transporter.Send(new UpdateProxyTransformMessage
	//		{
	//			ID = ID,
	//			Health = avatar.Health,
	//			Position = Position,
	//			Rotation = Rotation
	//		});
	//	}
	//}
}
