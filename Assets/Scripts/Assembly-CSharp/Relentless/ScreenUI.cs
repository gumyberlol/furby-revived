using UnityEngine;

namespace Relentless
{
	public class ScreenUI : RelentlessMonoBehaviour
	{
		private bool m_firstTime = true;

		public void OnEnable()
		{
			if (!m_firstTime)
			{
				return;
			}
			foreach (Transform item in base.gameObject.transform)
			{
				if (string.Compare(Application.loadedLevelName, item.gameObject.name) == 0)
				{
					item.gameObject.SetActive(false);
				}
			}
			m_firstTime = false;
		}
	}
}
