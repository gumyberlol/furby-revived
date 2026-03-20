using System;
using Relentless;
using UnityEngine;

namespace Furby.SaveSlotSelect
{
	public class SaveSlotSelectLogic : GameEventReceiver
	{
		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference TargetScreenNewGame;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference SaveGameSelectScene;

		[SerializeField]
		private NamedTextReference m_noFurbyNamedText;

		[SerializeField]
		private string m_nameFormat = "{0}-{1}";

		private int m_deletionSlotIndex;

		public override Type EventType
		{
			get
			{
				return typeof(SaveSlotSelectGameEvent);
			}
		}

		private void Start()
		{
			FurbyGlobals.Player.SetScannedFlag(false);
			Singleton<GameDataStoreObject>.Instance.SetSaveSlotIndex(-1);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			SaveSlotSelectGameEvent saveSlotSelectGameEvent = (SaveSlotSelectGameEvent)(object)enumValue;
			switch (saveSlotSelectGameEvent)
			{
			case SaveSlotSelectGameEvent.SelectNextFreeSlot:
			{
				for (int i = 0; i < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); i++)
				{
					if (!Singleton<GameDataStoreObject>.Instance.GetSlot(i).HasCompletedFirstTimeFlow)
					{
						Singleton<GameDataStoreObject>.Instance.SetSaveSlotIndex(i);
						FurbyGlobals.ScreenSwitcher.SwitchScreen(TargetScreenNewGame, true);
						break;
					}
				}
				break;
			}
			case SaveSlotSelectGameEvent.SelectSlot0:
			case SaveSlotSelectGameEvent.SelectSlot1:
			case SaveSlotSelectGameEvent.SelectSlot2:
			{
				int num = (int)saveSlotSelectGameEvent;
				Singleton<GameDataStoreObject>.Instance.SetSaveSlotIndex(num);
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode;
				if (!Singleton<GameDataStoreObject>.Instance.GetSlot(num).HasCompletedFirstTimeFlow)
				{
					Singleton<GameDataStoreObject>.Instance.Data.AudioVolume = Singleton<GameDataStoreObject>.Instance.GlobalData.GetPreSaveGameLoadAudioVolume();
					Singleton<GameDataStoreObject>.Instance.Save();
					FurbyGlobals.ScreenSwitcher.SwitchScreen(TargetScreenNewGame, true);
				}
				else if (Singleton<GameDataStoreObject>.Instance.Data.FurbyType == AdultFurbyType.Unknown)
				{
					FurbyGlobals.ScreenSwitcher.SwitchScreen("ScanningScreen", true);
				}
				else if (FurbyGlobals.SettingsHelper.IsUpgradableGame())
				{
					FurbyGlobals.ScreenSwitcher.SwitchScreen("ModeChoice");
				}
				else
				{
					Singleton<LaunchIntoFirstTimeFlow>.Instance.SwitchToStartScreen();
				}
				break;
			}
			case SaveSlotSelectGameEvent.DeleteSlot0:
				DeletePrompt(0);
				break;
			case SaveSlotSelectGameEvent.DeleteSlot1:
				DeletePrompt(1);
				break;
			case SaveSlotSelectGameEvent.DeleteSlot2:
				DeletePrompt(2);
				break;
			case SaveSlotSelectGameEvent.ConfirmDelete:
				Singleton<GameDataStoreObject>.Instance.Clear(m_deletionSlotIndex);
				FurbyGlobals.ScreenSwitcher.SwitchScreen(SaveGameSelectScene, false);
				break;
			case SaveSlotSelectGameEvent.ShowDeleteDialog:
				break;
			}
		}

		private void DeletePrompt(int index)
		{
			m_deletionSlotIndex = index;
			GameData slot = Singleton<GameDataStoreObject>.Instance.GetSlot(m_deletionSlotIndex);
			string text = "UNKNOWN";
			if (slot != null)
			{
				text = (slot.NoFurbyMode ? m_noFurbyNamedText.NamedText : string.Format(m_nameFormat, slot.FurbyNameLeft, slot.FurbyNameRight));
			}
			GameEventRouter.SendEvent(SaveSlotSelectGameEvent.ShowDeleteDialog, null, (m_deletionSlotIndex + 1).ToString(), text);
		}
	}
}
