using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class DroppedItem : MonoBehaviour
	{
		public enum ItemType
		{
			Hair_Small = 0,
			Hair_Large = 1,
			Dirt = 2,
			Rare = 3,
			Personality = 4
		}

		[SerializeField]
		private ItemType m_type;

		[SerializeField]
		public float m_likelihood = 1f;

		[SerializeField]
		private List<FurbyPersonality> m_limitPersonalities;

		[SerializeField]
		private GameObject m_assetPrefab;

		private GameObject m_asset;

		public ArriveAndDisperse ArriveAndDisperse
		{
			get
			{
				ArriveAndDisperse component = base.gameObject.GetComponent<ArriveAndDisperse>();
				if (component == null)
				{
					throw new ApplicationException(string.Format("{0} expected to have an ArriveAndDisperse component", base.gameObject.name));
				}
				return component;
			}
		}

		public bool IsRelevantForPersonality(FurbyPersonality p)
		{
			bool flag = m_limitPersonalities.Count == 0;
			bool flag2 = m_limitPersonalities.Contains(p);
			return flag || flag2;
		}

		public ItemType GetItemType()
		{
			return m_type;
		}

		public void Start()
		{
			if (m_assetPrefab == null)
			{
				throw new ApplicationException(string.Format("DroppedItem \"{0}\" is missing artwork.", base.gameObject.name));
			}
			m_asset = UnityEngine.Object.Instantiate(m_assetPrefab) as GameObject;
			m_asset.transform.parent = base.transform.parent;
			m_asset.transform.localPosition = Vector3.zero;
			ArriveAndDisperse arriveAndDisperse = ArriveAndDisperse;
			arriveAndDisperse.SetAnim(m_asset.animation);
			arriveAndDisperse.DispersalCompleted += delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			};
			arriveAndDisperse.ArrivalStarted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.ItemDropped);
			};
			arriveAndDisperse.ArrivalCompleted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.ItemArrivalComplete);
			};
			arriveAndDisperse.DispersalStarted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.ItemDepartureStarted);
			};
			arriveAndDisperse.DispersalCompleted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.ItemDepartureComplete);
			};
		}

		public void OnDestroy()
		{
			UnityEngine.Object.Destroy(m_asset);
		}
	}
}
