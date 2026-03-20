using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class PlayOrDeleteLogic : RelentlessMonoBehaviour
	{
		[SerializeField]
		private CodeControlledDialogBox m_DialogBox;

		[SerializeField]
		private GameObject m_BackButton;

		private FurbyBaby m_CurrentSelection;

		private void ResetFlow()
		{
			StopAllCoroutines();
			ResetState();
			StartCoroutine(StartPlayOrDeleteLogic());
		}

		private void ResetState()
		{
			m_DialogBox.Hide();
			m_CurrentSelection = null;
			ShowBackButton();
		}

		private void ShowBackButton()
		{
			m_BackButton.SetActive(true);
		}

		private void HideBackButton()
		{
			m_BackButton.SetActive(false);
		}

		private void Start()
		{
			m_DialogBox.Hide();
			m_CurrentSelection = null;
			StartCoroutine(StartPlayOrDeleteLogic());
		}

		private IEnumerator StartPlayOrDeleteLogic()
		{
			if (FurbyGlobals.SettingsHelper.IsDeleteBabyRequested())
			{
				yield return StartCoroutine(StartBabyDeleting());
				ResetFlow();
			}
			else if (FurbyGlobals.SettingsHelper.WasBabyDeleted())
			{
				yield return StartCoroutine(StartDeleteConfirmDialog());
				ResetFlow();
			}
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(HoodEvents.Hood_BabySelected));
			HoodEvents returnedEvent = (HoodEvents)(object)waiter.ReturnedEvent;
			if (returnedEvent == HoodEvents.Hood_BabySelected)
			{
				FurbyGlobals.Player.SelectedFurbyBaby = (FurbyBaby)waiter.ReturnedParameters[0];
				GameEventRouter.SendEvent(HoodEvents.Hood_BabyPlayroomConfirmed);
			}
			else
			{
				Logging.LogError(string.Format("Unexpected event in PlayOrDeleteLogic::StartPlayOrDeleteLogic : {0}", returnedEvent.ToString()));
			}
			ResetFlow();
		}

		private IEnumerator StartBabyDeleting()
		{
			HideBackButton();
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEBABY_CHOOSEBABY");
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.SmallMessageOneButton);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_CANCEL");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, HoodEvents.Hood_GenericDecline);
			m_DialogBox.Show(false);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(HoodEvents.Hood_BabySelected, HoodEvents.Hood_GenericDecline));
			m_DialogBox.Hide();
			HoodEvents returnedEvent = (HoodEvents)(object)waiter.ReturnedEvent;
			switch (returnedEvent)
			{
			case HoodEvents.Hood_BabySelected:
				m_CurrentSelection = (FurbyBaby)waiter.ReturnedParameters[0];
				yield return StartCoroutine(StartDeleteWarningDialog());
				break;
			case HoodEvents.Hood_GenericDecline:
				FurbyGlobals.SettingsHelper.ClearDeleteBabyRequest();
				GameEventRouter.SendEvent(HoodEvents.Hood_BabyDeleteCompleted);
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in PlayOrDeleteLogic::StartBabyDeleting : {0}", returnedEvent.ToString()));
				break;
			}
		}

		private IEnumerator StartDeleteWarningDialog()
		{
			m_DialogBox.SetLocalisedTextWithSubstitutedBabyName(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEBABY_WARNING", m_CurrentSelection.Name);
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "SAVEGAME_DELETE_YES");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, HoodEvents.Hood_GenericAccept);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "SAVEGAME_DELETE_NO");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, HoodEvents.Hood_GenericDecline);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(HoodEvents.Hood_GenericAccept, HoodEvents.Hood_GenericDecline));
				m_DialogBox.Hide();
				HoodEvents returnedEvent = (HoodEvents)(object)waiter.ReturnedEvent;
				switch (returnedEvent)
				{
				case HoodEvents.Hood_GenericAccept:
					GameEventRouter.SendEvent(HoodEvents.Hood_BabyDeleteConfirmed);
					Singleton<GameDataStoreObject>.Instance.Data.RemoveFurbyBaby(m_CurrentSelection);
					FurbyGlobals.SettingsHelper.ClearDeleteBabyRequest();
					FurbyGlobals.SettingsHelper.SetBabyWasDeleted(m_CurrentSelection.Name);
					GameEventRouter.SendEvent(HoodEvents.Hood_BabyDeleteReload);
					break;
				case HoodEvents.Hood_GenericDecline:
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in PlayOrDeleteLogic::StartDeleteWarningDialog : {0}", returnedEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator StartDeleteConfirmDialog()
		{
			m_DialogBox.SetLocalisedTextWithSubstitutedBabyName(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEBABY_CONFIRMED", FurbyGlobals.SettingsHelper.GetDeletedBabyName());
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_OK");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, HoodEvents.Hood_GenericAccept);
			m_DialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(HoodEvents.Hood_GenericAccept));
				m_DialogBox.Hide();
				HoodEvents returnedEvent = (HoodEvents)(object)waiter.ReturnedEvent;
				if (returnedEvent == HoodEvents.Hood_GenericAccept)
				{
					FurbyGlobals.SettingsHelper.ClearBabyWasDeleted();
					GameEventRouter.SendEvent(HoodEvents.Hood_BabyDeleteCompleted);
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in PlayOrDeleteLogic::StartDeleteConfirmDialog : {0}", returnedEvent.ToString()));
				}
			}
		}
	}
}
