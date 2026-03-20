using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class DroppedItemList : ScriptableObject
	{
		[SerializeField]
		private List<DroppedItem> m_items;

		private System.Random m_RNG = new System.Random();

		public DroppedItem InstantiateRandom(FurbyPersonality personality)
		{
			DroppedItem randomPrefab = GetRandomPrefab(personality);
			GameObject gameObject = UnityEngine.Object.Instantiate(randomPrefab.gameObject) as GameObject;
			return gameObject.GetComponent<DroppedItem>();
		}

		private DroppedItem GetRandomPrefab(FurbyPersonality personality)
		{
			List<DroppedItem> itemsForPersonality = GetItemsForPersonality(m_items.GetEnumerator(), personality);
			float num = 0f;
			foreach (DroppedItem item in itemsForPersonality)
			{
				num += item.m_likelihood;
			}
			float num2 = (float)m_RNG.NextDouble() * num;
			Logging.Log(string.Format("Random {0} in range 0 - {1}", num2, num));
			foreach (DroppedItem item2 in itemsForPersonality)
			{
				num2 -= item2.m_likelihood;
				if (num2 < 0f)
				{
					return item2;
				}
			}
			throw new ApplicationException(string.Format("Failed to choose a weighted random of {0} elements for personality {1}", itemsForPersonality.Count, personality));
		}

		private static List<DroppedItem> GetItemsForPersonality(IEnumerator<DroppedItem> source, FurbyPersonality p)
		{
			List<DroppedItem> list = new List<DroppedItem>();
			uint num = 0u;
			while (source.MoveNext())
			{
				num++;
				DroppedItem current = source.Current;
				if (current.IsRelevantForPersonality(p))
				{
					list.Add(current);
				}
			}
			Logging.Log(string.Format("{0} of {1} items are relevant for personality {2}", list.Count, num, p));
			return list;
		}
	}
}
