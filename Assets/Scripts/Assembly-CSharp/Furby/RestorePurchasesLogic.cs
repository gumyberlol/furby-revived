using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class RestorePurchasesLogic : MonoBehaviour
	{
		[SerializeField]
		public GameObject m_Button;

		[NamedText]
		[SerializeField]
		private string m_RestoreConfirmationText;

		[NamedText]
		[SerializeField]
		private string m_RestoreInProgressText;

		[NamedText]
		[SerializeField]
		private string m_RestoreSucceededText;

		[SerializeField]
		[NamedText]
		private string m_RestoreFailedText;

		[SerializeField]
		public string m_ItemName;

		private GameEventSubscription m_EventSubscription;

		private static bool m_VerboseDebugging;

		[SerializeField]
		private LayerMask m_layersToDisable;

		private UnityEngine.Object[] m_uiCameraList;

		public GameObject m_NoNetworkDialog;

		private void Activate(GameObject go)
		{
			if (go != null)
			{
				go.SetActive(true);
			}
		}

		private void Deactivate(GameObject go)
		{
			if (go != null)
			{
				go.SetActive(false);
			}
		}

		private void Awake()
		{
			StartCoroutine(Initialize());
		}

		private IEnumerator Initialize()
		{
			m_EventSubscription = new GameEventSubscription(typeof(RestorePurchasesEvent), OnInvocationEvent);
			GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
			while (SingletonInstance<RsStoreMediator>.Instance == null)
			{
				yield return new WaitForEndOfFrame();
			}
			while (!SingletonInstance<RsStoreMediator>.Instance.AmAbleToMediatePurchases())
			{
				yield return new WaitForEndOfFrame();
			}
			string targetBundleID = "FurbyIAPBundle4";
			if (SingletonInstance<RsStoreMediator>.Instance.HasItemBeenPurchased(targetBundleID) && m_Button != null)
			{
				m_Button.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			m_EventSubscription.Dispose();
		}

		private void OnInvocationEvent(Enum eventType, GameObject originator, params object[] parameters)
		{
			switch ((RestorePurchasesEvent)(object)eventType)
			{
			case RestorePurchasesEvent.ShowButton:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("OnInvocationEvent: ShowButton");
				}
				break;
			case RestorePurchasesEvent.InvokeRestorePurchasesFlow:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("OnInvocationEvent: ActivateDialog");
				}
				GameEventRouter.Instance.StartCoroutine(InitiateSequence());
				break;
			case RestorePurchasesEvent.HideButton:
				break;
			}
		}

		private void DisableInputLayers()
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.DisableInputLayers");
			}
			m_uiCameraList = UnityEngine.Object.FindObjectsOfType(typeof(UICamera));
			UnityEngine.Object[] uiCameraList = m_uiCameraList;
			for (int i = 0; i < uiCameraList.Length; i++)
			{
				UICamera uICamera = (UICamera)uiCameraList[i];
				if (((int)m_layersToDisable & (1 << uICamera.gameObject.layer)) != 0 && uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = false;
				}
			}
		}

		private void ReenableInputLayers()
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ReenableInputLayers");
			}
			UnityEngine.Object[] uiCameraList = m_uiCameraList;
			for (int i = 0; i < uiCameraList.Length; i++)
			{
				UICamera uICamera = (UICamera)uiCameraList[i];
				if (((int)m_layersToDisable & (1 << uICamera.gameObject.layer)) != 0 && uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = true;
				}
			}
		}

		private IEnumerator InitiateSequence()
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.InitiateSequence()");
			}
			bool haveNetwork = SetupNetworking.IsInternetReady;
			bool billingSystemAvailable = SingletonInstance<RsStoreMediator>.Instance.IsBillingSystemIsAvailable();
			bool nonConsumablesAvailable = SingletonInstance<RsStoreMediator>.Instance.IsCrystalSkinOnTheStore();
			DisableInputLayers();
			if (haveNetwork && billingSystemAvailable && nonConsumablesAvailable)
			{
				yield return StartCoroutine(ConductRestorePurchaseFlow());
			}
			else
			{
				m_NoNetworkDialog.SetActive(true);
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
				m_NoNetworkDialog.SetActive(false);
			}
			ReenableInputLayers();
		}

		private IEnumerator ConductRestorePurchaseFlow()
		{
			GameConfiguration gameConfig = SingletonInstance<GameConfiguration>.Instance;
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: IAP Available, checking billing");
			}
			string bundleId = gameConfig.GetGameConfigBlob().m_CrystalLookBundleID;
			GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ConfirmMode);
			GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, m_RestoreConfirmationText);
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Waiting for ok/exit...");
			}
			WaitForGameEvent areYouSure_Waiter = new WaitForGameEvent();
			yield return StartCoroutine(areYouSure_Waiter.WaitForEvent(InAppInProgressEvent.OKButtonPressed, InAppInProgressEvent.ExitButtonPressed));
			switch ((InAppInProgressEvent)(object)areYouSure_Waiter.ReturnedEvent)
			{
			case InAppInProgressEvent.OKButtonPressed:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Got OK, next!");
				}
				break;
			case InAppInProgressEvent.ExitButtonPressed:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Got EXIT, bombing out!");
				}
				ReenableInputLayers();
				GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
				yield break;
			}
			yield return null;
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Showing progress");
			}
			GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ProgressMode);
			GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, m_RestoreInProgressText);
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Starting flow ...");
			}
			RsStoreMediator storeMediator = SingletonInstance<RsStoreMediator>.Instance;
			storeMediator.TransactionRestored = RsStoreMediator.TransactionsRestoredState.InProgress;
			yield return StartCoroutine(storeMediator.ConductRestorePurchasesFlow());
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow: Completed flow! [State:" + storeMediator.TransactionRestored.ToString() + "]");
			}
			switch (storeMediator.TransactionRestored)
			{
			case RsStoreMediator.TransactionsRestoredState.InProgress:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow(InProgress):Still going on...");
				}
				break;
			case RsStoreMediator.TransactionsRestoredState.SuccessfullyRestored:
				yield return StartCoroutine(ConductRestorePurchaseFlow_RestoreSuccess(storeMediator, bundleId));
				break;
			case RsStoreMediator.TransactionsRestoredState.NotSet:
				yield return StartCoroutine(ConductRestorePurchaseFlow_AbortCancel());
				break;
			case RsStoreMediator.TransactionsRestoredState.RestoreFailed:
				yield return StartCoroutine(ConductRestorePurchaseFlow_RestoreFailed());
				break;
			}
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow COMPLETE");
			}
		}

		private IEnumerator ConductRestorePurchaseFlow_RestoreFailed()
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed(NotSet): Restore failed");
			}
			GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, m_RestoreFailedText);
			GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ModalityMode);
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed: Wait for " + SettingsPageEvents.GenericDialogAccept);
			}
			WaitForGameEvent failureConfirmation = new WaitForGameEvent();
			yield return StartCoroutine(failureConfirmation.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed: GOT! " + SettingsPageEvents.GenericDialogAccept);
			}
			GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed: Restore failed complete.");
			}
		}

		private IEnumerator ConductRestorePurchaseFlow_AbortCancel()
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed(NotSet): No transaction restore state change");
			}
			GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreFailed(NotSet): Reloading scene!");
			}
			ReenableInputLayers();
			Application.LoadLevel(Application.loadedLevelName);
			yield return null;
		}

		private IEnumerator ConductRestorePurchaseFlow_RestoreSuccess(RsStoreMediator storeMediator, string bundleId)
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreSuccess(SuccessfullyRestored): Resolving receipts... (if needed)");
			}
			yield return StartCoroutine(storeMediator.ResolveResponseViaReceiptValidation(bundleId));
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreSuccess(SuccessfullyRestored): Interpreting results...");
			}
			if (storeMediator.LastStoreResponse.HasValue)
			{
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreSuccess(SuccessfullyRestored): ProcessStoreResponse...");
				}
				yield return StartCoroutine(ProcessStoreResponse(storeMediator, bundleId));
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreSuccess(SuccessfullyRestored): Reloading scene!");
				}
				Application.LoadLevel(Application.loadedLevelName);
				ReenableInputLayers();
			}
			else
			{
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic.ConductRestorePurchaseFlow_RestoreSuccess(SuccessfullyRestored): No StoreResponse to process...");
				}
				yield return StartCoroutine(ConductRestorePurchaseFlow_RestoreFailed());
			}
		}

		private IEnumerator ProcessStoreResponse(RsStoreMediator storeMediator, string bundleId)
		{
			if (m_VerboseDebugging)
			{
				DebugUtils.Log_InCyan("ProcessStoreResponse: On:" + bundleId);
			}
			RsStoreMediator.StoreResponse? lastStoreResponse = storeMediator.LastStoreResponse;
			if (!lastStoreResponse.HasValue)
			{
				yield break;
			}
			switch (lastStoreResponse.Value)
			{
			case RsStoreMediator.StoreResponse.Cancelled:
			case RsStoreMediator.StoreResponse.Failed:
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic: Fail/Cancel");
				}
				GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, m_RestoreFailedText);
				GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ModalityMode);
				GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
				break;
			case RsStoreMediator.StoreResponse.Succeeded:
			{
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic: Successfully restored an earlier transaction");
				}
				string translatedItem = Singleton<Localisation>.Instance.GetText(m_ItemName);
				string translatedBase = Singleton<Localisation>.Instance.GetText(m_RestoreSucceededText);
				string substitutedText = string.Format(translatedBase, translatedItem);
				GameEventRouter.SendEvent(InAppInProgressEvent.UpdateContent, base.gameObject, substitutedText);
				GameEventRouter.SendEvent(InAppInProgressEvent.ShowInProgressDialog_ModalityMode);
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic: Waiting for OK button");
				}
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForEvent(InAppInProgressEvent.OKButtonPressed));
				if (m_VerboseDebugging)
				{
					DebugUtils.Log_InCyan("RestorePurchasesLogic: Got OK, applying change");
				}
				Singleton<GameDataStoreObject>.Instance.Data.IncrementSuccessfulIAPBundlePurchase(bundleId);
				Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
				Singleton<GameDataStoreObject>.Instance.Save();
				GameEventRouter.SendEvent(InAppInProgressEvent.HideInProgressDialog);
				break;
			}
			}
		}
	}
}
