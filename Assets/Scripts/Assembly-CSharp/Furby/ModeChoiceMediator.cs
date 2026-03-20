using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ModeChoiceMediator : MonoBehaviour
	{
		public delegate void ToyRequiredOutcome(bool haveToy);

		[SerializeField]
		private CodeControlledDialogBox m_DialogBox;

		[SerializeField]
		private UILabel m_DialogBoxText;

		[SerializeField]
		private float m_DialogBoxTextScale = 0.5f;

		[SerializeField]
		private Transform m_dialogSpawnPoint;

		[SerializeField]
		private UIPanel m_needFurbyDialog;

		[SerializeField]
		private UIPanel m_scanningInstructions;

		private void Start()
		{
			DialogBoxTextSanityCheck();
			ResetFlow();
		}

		private void DialogBoxTextSanityCheck()
		{
			if (m_DialogBoxText != null)
			{
				bool flag = false;
				Transform[] componentsInChildren = m_DialogBox.GetComponentsInChildren<Transform>(true);
				foreach (Transform transform in componentsInChildren)
				{
					flag |= transform == m_DialogBoxText.transform;
				}
				if (!flag)
				{
					throw new ApplicationException("Dialog box text is not a child of dialog box");
				}
			}
		}

		private void ResetFlow()
		{
			StopAllCoroutines();
			StartCoroutine(StartModeChoiceFlow());
		}

		private IEnumerator StartModeChoiceFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(FurbyModeChoice.PlayWithFurby, FurbyModeChoice.PlayWithNoFurby, FurbyModeChoice.BackButtonClicked, FurbyModeChoice.SettingsButtonClicked));
			FurbyModeChoice returnedEvent = (FurbyModeChoice)(object)waiter.ReturnedEvent;
			Logging.Log(string.Format("Got \"{0}\" from waiter.", returnedEvent.ToString()));
			switch (returnedEvent)
			{
			case FurbyModeChoice.PlayWithFurby:
				yield return StartCoroutine(HandlePlayWithFurby());
				break;
			case FurbyModeChoice.PlayWithNoFurby:
				yield return StartCoroutine(HandlePlayWithNoFurby());
				break;
			case FurbyModeChoice.BackButtonClicked:
				HandleBackButtonClicked();
				break;
			case FurbyModeChoice.SettingsButtonClicked:
				HandleSettingsButtonClicked();
				break;
			}
		}

		private bool IsUpgradableGame()
		{
			return FurbyGlobals.SettingsHelper.IsUpgradableGame();
		}

		private IEnumerator HandlePlayWithFurby()
		{
			if (TitlePageMicrophoneChecker.IsMicrophoneDisabled())
			{
				yield return StartCoroutine(ShowMicLockBlocker());
				ResetFlow();
				yield break;
			}
			bool haveToy = false;
			yield return StartCoroutine(ShowToyRequiredDialog(this, m_needFurbyDialog, m_scanningInstructions, m_dialogSpawnPoint, delegate(bool b)
			{
				haveToy = b;
			}));
			if (!haveToy)
			{
				ResetFlow();
				yield break;
			}
			if (IsUpgradableGame())
			{
				FurbyGlobals.Player.SetScannedFlag(false);
				Singleton<GameDataStoreObject>.Instance.Data.FurbyType = AdultFurbyType.Unknown;
				Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
				FurbyGlobals.SettingsHelper.RequestChangeFurby(AdultFurbyType.NoFurby, true);
			}
			GameEventRouter.SendEvent(FurbyModeChoice.GoToScanningScreen);
		}

		private IEnumerator ShowMicLock(string contentKey)
		{
			m_DialogBox.gameObject.SetActive(true);
			m_DialogBox.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, contentKey);
			m_DialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_BUTTON_ACCEPT");
			m_DialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, SettingsPageEvents.GenericDialogAccept);
			bool disableCamera = true;
			m_DialogBox.Show(disableCamera);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.GenericDialogAccept));
			m_DialogBox.Hide();
			m_DialogBox.gameObject.SetActive(false);
		}

		private IEnumerator ShowMicLockBlocker()
		{
			Vector3 prevScale = m_DialogBoxText.transform.localScale;
			m_DialogBoxText.transform.localScale *= m_DialogBoxTextScale;
			yield return StartCoroutine(ShowMicLock("ERROR_MICACCESS_FURBYBLOCK"));
			m_DialogBoxText.transform.localScale = prevScale;
		}

		private IEnumerator ShowMicLockWarning()
		{
			yield return StartCoroutine(ShowMicLock("ERROR_MICACCESS_WARNING"));
		}

		public static IEnumerator ShowToyRequiredDialog(MonoBehaviour self, UIPanel needFurbyDialog, UIPanel scanningInstructionsDialog, Transform spawnPoint, ToyRequiredOutcome outcome)
		{
			UIPanel dialog = UnityEngine.Object.Instantiate(needFurbyDialog) as UIPanel;
			dialog.transform.parent = spawnPoint;
			dialog.transform.localScale = Vector3.one;
			dialog.transform.localPosition = Vector3.zero;
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return self.StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
			UnityEngine.Object.Destroy(dialog.gameObject);
			yield return null;
			bool haveToy = waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogAccept);
			if (haveToy)
			{
				dialog = UnityEngine.Object.Instantiate(scanningInstructionsDialog) as UIPanel;
				dialog.transform.parent = spawnPoint;
				dialog.transform.localScale = Vector3.one;
				dialog.transform.localPosition = Vector3.zero;
				yield return self.StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
				UnityEngine.Object.Destroy(dialog.gameObject);
			}
			if (outcome != null)
			{
				outcome(haveToy);
			}
		}

		private IEnumerator HandlePlayWithNoFurby()
		{
			if (TitlePageMicrophoneChecker.IsMicrophoneDisabled())
			{
				yield return StartCoroutine(ShowMicLockWarning());
			}
			if (!IsUpgradableGame())
			{
				AdultFurbyType[] validTypes = new AdultFurbyType[10]
				{
					AdultFurbyType.Checkerboard,
					AdultFurbyType.Cubes,
					AdultFurbyType.Diagonals,
					AdultFurbyType.Hearts,
					AdultFurbyType.Lightning,
					AdultFurbyType.Peacock,
					AdultFurbyType.Stripes,
					AdultFurbyType.Triangles,
					AdultFurbyType.Waves,
					AdultFurbyType.Zigzags
				};
				Singleton<GameDataStoreObject>.Instance.Data.FurbyType = AdultFurbyType.NoFurby;
				Singleton<GameDataStoreObject>.Instance.Data.NoFurbyUnlockType = validTypes[UnityEngine.Random.Range(0, validTypes.Length)];
				Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow = true;
			}
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = true;
			GameEventRouter.SendEvent(FurbyModeChoice.GoToDashboard);
		}

		private bool IsFirstSaveGame()
		{
			bool flag = false;
			foreach (GameData item in Singleton<GameDataStoreObject>.Instance)
			{
				flag |= item.IsActiveGame;
			}
			return !flag;
		}

		private void HandleBackButtonClicked()
		{
			if (IsFirstSaveGame())
			{
				GameEventRouter.SendEvent(FurbyModeChoice.GoToTitlePage);
			}
			else
			{
				GameEventRouter.SendEvent(FurbyModeChoice.GoToSaveSlotSelect);
			}
		}

		private void HandleSettingsButtonClicked()
		{
			GameEventRouter.SendEvent(FurbyModeChoice.GoToGlobalSettings);
		}
	}
}
