using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class GiftEligibilityThresholds : ScriptableObject
	{
		[SerializeField]
		private string m_comments;

		[SerializeField]
		private List<int> m_furblingCounts;

		public int GiftCountForHatchCount(int hatches)
		{
			int num = m_furblingCounts.FindIndex((int x) => x > hatches);
			return (num < 0) ? m_furblingCounts.Count : num;
		}
	}
}
