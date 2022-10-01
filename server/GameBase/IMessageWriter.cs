using System;
using System.Buffers;

namespace GameBase
{
	public interface IMessageWriter : IBufferWriter<byte>
	{
		int WrittenCount { get; }
		ReadOnlyMemory<byte> WrittenMemory { get; }
		ReadOnlySpan<byte> WrittenSpan { get; }
	}
}