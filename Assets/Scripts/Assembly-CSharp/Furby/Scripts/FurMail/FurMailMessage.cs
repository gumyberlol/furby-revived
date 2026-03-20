using System;
using System.Text;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class FurMailMessage : FurMailServerMessage
	{
		public string MessageSubject;

		public string MessageBody;

		public bool IsRead;

		public DateTime ReceivedTime = DateTime.Now;

		public FurMailMessage()
		{
		}

		public FurMailMessage(FurMailServerMessage serverMessage)
		{
			MessageId = serverMessage.MessageId;
			ReceivedTime = DateTime.Now;
			Update(serverMessage);
		}

		public void Update(FurMailServerMessage serverMessage)
		{
			MessageSubject = Encoding.UTF8.GetString(Convert.FromBase64String(serverMessage.MessageSubject_Base64_UTF8));
			MessageBody = Encoding.UTF8.GetString(Convert.FromBase64String(serverMessage.MessageBody_Base64_UTF8));
			LanguageCode = serverMessage.LanguageCode;
			TemplateName = serverMessage.TemplateName;
			Created = serverMessage.Created;
			LastModified = serverMessage.LastModified;
			Priority = serverMessage.Priority;
		}
	}
}
