using System;
using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class RsUniBillInterface : SingletonInstance<RsUniBillInterface>
	{
		public enum PurchaseState
		{
			PurchaseInactive = 0,
			PurchaseInProgress = 1,
			PurchaseCompleted = 2,
			PurchaseCancelled = 3,
			PurchaseFailed = 4
		}

		public delegate void RsUniBillPurchaseCompleteDelegate(string userId, string itemID, string receipt);

		public delegate void RsUniBillPurchaseDelegate_Items(string itemID);

		public delegate void RsUniBillPurchaseDelegate_State(bool state);

		private string m_lastReceipt = string.Empty;

		private bool m_InitializationComplete;

		private UnibillState m_UnibillState = UnibillState.CRITICAL_ERROR;

		public bool InitializationComplete
		{
			get
			{
				return m_InitializationComplete;
			}
		}

		public UnibillState UnibillState
		{
			get
			{
				return m_UnibillState;
			}
		}

		public string UserId
		{
			get
			{
				string result = string.Empty;
				if (!string.IsNullOrEmpty(Unibiller.UserId))
				{
					result = Unibiller.UserId;
				}
				return result;
			}
		}

		public event RsUniBillPurchaseCompleteDelegate OnPurchaseComplete_WithReceipt;

		public event RsUniBillPurchaseCompleteDelegate OnPurchaseComplete_NoReceipt;

		public event RsUniBillPurchaseDelegate_Items OnPurchaseCancelled;

		public event RsUniBillPurchaseDelegate_Items OnPurchaseFailed;

		public event RsUniBillPurchaseDelegate_State OnTransactionsRestored;

		public override void Awake()
		{
			base.Awake();
			Unibiller.onBillerReady += OnBillerReady;
			Unibiller.onTransactionsRestored += OnTransactionsRestoredHandler;
			Unibiller.onPurchaseCancelled += OnPurchaseCancelledHandler;
			Unibiller.onPurchaseFailed += OnPurchaseFailedHandler;
			Unibiller.onPurchaseComplete += OnPurchaseCompleteHandler;
			Unibiller.onPurchaseRefunded += OnPurchaseRefundedHandler;
		}

		public IEnumerator Initialize()
		{
			Logging.Log("RsUniBillInterface:: Initialization started...");
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			Unibiller.Initialise();
			while (!m_InitializationComplete)
			{
				yield return new WaitForEndOfFrame();
			}
			Logging.Log("RsUniBillInterface:: ...Initialization complete.");
		}

		public bool AbleToProcessPurchases()
		{
			DebugUtils.Assert(m_InitializationComplete, "RsUniBillInterface:: UniBill hasn't completed initialisation!");
			DebugUtils.Assert(m_UnibillState == UnibillState.SUCCESS, "RsUniBillInterface:: UniBill didn't Initialise!");
			return m_InitializationComplete && m_UnibillState == UnibillState.SUCCESS;
		}

		public void RestorePurchasedItems()
		{
			if (AbleToProcessPurchases())
			{
				Unibiller.restoreTransactions();
			}
		}

		public void ConductPurchase(string purchaseID)
		{
			if (AbleToProcessPurchases())
			{
				PurchasableItem purchasableItemById = Unibiller.GetPurchasableItemById(purchaseID);
				if (purchasableItemById != null)
				{
					Unibiller.initiatePurchase(purchasableItemById);
				}
			}
		}

		public int GetPurchaseCount(string purchaseID)
		{
			PurchasableItem purchasableItemById = Unibiller.GetPurchasableItemById(purchaseID);
			if (purchasableItemById != null)
			{
				return Unibiller.GetPurchaseCount(purchasableItemById);
			}
			Logging.LogError("Trying to get purchase counts on a bundle that doesn't exist..." + purchaseID);
			return 0;
		}

		public PurchaseType GetPurchaseType(string purchaseID)
		{
			PurchasableItem purchasableItemById = Unibiller.GetPurchasableItemById(purchaseID);
			if (purchasableItemById != null)
			{
				return purchasableItemById.PurchaseType;
			}
			return PurchaseType.Consumable;
		}

		public bool HasItemBeenPurchased(string purchaseID)
		{
			return GetPurchaseCount(purchaseID) > 0;
		}

		public bool DoesItemExist(string purchaseID)
		{
			PurchasableItem purchasableItemById = Unibiller.GetPurchasableItemById(purchaseID);
			if (purchasableItemById != null)
			{
				return true;
			}
			return false;
		}

		public string GetLocalizedPriceString(string purchaseID)
		{
			PurchasableItem purchasableItemById = Unibiller.GetPurchasableItemById(purchaseID);
			if (purchasableItemById != null)
			{
				string localizedPriceString = purchasableItemById.localizedPriceString;
				if (localizedPriceString == null)
				{
					return string.Empty;
				}
				return localizedPriceString;
			}
			return string.Empty;
		}

		private void OnBillerReady(UnibillState state)
		{
			Logging.Log("RsUniBillInterface::onBillerReady:" + state);
			m_UnibillState = state;
			m_InitializationComplete = true;
		}

		private void OnPurchaseCompleteHandler(PurchasableItem item)
		{
			Logging.Log("RsUniBillInterface::Purchase OK: " + item.Id);
			Logging.Log(string.Format("{0} has now been purchased {1} times.", item.name, Unibiller.GetPurchaseCount(item)));
			try
			{
				string[] allPurchaseReceipts = Unibiller.GetAllPurchaseReceipts(item);
				if (allPurchaseReceipts == null || allPurchaseReceipts.Length == 0)
				{
					Logging.LogWarning("RsUniBillInterface::Purchase complete: " + item.Id + " BUT NO RECEIPT");
					if (this.OnPurchaseComplete_NoReceipt != null)
					{
						this.OnPurchaseComplete_NoReceipt(string.Empty, item.Id, string.Empty);
					}
					return;
				}
				string receipt = allPurchaseReceipts[allPurchaseReceipts.Length - 1];
				if (this.OnPurchaseComplete_WithReceipt != null)
				{
					string userId = string.Empty;
					if (!string.IsNullOrEmpty(Unibiller.UserId))
					{
						userId = Unibiller.UserId;
					}
					this.OnPurchaseComplete_WithReceipt(userId, item.Id, receipt);
				}
			}
			catch (Exception ex)
			{
				Logging.LogError("RsUniBillInterface : OnPurchaseCompleteHandler Error: " + ex.ToString());
			}
		}

		private void OnPurchaseCancelledHandler(PurchasableItem item)
		{
			Logging.LogWarning("RsUniBillInterface::Purchase cancelled: " + item.Id);
			if (this.OnPurchaseCancelled != null)
			{
				this.OnPurchaseCancelled(item.Id);
			}
		}

		private void OnPurchaseFailedHandler(PurchasableItem item)
		{
			Logging.LogWarning("RsUniBillInterface::Purchase failed: " + item.Id);
			if (this.OnPurchaseFailed != null)
			{
				this.OnPurchaseFailed(item.Id);
			}
		}

		private void OnPurchaseRefundedHandler(PurchasableItem item)
		{
			Logging.Log("RsUniBillInterface::Purchase Refunded: " + item.Id);
		}

		public void OnTransactionsRestoredHandler(bool successState)
		{
			Logging.Log("RsUniBillInterface::OnTransactionsRestoredHandler Transactions restored.");
			if (this.OnTransactionsRestored != null)
			{
				Logging.Log("RsUniBillInterface::OnTransactionsRestoredHandler Calling callback");
				this.OnTransactionsRestored(successState);
			}
		}
	}
}
