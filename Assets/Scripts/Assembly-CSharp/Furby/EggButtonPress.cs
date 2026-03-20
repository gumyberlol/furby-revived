using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class EggButtonPress : MonoBehaviour
	{
		private float GetLevelCompletion(int xp, int level)
		{
			float result = 1f;
			if (level < FurbyGlobals.AdultLibrary.XpLevels.Count - 1)
			{
				float num = FurbyGlobals.AdultLibrary.XpLevels[level];
				float num2 = FurbyGlobals.AdultLibrary.XpLevels[level + 1];
				result = ((float)xp - num) / (num2 - num);
			}
			return result;
		}

		private void OnAcceptSpecialEgg(Enum specialEgg, GameObject go, params object[] parameters)
		{
			FurbyData furby = FurbyGlobals.Player.Furby;
			Logging.Log(furby);
			AdultFurbyType type = FurbyGlobals.Player.Furby.UnlocksInOrder.First();
			FurbyBabyTypeInfo nextBabyTypeFromVirtualFurby = FurbyGlobals.AdultLibrary.GetAdultFurby(type).GetNextBabyTypeFromVirtualFurby();
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(nextBabyTypeFromVirtualFurby.TypeID);
			GameEventRouter.SendEvent(BabyLifecycleEvent.FromSpecialFriend, null, furbyBaby);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			FurbyGlobals.Player.NumEggsAvailable = 0;
			FurbyGlobals.ScreenSwitcher.SwitchScreen("EggCarton", true);
		}

		private void OnClick()
		{
			if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				if (FurbyGlobals.Player.NumEggsAvailable > 0)
				{
					if (FurbyGlobals.BabyRepositoryHelpers.IsEggCartonFull())
					{
						GameEventRouter.SendEvent(EggCartonFullEvent.EggCartonFull);
					}
					else
					{
						FurbyGlobals.ScreenSwitcher.SwitchScreen("EggTransfer", true);
					}
				}
			}
			else if (FurbyGlobals.VideoSettings.m_showVideos)
			{
				string videoName = FurbyGlobals.VideoFilenameLookup.GetVideoName(TutorialVideoEvents.HatchingVideo);
				if (!Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(videoName))
				{
					Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Add(videoName);
					Singleton<GameDataStoreObject>.Instance.Save();
				}
				GameEventRouter.SendEvent(TutorialVideoEvents.HatchingVideo);
			}
		}
	}
}
