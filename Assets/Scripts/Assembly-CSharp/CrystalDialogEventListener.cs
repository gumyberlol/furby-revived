using System.Collections;
using Furby;
using Relentless;
using UnityEngine;

public class CrystalDialogEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine("ListenForBuy");
	}

	private void OnDestory()
	{
		StopCoroutine("ListenForBuy");
	}

	private IEnumerator ListenForBuy()
	{
		WaitForGameEvent waiter = new WaitForGameEvent();
		yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.CrystalThemeBuy));
		SharedGuiEvents returnedEvent = (SharedGuiEvents)(object)waiter.ReturnedEvent;
		if (returnedEvent == SharedGuiEvents.CrystalThemeBuy)
		{
			SetStartingShop.s_startingShop = "InApp";
			FurbyGlobals.ScreenSwitcher.SwitchScreen("Shop", true);
		}
	}
}
