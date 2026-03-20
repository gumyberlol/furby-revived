using System;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class FurMailServerMessage
	{
		public Guid MessageId;

		public string MessageSubject_Base64_UTF8;

		public string MessageBody_Base64_UTF8;

		public string LanguageCode;

		public string TemplateName;

		public DateTime Created;

		public DateTime LastModified;

		public int Priority;
	}
}
