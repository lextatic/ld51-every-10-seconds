using System;

namespace GameBase
{
	public interface IMessageSerializer
	{
		void Serialize<TMessage, TWriter>(TMessage message, ref TWriter writer)
			where TMessage : IBaseMessage
			where TWriter : IMessageWriter;

		TMessage Deserialize<TMessage>(ReadOnlySpan<byte> span) where TMessage : IBaseMessage;
	}
}