using System;
using Relentless;

namespace Furby.Scripts.FurMail
{
	[Serializable]
	public class GetMessagesRequestContent : MessageRequestContent
	{
		public TargetGroupInformation TargetGroupInformation;
	}
}
