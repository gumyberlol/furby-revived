using System.Collections.Generic;

namespace Furby.Scripts.FurMail
{
	public class FurMailMessageSorter : IComparer<FurMailMessage>
	{
		public int Compare(FurMailMessage x, FurMailMessage y)
		{
			if (x == null || y == null)
			{
				return 0;
			}
			if (!x.IsRead && y.IsRead)
			{
				return -1;
			}
			if (x.IsRead && !y.IsRead)
			{
				return 1;
			}
			if (x.ReceivedTime < y.ReceivedTime)
			{
				return -1;
			}
			if (x.ReceivedTime > y.ReceivedTime)
			{
				return 1;
			}
			if (x.Priority < y.Priority)
			{
				return -1;
			}
			if (x.Priority > y.Priority)
			{
				return 1;
			}
			if (x.Created < y.Created)
			{
				return -1;
			}
			if (x.Created > y.Created)
			{
				return 1;
			}
			return 0;
		}
	}
}
