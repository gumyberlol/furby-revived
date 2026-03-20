using Relentless;
using UnityEngine;

namespace Furby
{
	public class DisableIfTribeIneligible : MonoBehaviour
	{
		public Tribeset m_Tribe;

		public GameObject m_TargetObject;

		private void OnEnable()
		{
			UpdateTargetState();
		}

		private void Awake()
		{
			UpdateTargetState();
		}

		private void UpdateTargetState()
		{
			if (m_Tribe == Tribeset.Spring || m_Tribe == Tribeset.CrystalGem)
			{
				DebugUtils.Log_InMagenta("DisableIfTribeIneligible:: On " + base.gameObject.name + ", " + m_Tribe);
				bool flag = false;
				if (m_Tribe == Tribeset.Spring)
				{
					flag = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring;
				}
				if (m_Tribe == Tribeset.CrystalGem)
				{
					flag = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
				}
				if (flag)
				{
					EnableTarget();
				}
				else
				{
					DisableTarget();
				}
			}
		}

		private void DisableTarget()
		{
			if (m_TargetObject == null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				m_TargetObject.SetActive(false);
			}
		}

		private void EnableTarget()
		{
			if (m_TargetObject == null)
			{
				base.gameObject.SetActive(true);
			}
			else
			{
				m_TargetObject.SetActive(true);
			}
		}
	}
}
