using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GiftList : MonoBehaviour
	{
		[SerializeField]
		public List<GiftItem> m_GiftItemData = new List<GiftItem>();

		public int GetIndexOfGiftByName(string name)
		{
			for (int i = 0; i < m_GiftItemData.Count; i++)
			{
				if (m_GiftItemData[i].m_GiftID.Equals(name))
				{
					return i;
				}
			}
			string text = "Cant find a gift named [" + name + "]";
			DebugUtils.Assert(false, text);
			Logging.LogError(text);
			return 0;
		}

		public GiftItem GetGiftByName(string name)
		{
			foreach (GiftItem giftItemDatum in m_GiftItemData)
			{
				if (giftItemDatum.m_GiftID.Equals(name))
				{
					return giftItemDatum;
				}
			}
			string text = "Cant find a gift named [" + name + "]";
			DebugUtils.Assert(false, text);
			Logging.LogError(text);
			return m_GiftItemData[0];
		}
	}
}
