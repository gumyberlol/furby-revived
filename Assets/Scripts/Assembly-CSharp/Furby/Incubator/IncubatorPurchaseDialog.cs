using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorPurchaseDialog : IncubatorConfirmDialog
	{
		public enum ConsumableTelemetry
		{
			ConsumableTakenAndSucceeded = 0,
			ConsumableTakenButFailed = 1,
			ConsumableNotInterestedDeclined = 2,
			ConsumableInterestedButDeclined = 3
		}

		[SerializeField]
		private IncubatorLogic m_GameLogic;

		[SerializeField]
		private IncubatorConfirmDialog m_FailureDialog;

		[SerializeField]
		private IncubatorConfirmDialog m_SuccessDialog;

		[SerializeField]
		private GameObject m_BuyButtonPrefab;

		[SerializeField]
		private GameObject m_ProgressSpinner;

		[SerializeField]
		private GameObject m_ButtonGroup;

		[SerializeField]
		private UIGrid m_ButtonGrid;

		[SerializeField]
		private int m_MaxNumOfReceiptValidationAttempts = 2;

		[SerializeField]
		public RsStoreMediator.StoreResponse m_ResponseAfterFailingToValidate = RsStoreMediator.StoreResponse.Failed;

		private void PopulatePurchaseGrid()
		{
			if (m_ButtonGrid.transform.childCount == 0)
			{
				foreach (GameConsumable fastForwardConsumable in m_GameLogic.FastForwardConsumables)
				{
					if (SingletonInstance<RsStoreMediator>.Instance.IsItemAvailable(fastForwardConsumable.StoreID))
					{
						GameObject gameObject = Object.Instantiate(m_BuyButtonPrefab) as GameObject;
						IncubatorDialogButton component = gameObject.GetComponent<IncubatorDialogButton>();
						gameObject.transform.parent = m_ButtonGrid.transform;
						gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localRotation = Quaternion.identity;
						GameObject childGameObject = gameObject.GetChildGameObject("ValueText");
						UILabel component2 = childGameObject.GetComponent<UILabel>();
						string text = Singleton<Localisation>.Instance.GetText("INCUBATOR_FF_BUYBUTTON");
						string text2 = string.Format(text, fastForwardConsumable.ContentUnits);
						component2.text = text2;
						GameObject childGameObject2 = gameObject.GetChildGameObject("CostText");
						UILabel component3 = childGameObject2.GetComponent<UILabel>();
						component3.text = SingletonInstance<InAppPurchaseServer>.Instance.GetLocalizedPrice_FromProvider(fastForwardConsumable.StoreID);
						component.ParentPanel = this;
						component.ObjectModel = fastForwardConsumable;
					}
				}
			}
			m_ButtonGrid.Reposition();
		}

		public override IEnumerator ShowModal()
		{
			PopulatePurchaseGrid();
			m_DialogPanel.SetActive(true);
			yield return StartCoroutine(base.AwaitInput());
			if (m_EventSender.ObjectModel != null)
			{
				GameConsumable purchasedConsumable = m_EventSender.ObjectModel as GameConsumable;
				RsStoreMediator.StoreResponse? response = null;
				RsStoreMediator.RsStoreMediatorValidationDelegate_ForItems purchaseFailed = delegate
				{
					response = RsStoreMediator.StoreResponse.Failed;
				};
				RsStoreMediator.RsStoreMediatorValidationDelegate_ForItems purchaseCancelled = delegate
				{
					response = RsStoreMediator.StoreResponse.Cancelled;
				};
				RsStoreMediator.RsStoreMediatorValidationDelegate_ForItems purchaseComplete = delegate
				{
					response = RsStoreMediator.StoreResponse.Succeeded;
				};
				RsStoreMediator.RsStoreMediatorValidationDelegate_ForItems purchaseCouldntValidate = delegate
				{
					response = RsStoreMediator.StoreResponse.CouldntValidate;
				};
				GameConfigBlob gameConfigBlob = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob();
				bool requireValidation = gameConfigBlob.m_RequireValidatedReceipts;
				RsStoreMediator billingMediator = SingletonInstance<RsStoreMediator>.Instance;
				billingMediator.OnPurchaseFailed += purchaseFailed;
				billingMediator.OnPurchaseCancelled += purchaseCancelled;
				billingMediator.OnPurchaseComplete_ReceiptValidationSucceeded += purchaseComplete;
				billingMediator.OnPurchaseComplete_ReceiptValidationFailed += purchaseFailed;
				billingMediator.OnPurchaseComplete_ReceiptValidationUnresolved += purchaseCouldntValidate;
				string productID = purchasedConsumable.StoreID;
				billingMediator.AttemptToPurchaseItem(productID);
				m_ButtonGroup.SetActive(false);
				m_ProgressSpinner.SetActive(true);
				while (!response.HasValue)
				{
					yield return null;
				}
				if (response == RsStoreMediator.StoreResponse.CouldntValidate)
				{
					if (!requireValidation)
					{
						response = RsStoreMediator.StoreResponse.Succeeded;
					}
					else
					{
						GameData saveGameData = Singleton<GameDataStoreObject>.Instance.Data;
						int numOfValidationAttempts = 0;
						if (saveGameData.HaveUnresolvedReceipt(productID))
						{
							numOfValidationAttempts = saveGameData.GetUnresolvedReceipt(productID).m_ValidationCount;
						}
						for (int nthValidationAttempt = numOfValidationAttempts; nthValidationAttempt < m_MaxNumOfReceiptValidationAttempts; nthValidationAttempt++)
						{
							response = null;
							billingMediator.AttemptToReValidatePurchase(productID);
							while (!response.HasValue)
							{
								yield return null;
							}
							if (response != RsStoreMediator.StoreResponse.CouldntValidate)
							{
								break;
							}
						}
						if (response.HasValue)
						{
							switch (response.Value)
							{
							case RsStoreMediator.StoreResponse.CouldntValidate:
								GameEventRouter.SendEvent(IAPReceiptValidationEvents.ReceiptValidationUnresolved, null, productID);
								break;
							case RsStoreMediator.StoreResponse.Failed:
								GameEventRouter.SendEvent(IAPReceiptValidationEvents.ReceiptValidationFailed, null, productID);
								break;
							case RsStoreMediator.StoreResponse.Succeeded:
								GameEventRouter.SendEvent(IAPReceiptValidationEvents.ReceiptValidationSuccess, null, productID);
								break;
							}
						}
						if (response == RsStoreMediator.StoreResponse.CouldntValidate)
						{
							response = m_ResponseAfterFailingToValidate;
						}
					}
				}
				m_ButtonGroup.SetActive(true);
				m_ProgressSpinner.SetActive(false);
				billingMediator.OnPurchaseFailed -= purchaseFailed;
				billingMediator.OnPurchaseCancelled -= purchaseCancelled;
				billingMediator.OnPurchaseComplete_ReceiptValidationSucceeded -= purchaseComplete;
				billingMediator.OnPurchaseComplete_ReceiptValidationFailed -= purchaseCouldntValidate;
				billingMediator.OnPurchaseComplete_ReceiptValidationUnresolved -= purchaseCouldntValidate;
				if (response.HasValue)
				{
					switch (response.Value)
					{
					case RsStoreMediator.StoreResponse.Failed:
						GameEventRouter.SendEvent(ConsumableTelemetry.ConsumableTakenButFailed, null, productID);
						m_DialogPanel.SetActive(false);
						yield return StartCoroutine(m_FailureDialog.ShowModal());
						GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseFailed, null, productID);
						break;
					case RsStoreMediator.StoreResponse.Succeeded:
						Singleton<GameDataStoreObject>.Instance.Data.IncrementSuccessfulIAPBundlePurchase(productID);
						Singleton<GameDataStoreObject>.Instance.Save();
						GameEventRouter.SendEvent(IncubatorGameEvent.Incubator_FastForward_Activated);
						GameEventRouter.SendEvent(ConsumableTelemetry.ConsumableTakenAndSucceeded, null, productID);
						GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseComplete, null, productID);
						m_DialogPanel.SetActive(false);
						m_GameLogic.OnPurchaseFastForward(purchasedConsumable.ContentUnits);
						m_GameLogic.OnFastForwardActivate();
						break;
					case RsStoreMediator.StoreResponse.Cancelled:
						GameEventRouter.SendEvent(ConsumableTelemetry.ConsumableInterestedButDeclined, null, productID);
						GameEventRouter.SendEvent(IAPPurchaseEvents.PurchaseCancelled, null, productID);
						m_DialogPanel.SetActive(false);
						break;
					}
				}
			}
			else
			{
				GameEventRouter.SendEvent(ConsumableTelemetry.ConsumableNotInterestedDeclined, null, null);
			}
			m_DialogPanel.SetActive(false);
		}
	}
}
