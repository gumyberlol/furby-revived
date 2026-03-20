using System.Collections;
using Relentless;

namespace Furby
{
	public class GoldenUnlockingLogic : RelentlessMonoBehaviour
	{
		public float m_AnimationDuration = 5f;

		public IEnumerator Start()
		{
			yield return null;
			GameEventRouter.SendEvent(UnlockItemsEvents.GoldenItemsSequenceStarted);
			Invoke("BroadcastSequenceComplete", m_AnimationDuration);
		}

		private void BroadcastSequenceComplete()
		{
			GameEventRouter.SendEvent(UnlockItemsEvents.GoldenItemsSequenceEnded);
		}
	}
}
