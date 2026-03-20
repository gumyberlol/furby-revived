using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class GiftItem
	{
		[SerializeField]
		public string m_GiftID = string.Empty;

		[SerializeField]
		public GiftItemType m_GiftType;
	}
}
