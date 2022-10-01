using GameBase;
using GameEntities;
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.IO;
using System.Text;
using TypeManager;

namespace CustomSerializer
{
	/// <summary>
	/// Serializador custom que injeta o nome do tipo sendo manipulado
	/// </summary>
	public class CustomJsonSerialize : IMessageSerializer
	{
		public void Serialize<TMessage, TWriter>(TMessage message, ref TWriter writer)
			where TMessage : IBaseMessage
			where TWriter : IMessageWriter
		{
			var strMessage = $"{message.GetType().FullName}\r\n{JsonConvert.SerializeObject(message, Formatting.Indented)}";

			writer.Write(new System.ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(strMessage)));
		}

		public TMessage Deserialize<TMessage>(System.ReadOnlySpan<byte> span) where TMessage : IBaseMessage
		{
			// TODO: Não usar span.ToArray() usar span
			using (var textReader = new StringReader(Encoding.UTF8.GetString(span.ToArray())))
			{
				var messageTypeName = textReader.ReadLine();
				var messageContents = textReader.ReadToEnd();
				var messageType = TypeManager.TypeManager.Get(messageTypeName);

				return (TMessage)JsonConvert.DeserializeObject(messageContents, messageType);
			}
		}
	}
}
