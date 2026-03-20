using System.Collections;
using Furby.Utilities.EggCarton;
using Relentless;
using UnityEngine;

namespace Furby.EggCarton
{
	public class EggGiftButton : RelentlessMonoBehaviour
	{
		[SerializeField]
		private MicAvailableChecker m_micAvailableChecker;

		[SerializeField]
		private Transform m_errorSpawnPoint;

		private void OnClick()
		{
			StartCoroutine(Flow());
		}

		private IEnumerator Flow()
		{
			bool waiting = true;
			bool available = false;
			yield return StartCoroutine(m_micAvailableChecker.Check(m_errorSpawnPoint, delegate(bool b)
			{
				waiting = false;
				available = b;
			}));
			while (waiting)
			{
				yield return null;
			}
			if (available)
			{
				GameEventRouter.SendEvent(CartonGameEvent.GiftButtonClicked);
			}
		}
	}
}
