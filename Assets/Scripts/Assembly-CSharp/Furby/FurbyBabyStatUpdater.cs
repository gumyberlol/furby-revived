using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyBabyStatUpdater : MonoBehaviour
	{
		[SerializeField]
		private float m_timeBetweenUpdates = 10f;

		[SerializeField]
		private float m_animationTime = 1f;

		[SerializeField]
		private float m_initialPause = 1f;

		private IEnumerator Start()
		{
			PlayerFurby adultFurby = FurbyGlobals.Player;
			FurbyBaby furbyBaby = adultFurby.InProgressFurbyBaby;
			if (furbyBaby == null)
			{
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyAttention, null, 0f, 0f);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyCleanliness, null, 0f, 0f);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabySatiatedness, null, 0f, 0f);
			}
			else
			{
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyAttention, null, furbyBaby.Attention, 0f);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyCleanliness, null, furbyBaby.Cleanliness, 0f);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabySatiatedness, null, furbyBaby.Satiatedness, 0f);
			}
			GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBowelEmptiness, null, adultFurby.BowelEmptiness, 0f);
			GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyCleanliness, null, adultFurby.Cleanliness, 0f);
			GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyHappiness, null, adultFurby.Happiness, 0f);
			GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbySatietedness, null, adultFurby.Satiatedness, 0f);
			yield return new WaitForSeconds(m_initialPause);
			while (true)
			{
				if (furbyBaby != null)
				{
					furbyBaby.UpdateStats();
					GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyAttention, null, furbyBaby.Attention, m_animationTime);
					GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabyCleanliness, null, furbyBaby.Cleanliness, m_animationTime);
					GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBabySatiatedness, null, furbyBaby.Satiatedness, m_animationTime);
				}
				adultFurby.UpdateStats();
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyBowelEmptiness, null, adultFurby.BowelEmptiness, m_animationTime);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyCleanliness, null, adultFurby.Cleanliness, m_animationTime);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbyHappiness, null, adultFurby.Happiness, m_animationTime);
				GameEventRouter.SendEvent(FurbyStatEvent.UpdateFurbySatietedness, null, adultFurby.Satiatedness, m_animationTime);
				yield return new WaitForSeconds(m_timeBetweenUpdates);
			}
		}

		private void OnDestroy()
		{
			if ((bool)Singleton<GameDataStoreObject>.Instance)
			{
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}
	}
}
