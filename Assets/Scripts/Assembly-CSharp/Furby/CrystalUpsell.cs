using Relentless;
using UnityEngine;

namespace Furby
{
	public class CrystalUpsell : MonoBehaviour
	{
		public int m_showUpsellEveryXTimes = 20;

		private void Start()
		{
			GameDataStoreObject instance = Singleton<GameDataStoreObject>.Instance;
			GameData data = instance.Data;
			if (instance.GlobalData.AmEligibleForCrystal && !instance.GlobalData.CrystalUnlocked && data.HasCompletedFirstTimeFlow && data.DashboardVisitCount != 0 && (data.DashboardVisitCount == 1 || data.DashboardVisitCount % m_showUpsellEveryXTimes == 0))
			{
				base.gameObject.SendGameEvent(SharedGuiEvents.CrystalThemeLocked);
			}
		}
	}
}
