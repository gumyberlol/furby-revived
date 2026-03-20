using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class EggFromSpecialFriend : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum m_dialogEvent;

		private IEnumerator Start()
		{
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				while (FurbyGlobals.Player.NumEggsAvailable != 1)
				{
					yield return null;
				}
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForEvent(DashboardGameEvent.Egg_Sparkling));
				FurbyData furbyData = FurbyGlobals.Player.Furby;
				Logging.Log(furbyData);
				IEnumerable<AdultFurbyType> unlocks = FurbyGlobals.Player.Furby.UnlocksInOrder;
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					unlocks = FurbyGlobals.Player.NoFurbyUnlock.UnlocksInOrder;
				}
				AdultFurbyType firstVirtualFriend = unlocks.First();
				FurbyBabyTypeInfo furbyType = FurbyGlobals.AdultLibrary.GetAdultFurby(firstVirtualFriend).GetNextBabyTypeFromVirtualFurby();
				FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(furbyType.TypeID);
				GameEventRouter.SendEvent(BabyLifecycleEvent.FromSpecialFriend, null, furbyBaby);
				furbyBaby.Progress = FurbyBabyProgresss.E;
				FurbyGlobals.Player.NumEggsAvailable = 0;
				if (Singleton<GameDataStoreObject>.Instance.Data.FlowStage == FlowStage.Normal)
				{
					GameEventRouter.SendEvent(m_dialogEvent.Value);
				}
			}
		}
	}
}
