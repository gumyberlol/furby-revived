using System.Collections;
using Furby.Scanner;
using Relentless;
using UnityEngine;

namespace Furby.Dashboard
{
	public class SicknessChance : MonoBehaviour
	{
		public float m_SicknessChance = 0.3f;

		public FurbyCommand m_CommandToSend = FurbyCommand.Sickness;

		public ScannerBehaviour m_ScannerBehaviourComponent;

		public static bool s_AttemptedForcedSicknessThisBoot;

		private void Start()
		{
			if (!s_AttemptedForcedSicknessThisBoot)
			{
				if (Singleton<GameDataStoreObject>.Instance.Data.DashboardVisitCount > 1)
				{
					ApplyProbabilityOfMakingFurbySick();
					s_AttemptedForcedSicknessThisBoot = true;
				}
				else
				{
					m_ScannerBehaviourComponent.enabled = true;
				}
			}
			else
			{
				m_ScannerBehaviourComponent.enabled = true;
			}
			Singleton<GameDataStoreObject>.Instance.Data.DashboardVisitCount++;
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		private void ApplyProbabilityOfMakingFurbySick()
		{
			int num = (int)(m_SicknessChance * 100f);
			int num2 = Random.Range(0, 100);
			if (num2 < num)
			{
				Logging.Log("SicknessChance - MAKING SICK");
				StartCoroutine(SendSicknessTones());
			}
			else
			{
				Logging.Log("SicknessChance - NOT making sick, maybe next time...");
				m_ScannerBehaviourComponent.enabled = true;
			}
		}

		private IEnumerator SendSicknessTones()
		{
			yield return this.HeartBeatAndWaitOnSend();
			yield return this.WaitWhileComAirIsBusy();
			yield return this.CommandAndWaitOnSend(m_CommandToSend);
			yield return this.WaitWhileComAirIsBusy();
			m_ScannerBehaviourComponent.enabled = true;
		}
	}
}
