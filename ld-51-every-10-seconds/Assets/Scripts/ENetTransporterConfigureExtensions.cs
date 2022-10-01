using ENetTransporter;
using System;

public static class ENetTransporterConfigureExtensions
{
	public static ENetTransporterConfigure ToModel(this SerializableTransportConfig serializable)
	{
		return new ENetTransporterConfigure()
		{
			Host = serializable.Host,
			Port = (ushort)serializable.Port
		};
	}

	public static SerializableTransportConfig ToSerializable(this ENetTransporterConfigure model)
	{
		return new SerializableTransportConfig()
		{
			Host = model.Host,
			Port = model.Port
		};
	}
}

[Serializable]
public struct SerializableTransportConfig
{
	public int Port;
	public string Host;
}