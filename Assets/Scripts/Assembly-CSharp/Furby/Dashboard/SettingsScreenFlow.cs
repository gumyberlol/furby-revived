using System;
using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Dashboard
{
	public class SettingsScreenFlow : RelentlessMonoBehaviour
	{
		private enum SettingsPage
		{
			GlobalSettingsMainPage = 0,
			AdvancedSettingsPage = 1,
			InGameSettingsMainPage = 2,
			PreferencesSettingsPage = 3,
			ManageGameSettingsPage = 4
		}

		private const int MAX_BUTTONS = 6;

		private const int BUTTON_UP_EVENT_INDEX = 0;

		private const int BUTTON_MAIN_EVENT_INDEX = 3;

		[SerializeField]
		private SettingsPage m_SettingsPage;

		[SerializeField]
		private CodeControlledDialogBox m_DialogBox;

		[SerializeField]
		private GameObject m_SettingsBanner;

		[SerializeField]
		private UIPanel m_Panel;

		[SerializeField]
		private GameObject m_ButtonPrefab;

		[SerializeField]
		private GameObject m_VolumeMeter;

		[SerializeField]
		private GameObject m_ChangeLanguagePanel;

		[SerializeField]
		private UIPanel m_needFurbyDialog;

		[SerializeField]
		private UIPanel m_scanningInstructions;

		[SerializeField]
		private Transform m_needFurbyDialogSpawnPoint;

		private GameObject[] m_Buttons = new GameObject[6];

		private Vector3 m_LayoutPosition = Vector3.zero;

		private Vector3 m_LayoutPositionStart = new Vector3(0f, 250f, 0f);

		private Vector3 m_LayoutPositionIncrement = new Vector3(0f, -120f, 0f);

		private Vector3 m_LayoutVolumeMeterOffset = new Vector3(0f, -100f, 0f);

		private void Awake()
		{
			if (m_DialogBox == null)
			{
				Logging.LogError("No Dialog Box Reference!");
			}
			if (m_SettingsBanner == null)
			{
				Logging.LogError("No Settings Banner Reference!");
			}
			if (m_Panel == null)
			{
				Logging.LogError("No Panel Reference!");
			}
			if (m_ButtonPrefab == null)
			{
				Logging.LogError("No Button Prefab Reference!");
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_ButtonPrefab);
				gameObject.transform.parent = m_Panel.transform;
				gameObject.transform.localScale = Vector3.one;
				m_Buttons[i] = gameObject;
			}
		}

		private void ResetFlow()
		{
			StopAllCoroutines();
			ResetState();
			StartCoroutine(StartSettingsScreenFlow());
		}

		private void ResetState()
		{
			HideEverything();
			switch (m_SettingsPage)
			{
			case SettingsPage.GlobalSettingsMainPage:
				ResetStateForGlobalMainPage();
				break;
			case SettingsPage.AdvancedSettingsPage:
				ResetStateForAdvancedSettingsPage();
				break;
			case SettingsPage.InGameSettingsMainPage:
				ResetStateForInGameMainPage();
				break;
			case SettingsPage.PreferencesSettingsPage:
				ResetStateForPreferences();
				break;
			case SettingsPage.ManageGameSettingsPage:
				ResetStateForManageGamePage();
				break;
			}
		}

		private void HideEverything()
		{
			m_DialogBox.Hide();
			m_VolumeMeter.SetActive(false);
			GameObject[] buttons = m_Buttons;
			foreach (GameObject button in buttons)
			{
				HideButton(button);
			}
		}

		private void ShowVolumeMeter()
		{
			m_VolumeMeter.SetActive(true);
			m_LayoutPosition += m_LayoutVolumeMeterOffset;
		}

		private void StartLayout()
		{
			m_LayoutPosition = m_LayoutPositionStart;
		}

		private void IncrementLayoutPosition()
		{
			m_LayoutPosition += m_LayoutPositionIncrement;
		}

		private void HideButton(GameObject button)
		{
			button.SetActive(false);
		}

		private void ShowButton(GameObject button, bool shouldDisable, SerialisableEnum gameEvent, string text)
		{
			ShowButton(button, shouldDisable, gameEvent, text, false);
		}

		private void ShowButton(GameObject button, bool shouldDisable, SerialisableEnum gameEvent, string text, bool isLocked)
		{
			LockedButtonSwitcher component = button.GetComponent<LockedButtonSwitcher>();
			component.m_isLocked = isLocked;
			button.SetActive(true);
			button.transform.localPosition = m_LayoutPosition;
			IncrementLayoutPosition();
			if (shouldDisable)
			{
				DisableButton(button);
			}
			else
			{
				EnableButton(button);
			}
			SetButtonGameEvent(button, 3, gameEvent);
			SetButtonText(button, text);
		}

		private void EnableButton(GameObject button)
		{
			UISlicedSprite componentInChildren = button.GetComponentInChildren<UISlicedSprite>();
			componentInChildren.color = new Color(0f, 13f / 15f, 0.99607843f);
			SetButtonGameEvent(button, 0, SharedGuiEvents.ButtonUp);
		}

		private void DisableButton(GameObject button)
		{
			UISlicedSprite componentInChildren = button.GetComponentInChildren<UISlicedSprite>();
			componentInChildren.color = Color.grey;
			SetButtonGameEvent(button, 0, SharedGuiEvents.Cancel);
		}

		private void SetButtonGameEvent(GameObject button, int eventIndex, SerialisableEnum gameEvent)
		{
			GameEventOnNGUIEvent[] componentsInChildren = button.GetComponentsInChildren<GameEventOnNGUIEvent>();
			componentsInChildren[eventIndex].GameEvent = gameEvent;
		}

		private void SetButtonText(GameObject button, string text)
		{
			SetLocalisedText(button, text);
		}

		private void SetSettingsBannerText(string text)
		{
			SetLocalisedText(m_SettingsBanner, text);
		}

		private void SetLocalisedText(GameObject objectToLocalise, string text)
		{
			NGUILocaliser component = objectToLocalise.GetComponent<NGUILocaliser>();
			component.LocalisedStringKey = text;
			component.UpdateUI();
		}

		private void Start()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = FurbyGlobals.Player.NoFurbyOnSaveGame();
			ResetFlow();
		}

		private IEnumerator StartSettingsScreenFlow()
		{
			switch (m_SettingsPage)
			{
			case SettingsPage.GlobalSettingsMainPage:
				yield return StartCoroutine(StartGlobalMainPageSettingsScreenFlow());
				break;
			case SettingsPage.AdvancedSettingsPage:
				yield return StartCoroutine(StartAdvancedSettingsScreenFlow());
				break;
			case SettingsPage.InGameSettingsMainPage:
				yield return StartCoroutine(StartInGameMainPageSettingsScreenFlow());
				break;
			case SettingsPage.PreferencesSettingsPage:
				yield return StartCoroutine(StartPreferencesSettingsScreenFlow());
				break;
			case SettingsPage.ManageGameSettingsPage:
				yield return StartCoroutine(StartManageGameSettingsScreenFlow());
				break;
			default:
				Logging.LogError(string.Format("Unexpected SettingsPage in SettingsScreenFlow::StartSettingsScreenFlow : {0}", m_SettingsPage.ToString()));
				break;
			}
		}

		private void ResetStateForGlobalMainPage()
		{
			SetSettingsBannerText("SETTINGS_SCREEN_TITLE");
			StartLayout();
			int num = 0;
			bool shouldDisable = true;
			bool shouldDisable2 = false;
			if (!AreThereAnySaveGames())
			{
				ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_DELETESAVESLOT");
			}
			else
			{
				ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.SwitchToDeleteSaveGameScreen, "SETTINGS_SCREEN_DELETESAVESLOT");
			}
			ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.PrivacyPolicyButtonClicked, "SETTINGS_SCREEN_PRIVACYPOLICY");
			ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.SwitchToAdvancedSettingsScreen, "SETTINGS_SCREEN_ADVANCEDSETTINGS");
			if (SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable())
			{
				ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.RestorePurchases, "SETTINGS_SCREEN_RESTOREPURCHASES");
			}
		}

		private bool AreThereAnySaveGames()
		{
			for (int i = 0; i < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); i++)
			{
				if (Singleton<GameDataStoreObject>.Instance.GetSlot(i).HasCompletedFirstTimeFlow)
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerator StartGlobalMainPageSettingsScreenFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.BackButtonClicked, SettingsPageEvents.PrivacyPolicyButtonClicked, SettingsPageEvents.RestorePurchases));
				SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
				switch (returnedEvent)
				{
				case SettingsPageEvents.BackButtonClicked:
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPreviousScreen);
					break;
				case SettingsPageEvents.PrivacyPolicyButtonClicked:
					FurbyGlobals.SettingsHelper.SetCameFromSettings();
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPrivacyPolicyScreen);
					break;
				case SettingsPageEvents.RestorePurchases:
					GameEventRouter.SendEvent(RestorePurchasesEvent.InvokeRestorePurchasesFlow);
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::StartGlobalMainPageSettingsScreenFlow : {0}", returnedEvent.ToString()));
					break;
				}
			}
		}

		private void ResetStateForAdvancedSettingsPage()
		{
			SetSettingsBannerText("SETTINGS_SCREEN_ADVANCEDSETTINGS");
			StartLayout();
			int num = 0;
			bool shouldDisable = false;
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.ChangeLanguageButtonClicked, "SETTINGS_SCREEN_CHANGELANGUAGE");
			bool flag = !FurbyGlobals.DeviceSettings.DeviceIsKnown;
			if (true)
			{
				ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.SwitchToFurbyCommsScreen, "SETTINGS_SCREEN_FURBYCOMMS");
				ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.SwitchToScreenOrientationScreen, "SETTINGS_SCREEN_SCREENORIENTATION");
			}
		}

		private IEnumerator StartAdvancedSettingsScreenFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.ChangeLanguageButtonClicked, SettingsPageEvents.BackButtonClicked));
				SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
				switch (returnedEvent)
				{
				case SettingsPageEvents.ChangeLanguageButtonClicked:
					m_ChangeLanguagePanel.SetActive(true);
					break;
				case SettingsPageEvents.BackButtonClicked:
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPreviousScreen);
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::StartAdvancedSettingsScreenFlow : {0}", returnedEvent.ToString()));
					break;
				}
			}
		}

		private void ResetStateForInGameMainPage()
		{
			SetSettingsBannerText("SETTINGS_SCREEN_TITLE");
			StartLayout();
			int num = 0;
			bool shouldDisable = false;
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.SwitchToPreferencesScreen, "SETTINGS_SCREEN_PREFERENCES");
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.SwitchToManageGameScreen, "SETTINGS_SCREEN_MANAGEGAME");
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.PrivacyPolicyButtonClicked, "SETTINGS_SCREEN_PRIVACYPOLICY");
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.ChangeUserButtonClicked, "SETTINGS_SCREEN_CHANGEUSER");
			ShowButton(m_Buttons[num++], shouldDisable, SettingsPageEvents.RestorePurchases, "SETTINGS_SCREEN_RESTOREPURCHASES");
		}

		private IEnumerator StartInGameMainPageSettingsScreenFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.BackButtonClicked, SettingsPageEvents.PrivacyPolicyButtonClicked, SettingsPageEvents.ChangeUserButtonClicked, SettingsPageEvents.RestorePurchases));
				SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
				switch (returnedEvent)
				{
				case SettingsPageEvents.BackButtonClicked:
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToDashboard);
					break;
				case SettingsPageEvents.PrivacyPolicyButtonClicked:
					FurbyGlobals.SettingsHelper.SetCameFromSettings();
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPrivacyPolicyScreen);
					break;
				case SettingsPageEvents.ChangeUserButtonClicked:
					FurbyGlobals.HardwareSettingsScreenFlow.SetIsControllingGlobalInGameVolume(true);
					Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
					GameEventRouter.SendEvent(SettingsPageEvents.SwitchToChangeUserScreen);
					break;
				case SettingsPageEvents.RestorePurchases:
					GameEventRouter.SendEvent(RestorePurchasesEvent.InvokeRestorePurchasesFlow);
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::StartInGameMainPageSettingsScreenFlow : {0}", returnedEvent.ToString()));
					break;
				}
			}
		}

		private void ResetStateForPreferences()
		{
			SetSettingsBannerText("SETTINGS_SCREEN_PREFERENCES");
			StartLayout();
			ShowVolumeMeter();
			int num = 0;
			GameDataStoreObject instance = Singleton<GameDataStoreObject>.Instance;
			AppLookAndFeel appLookAndFeel = instance.Data.AppLookAndFeel;
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal)
			{
				if (!instance.GlobalData.CrystalUnlocked)
				{
					ShowButton(m_Buttons[num++], false, SharedGuiEvents.CrystalThemeLocked, "SETTINGS_SCREEN_CRYSTALTHEME", true);
				}
				else
				{
					ShowButton(m_Buttons[num++], false, SettingsPageEvents.ChangeCrystalTheme, (appLookAndFeel != AppLookAndFeel.Normal) ? "SETTINGS_SCREEN_REGULARTHEME" : "SETTINGS_SCREEN_CRYSTALTHEME");
				}
			}
			ShowButton(m_Buttons[num++], false, SettingsPageEvents.ChangeTheme, "SETTINGS_SCREEN_CHANGETHEME");
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				ShowButton(m_Buttons[num++], true, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_REFRESHFURBY");
			}
			else
			{
				ShowButton(m_Buttons[num++], false, SettingsPageEvents.RefreshFurbyStatuses, "SETTINGS_SCREEN_REFRESHFURBY");
			}
		}

		private IEnumerator StartPreferencesSettingsScreenFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			Enum[] events = new Enum[3]
			{
				SettingsPageEvents.RefreshFurbyStatuses,
				SettingsPageEvents.BackButtonClicked,
				SettingsPageEvents.ChangeCrystalTheme
			};
			yield return StartCoroutine(waiter.WaitForEvent(events));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.RefreshFurbyStatuses:
				FurbyGlobals.Player.SetScannedFlag(false);
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToDashboard);
				break;
			case SettingsPageEvents.BackButtonClicked:
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPreviousScreen);
				break;
			case SettingsPageEvents.ChangeCrystalTheme:
			{
				GameDataStoreObject store = Singleton<GameDataStoreObject>.Instance;
				GameData data = store.Data;
				data.AppLookAndFeel = ((data.AppLookAndFeel != AppLookAndFeel.Crystal) ? AppLookAndFeel.Crystal : AppLookAndFeel.Normal);
				if (!data.CanPlayAppChangeAnimation && data.AppLookAndFeel == AppLookAndFeel.Normal)
				{
					data.CanPlayAppChangeAnimation = true;
				}
				store.Save();
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToDashboard);
				break;
			}
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::StartPreferencesSettingsScreenFlow : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private bool IsEggCartonEmpty()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() == 0)
			{
				return true;
			}
			return false;
		}

		private bool IsNeighbourhoodEmpty()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.Neighbourhood.Count() == 0)
			{
				return true;
			}
			return false;
		}

		private void ResetStateForManageGamePage()
		{
			SetSettingsBannerText("SETTINGS_SCREEN_MANAGEGAME");
			StartLayout();
			int num = 0;
			bool shouldDisable = true;
			bool shouldDisable2 = false;
			if (IsNeighbourhoodEmpty())
			{
				ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_DELETEBABY");
			}
			else
			{
				ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.DeleteABaby, "SETTINGS_SCREEN_DELETEBABY");
			}
			if (IsEggCartonEmpty())
			{
				ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_DELETEEGG");
			}
			else
			{
				ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.DeleteAnEgg, "SETTINGS_SCREEN_DELETEEGG");
			}
			bool flag = FurbyGlobals.Player.NoFurbyOnSaveGame();
			bool flag2 = Singleton<GameDataStoreObject>.Instance.Data.FlowStage == FlowStage.Normal;
			if (flag)
			{
				ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_RENAME");
				ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_CHANGETOY");
				if (flag2)
				{
					ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.ShowUpgradePrompt, "SETTINGS_SCREEN_UPGRADEGAME");
				}
				else
				{
					ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_UPGRADEGAME");
				}
			}
			else
			{
				ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.ShowRenamePrompt, "SETTINGS_SCREEN_RENAME");
				if (flag2)
				{
					ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.ShowChangeFurbyPrompt, "SETTINGS_SCREEN_CHANGETOY");
					ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.ShowDowngradePrompt, "SETTINGS_SCREEN_DOWNGRADEGAME");
				}
				else
				{
					ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_CHANGETOY");
					ShowButton(m_Buttons[num++], shouldDisable, SharedGuiEvents.Cancel, "SETTINGS_SCREEN_DOWNGRADEGAME");
				}
			}
			ShowButton(m_Buttons[num++], shouldDisable2, SettingsPageEvents.ShowDeleteGamePrompt, "SETTINGS_SCREEN_DELETEGAME");
		}

		private IEnumerator StartManageGameSettingsScreenFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.DeleteABaby, SettingsPageEvents.DeleteAnEgg, SettingsPageEvents.ShowRenamePrompt, SettingsPageEvents.ShowChangeFurbyPrompt, SettingsPageEvents.ShowDowngradePrompt, SettingsPageEvents.ShowUpgradePrompt, SettingsPageEvents.ShowDeleteGamePrompt, SettingsPageEvents.BackButtonClicked));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.DeleteABaby:
				FurbyGlobals.SettingsHelper.RequestDeleteBaby();
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToNeighbourhood);
				break;
			case SettingsPageEvents.DeleteAnEgg:
				FurbyGlobals.SettingsHelper.RequestDeleteEgg();
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToEggCarton);
				break;
			case SettingsPageEvents.ShowRenamePrompt:
				yield return StartCoroutine(ShowConfirmRenameDialog());
				break;
			case SettingsPageEvents.ShowChangeFurbyPrompt:
				yield return StartCoroutine(ShowChangeFurbyDialog());
				break;
			case SettingsPageEvents.ShowDowngradePrompt:
				yield return StartCoroutine(ShowDowngradeDialog());
				break;
			case SettingsPageEvents.ShowUpgradePrompt:
				yield return StartCoroutine(ShowUpgradeDialog());
				break;
			case SettingsPageEvents.ShowDeleteGamePrompt:
				yield return StartCoroutine(ShowDeleteGameDialog());
				break;
			case SettingsPageEvents.BackButtonClicked:
				GameEventRouter.SendEvent(SettingsPageEvents.SwitchToPreviousScreen);
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::StartManageGameSettingsScreenFlow : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private IEnumerator ShowConfirmRenameDialog()
		{
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "FURBYNAMING_MESSAGE_RENAMEWARNING");
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "MENU_BUTTON_CONTINUE");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, SettingsPageEvents.GenericDialogAccept);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "MENU_OPTION_CANCEL");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, SettingsPageEvents.GenericDialogCancel);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept, SettingsPageEvents.GenericDialogCancel));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.GenericDialogAccept:
				GameEventRouter.SendEvent(SettingsPageEvents.ConfirmRename);
				break;
			case SettingsPageEvents.GenericDialogCancel:
				ResetFlow();
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::ShowConfirmRenameDialog : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private IEnumerator ShowChangeFurbyDialog()
		{
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_CHANGEFURBY_WARNING");
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "MENU_BUTTON_CONTINUE");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, SettingsPageEvents.GenericDialogAccept);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "MENU_OPTION_CANCEL");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, SettingsPageEvents.GenericDialogCancel);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept, SettingsPageEvents.GenericDialogCancel));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.GenericDialogAccept:
				FurbyGlobals.Player.SetScannedFlag(false);
				FurbyGlobals.SettingsHelper.RequestChangeFurby(Singleton<GameDataStoreObject>.Instance.Data.FurbyType, false);
				Singleton<GameDataStoreObject>.Instance.Data.FurbyType = AdultFurbyType.Unknown;
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
				GameEventRouter.SendEvent(SettingsPageEvents.ConfirmChangeFurby);
				break;
			case SettingsPageEvents.GenericDialogCancel:
				ResetFlow();
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::ShowChangeFurbyDialog : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private IEnumerator ShowDowngradeDialog()
		{
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DOWNGRADEGAME_WARNING");
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "MENU_BUTTON_CONTINUE");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, SettingsPageEvents.GenericDialogAccept);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "MENU_OPTION_CANCEL");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, SettingsPageEvents.GenericDialogCancel);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept, SettingsPageEvents.GenericDialogCancel));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.GenericDialogAccept:
				Singleton<GameDataStoreObject>.Instance.Data.FurbyType = AdultFurbyType.NoFurby;
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = true;
				Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow = true;
				GameEventRouter.SendEvent(SettingsPageEvents.ConfirmDowngrade);
				break;
			case SettingsPageEvents.GenericDialogCancel:
				ResetFlow();
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::ShowDowngradeDialog : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private IEnumerator ShowUpgradeDialog()
		{
			bool ok = false;
			yield return StartCoroutine(ModeChoiceMediator.ShowToyRequiredDialog(this, m_needFurbyDialog, m_scanningInstructions, m_needFurbyDialogSpawnPoint, delegate(bool b)
			{
				ok = b;
			}));
			if (ok)
			{
				FurbyGlobals.Player.SetScannedFlag(false);
				Singleton<GameDataStoreObject>.Instance.Data.FurbyType = AdultFurbyType.Unknown;
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
				FurbyGlobals.SettingsHelper.RequestChangeFurby(AdultFurbyType.NoFurby, false);
				GameEventRouter.SendEvent(SettingsPageEvents.ConfirmUpgrade);
			}
			else
			{
				ResetFlow();
			}
		}

		private IEnumerator ShowDeleteGameDialog()
		{
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEGAME_WARNING");
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "SAVEGAME_DELETE_YES");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, SettingsPageEvents.GenericDialogAccept);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "SAVEGAME_DELETE_NO");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, SettingsPageEvents.GenericDialogCancel);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept, SettingsPageEvents.GenericDialogCancel));
			SettingsPageEvents returnedEvent = (SettingsPageEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case SettingsPageEvents.GenericDialogAccept:
			{
				FurbyGlobals.HardwareSettingsScreenFlow.SetIsControllingGlobalInGameVolume(true);
				int saveIndex = Singleton<GameDataStoreObject>.Instance.GetCurrentSlotIndex();
				Singleton<GameDataStoreObject>.Instance.Clear(saveIndex);
				GameEventRouter.SendEvent(SettingsPageEvents.ConfirmDelete);
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
				break;
			}
			case SettingsPageEvents.GenericDialogCancel:
				ResetFlow();
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in SettingsScreenFlow::ShowDeleteGameDialog : {0}", returnedEvent.ToString()));
				break;
			}
		}
	}
}
