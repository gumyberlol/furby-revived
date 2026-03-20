using Furby;
using Relentless;
using UnityEngine;

public class CrystalTribeHoodNavigation : MonoBehaviour
{
	[LevelReferenceRootFolder("Furby/Scenes")]
	public LevelReference m_CrystalTribeNeighbourhoodScene;

	[LevelReferenceRootFolder("Furby/Scenes")]
	public LevelReference m_InAppShopScene;

	[SerializeField]
	protected GameObject[] m_closeTargets;

	public void OnClick()
	{
		GameDataStoreObject instance = Singleton<GameDataStoreObject>.Instance;
		if (instance.GlobalData.AmEligibleForCrystal)
		{
			if (instance.GlobalData.CrystalUnlocked)
			{
				NavigateToNeighbourhood();
			}
			else
			{
				NavigateToInAppShop();
			}
		}
		else
		{
			Logging.Log("CrystalTribeHoodNavigation:Ineligible for Crystal, doing NOWT");
		}
	}

	private void NavigateToInAppShop()
	{
		Logging.Log("CrystalTribeHoodNavigation:NavigateToInAppShop");
		base.gameObject.SendGameEvent(SharedGuiEvents.CrystalThemeLocked);
		GameObject[] closeTargets = m_closeTargets;
		foreach (GameObject gameObject in closeTargets)
		{
			gameObject.SetActive(!gameObject.activeSelf);
		}
	}

	private void NavigateToNeighbourhood()
	{
		Logging.Log("CrystalTribeHoodNavigation:NavigateToNeighbourhood");
		if (!SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled)
		{
			SpsScreenSwitcher.s_SwitchOfAnyTypeIsBeingHandled = true;
			FurbyGlobals.ScreenSwitcher.SwitchScreen(m_CrystalTribeNeighbourhoodScene, true);
		}
	}
}
