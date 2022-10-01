using System;

namespace GameBase
{
	/// <summary>
	/// Classe de transporte (aka "rede").
	/// </summary>
	public abstract class BaseTransporter : IDisposable
	{
		protected readonly IMessageSerializer _serializer;

		public event EventHandler<IBasePeer> ConnectHandle;
		public event EventHandler<IBasePeer> DisconnectHandle;

		public BaseTransporter(IMessageSerializer serializer)
		{
			_serializer = serializer;
		}

		/// <summary>
		/// Uma mensagem foi recebida
		/// </summary>
		public event EventHandler<MessageEventArgs> MessageReceived;


		/// <summary>
		/// Envia uma mensagem para a outra ponta (estamos desconsiderando broadcast, rede, etc.)
		/// </summary>
		/// <param name="message">Mensagem a enviar</param>
		public void Send(IBaseMessage message, IBasePeer targetPeer = null)
		{
			var writer = new MessageWriter();

			_serializer.Serialize(message, ref writer);

			var data = writer.WrittenSpan.ToArray();

			if(targetPeer == null)
			{
				OnSend(data);
			}
			else
			{
				targetPeer.Send(data);
			}
		}

		/// <summary>
		/// Transportar esta mensagem.
		/// </summary>
		/// <param name="serializedMessage">Mensagem já serializada.</param>
		protected abstract void OnSend(byte[] serializedMessage);

		/// <summary>
		/// Uma mensagem foi recebida.
		/// </summary>
		/// <param name="serializedMessage"></param>
		protected void Received(IBasePeer sender, byte[] serializedMessage)
		{
			var deserializedMessage = _serializer.Deserialize<IBaseMessage>(new ReadOnlySpan<byte>(serializedMessage));

			MessageReceived.Invoke(this, new MessageEventArgs(sender, deserializedMessage));
		}

		/// <summary>
		/// Limpa o transporte.
		/// </summary>
		public virtual void Dispose()
		{
			if (MessageReceived != null)
			{
				foreach (var invocation in MessageReceived.GetInvocationList())
				{
					MessageReceived -= invocation as EventHandler<MessageEventArgs>;
				}
			}

			GC.SuppressFinalize(this);
		}

		protected virtual void OnConnectHandle(IBasePeer peer)
		{
			ConnectHandle?.Invoke(this, peer);
		}

		protected virtual void OnDisconnectHandle(IBasePeer peer)
		{
			DisconnectHandle?.Invoke(this, peer);
		}
	}
}