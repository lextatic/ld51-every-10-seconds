using System;
using System.Diagnostics;

namespace GameBase
{
    public class MessageWriter : IMessageWriter
    {
        private byte[] m_Buffer;
        private int m_Index;

        private const int DefaultInitialBufferSize = 256;

        public MessageWriter()
        {
            m_Buffer = Array.Empty<byte>();
            m_Index = 0;
        }

        public ReadOnlyMemory<byte> WrittenMemory => m_Buffer.AsMemory(0, m_Index);
        public ReadOnlySpan<byte> WrittenSpan => m_Buffer.AsSpan(0, m_Index);
        public int WrittenCount => m_Index;
        
        public int Capacity => m_Buffer.Length;
        public int FreeCapacity => m_Buffer.Length - m_Index;

        public void Advance(int count)
        {
            if (count < 0)
                throw new ArgumentException(nameof(count));

            if (m_Index > m_Buffer.Length - count)
                ThrowInvalidOperationException_AdvancedTooFar(m_Buffer.Length);

            m_Index += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            Debug.Assert(m_Buffer.Length > m_Index);
            return m_Buffer.AsMemory(m_Index);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            CheckAndResizeBuffer(sizeHint);
            Debug.Assert(m_Buffer.Length > m_Index);
            return m_Buffer.AsSpan(m_Index);
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
                throw new ArgumentException(nameof(sizeHint));

            if (sizeHint == 0)
            {
                sizeHint = 1;
            }

            if (sizeHint > FreeCapacity)
            {
                int growBy = Math.Max(sizeHint, m_Buffer.Length);

                if (m_Buffer.Length == 0)
                {
                    growBy = Math.Max(growBy, DefaultInitialBufferSize);
                }

                int newSize = checked(m_Buffer.Length + growBy);

                Array.Resize(ref m_Buffer, newSize);
            }

            Debug.Assert(FreeCapacity > 0 && FreeCapacity >= sizeHint);
        }

        private static void ThrowInvalidOperationException_AdvancedTooFar(int capacity)
        {
            throw new InvalidOperationException();
        }
    }
}