using Relentless;
using UnityEngine;

namespace Furby
{
	public class SynchronizeOtherObject : RelentlessMonoBehaviour
	{
		public GameObject m_OtherObject;

		public SynchronizeType m_Type;

		public void OnEnable()
		{
			if ((bool)m_OtherObject)
			{
				switch (m_Type)
				{
				case SynchronizeType.MatchThisObject:
					m_OtherObject.SetActive(true);
					break;
				case SynchronizeType.OpposeThisObject:
					m_OtherObject.SetActive(false);
					break;
				}
			}
		}

		public void OnDisable()
		{
			if ((bool)m_OtherObject)
			{
				switch (m_Type)
				{
				case SynchronizeType.MatchThisObject:
					m_OtherObject.SetActive(false);
					break;
				case SynchronizeType.OpposeThisObject:
					m_OtherObject.SetActive(true);
					break;
				}
			}
		}
	}
}
