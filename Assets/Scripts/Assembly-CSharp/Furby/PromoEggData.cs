using System;
using UnityEngine;

namespace Furby
{
	public class PromoEggData : ScriptableObject
	{
		[Serializable]
		public class PurchaseableEgg
		{
			[SerializeField]
			public FurbyBabyTypeInfo m_babyInfo;

			[SerializeField]
			public UIAtlas m_atlas;

			[SerializeField]
			public string m_spriteName;

			[SerializeField]
			public int m_furbucksCost;
		}

		[SerializeField]
		public PurchaseableEgg[] m_eggsAvailable;
	}
}
