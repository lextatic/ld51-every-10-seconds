//using ENetTransporter;

public static class ConfigReader
{
	//public static ENetTransporterConfigure GetENetTransporterConfig()
	//{
	//	var fileName = "config.json";
	//	if (File.Exists(fileName))
	//	{
	//		var deserializedConfigurations = File.ReadAllText(fileName);
	//		var serializableTransportConfig = JsonUtility.FromJson<SerializableTransportConfig>(deserializedConfigurations);
	//		if (string.IsNullOrEmpty(serializableTransportConfig.Host)) //failsafe
	//		{
	//			serializableTransportConfig.Host = "localhost";
	//		}
	//		return serializableTransportConfig.ToModel();
	//	}

	//	return new ENetTransporterConfigure
	//	{
	//		Port = 7070,
	//		Host = "localhost",
	//	};
	//}
}