using Relentless;
using UnityEngine;

namespace Furby
{
	public class FirstTimeFlowToggle : MonoBehaviour
	{
		private void Start()
		{
			bool flag = IsFirstSaveGame();
			bool flag2 = IsUpgradableGame();
			bool flag3 = !flag && !flag2;
			bool tutorialState = flag;
			base.gameObject.SetActive(flag3);
			SetTutorialState(tutorialState);
		}

		private bool IsFirstSaveGame()
		{
			bool flag = false;
			foreach (GameData item in Singleton<GameDataStoreObject>.Instance)
			{
				flag |= item.IsActiveGame;
			}
			return !flag;
		}

		private bool IsUpgradableGame()
		{
			return FurbyGlobals.SettingsHelper.IsUpgradableGame();
		}

		private void SetTutorialState(bool shouldStartTutorial)
		{
			Singleton<GameDataStoreObject>.Instance.Data.EarnedXP = 0;
			if (shouldStartTutorial)
			{
				Singleton<GameDataStoreObject>.Instance.Data.FlowStage = FlowStage.Dashboard_Initial;
				return;
			}
			if (!IsUpgradableGame())
			{
				Singleton<GameDataStoreObject>.Instance.Data.EarnedXP = 5;
			}
			Singleton<GameDataStoreObject>.Instance.Data.FlowStage = FlowStage.Normal;
		}
	}
}
