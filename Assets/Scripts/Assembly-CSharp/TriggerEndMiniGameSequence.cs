using Furby;
using Relentless;
using UnityEngine;

public class TriggerEndMiniGameSequence : MonoBehaviour
{
	public int m_score = 500;

	public int m_furbucks = 50;

	private void OnClick()
	{
		FurbyGlobals.Wallet.Balance += m_furbucks;
		GameEventRouter.SendEvent(BabyEndMinigameEvent.SetScore, base.gameObject, m_score);
		GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowDialog);
	}
}
