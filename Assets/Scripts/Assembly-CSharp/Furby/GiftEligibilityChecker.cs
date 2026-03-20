using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GiftEligibilityChecker : MonoBehaviour
	{
		[SerializeField]
		private GiftDialog m_dialogPrefab;

		[SerializeField]
		private Transform m_dialogSpawnPoint;

		[SerializeField]
		private GiftAwarder m_awarder;

		public IEnumerator Start()
		{
			GameData gd = Singleton<GameDataStoreObject>.Instance.Data;
			bool eligible = gd.HasEligibilityForGift;
			bool canAward = m_awarder.CanAwardAGift();
			bool firstTimeFlow = gd.FlowStage != FlowStage.Normal;
			if (eligible && canAward && !firstTimeFlow)
			{
				yield return StartCoroutine(SingletonInstance<ModalityMediator>.Instance.WaitAndAcquire(this, null));
				m_awarder.AwardRandomGift();
				gd.MarkGiftAwarded();
				GiftDialog d = ShowDialog();
				d.Destroyed += delegate
				{
					SingletonInstance<ModalityMediator>.Instance.Release(this);
				};
			}
		}

		private GiftDialog ShowDialog()
		{
			GiftDialog giftDialog = Object.Instantiate(m_dialogPrefab) as GiftDialog;
			giftDialog.transform.parent = m_dialogSpawnPoint;
			giftDialog.transform.localPosition = Vector3.zero;
			giftDialog.transform.localScale = Vector3.one;
			giftDialog.gameObject.layer = m_dialogSpawnPoint.gameObject.layer;
			return giftDialog;
		}
	}
}
