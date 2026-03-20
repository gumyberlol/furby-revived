using Furby;
using Relentless;
using UnityEngine;

public class GetEggButtonLogic : MonoBehaviour
{
	[SerializeField]
	private GameObject m_EggMenu;

	private void OnClick()
	{
		if (FurbyGlobals.BabyRepositoryHelpers.IsEggCartonFull())
		{
			GameEventRouter.SendEvent(SharedGuiEvents.DialogShow);
		}
		else
		{
			m_EggMenu.SetActive(true);
		}
	}
}
