using System;

namespace GameBase
{
	/// <summary>
	/// Mensagem recebida no transporte.
	/// </summary>
	public sealed class MessageEventArgs : EventArgs
	{
		public IBasePeer Sender { get; }

		/// <summary>
		/// Mensagem recebida.
		/// </summary>
		public IBaseMessage Message { get; }

		public MessageEventArgs(IBasePeer sender, IBaseMessage message)
		{
			Sender = sender;
			Message = message;
		}
	}
}
