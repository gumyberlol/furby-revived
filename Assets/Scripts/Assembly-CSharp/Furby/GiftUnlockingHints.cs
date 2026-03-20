using UnityEngine;

namespace Furby
{
	public class GiftUnlockingHints : MonoBehaviour
	{
		[SerializeField]
		public HintState m_SuggestFlicking;

		public void Update()
		{
			if (m_SuggestFlicking.IsEnabled())
			{
				m_SuggestFlicking.TestAndBroadcastState();
			}
		}
	}
}
