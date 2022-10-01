using GameBase;
using System;
using System.Threading.Tasks;

namespace ENetTransporter
{
	public abstract class ENetTransporter : BaseTransporter
	{
		protected ENet.Host Host;
		protected Task Task;

		protected bool IsRunning;

		public ENetTransporter(IMessageSerializer serializer) : base(serializer)
		{
		}

		protected void NetworkMain()
		{
			while (IsRunning)
			{
				Receive();
			}

			Host.Flush();
		}

		protected bool CheckEvents(out ENet.Event @event)
		{
			return Host.CheckEvents(out @event) > 0 || Host.Service(15, out @event) > 0;
		}

		protected abstract void Receive();

		public override void Dispose()
		{
			base.Dispose();

			IsRunning = false;
			Task.Wait();
			Task = null;
		}
	}
}
