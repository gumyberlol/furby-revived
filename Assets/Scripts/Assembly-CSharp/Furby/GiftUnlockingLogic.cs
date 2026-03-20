using System.Collections;
using System.Collections.Generic;
using Furby.Utilities;
using Relentless;
using UnityEngine;

namespace Furby
{
	[RequireComponent(typeof(GiftUnlockingHints))]
	public class GiftUnlockingLogic : MonoBehaviour
	{
		public UIAtlas m_GiftUnlockingAtlas;

		[EasyEditArray]
		public GiftSprite[] m_ListOfGiftSpriteNames;

		[HideInInspector]
		private List<GiftSprite> m_AllocatedGiftSprites;

		public UISprite m_UnlockSpriteTarget;

		public GameObject m_BurstScreenRoot;

		public GameObject m_GiftLocatorRoot;

		public GameObject m_GiftButtonPrefab;

		public GiftPresentation m_GiftPresentation;

		public GameObject m_BounceItemGameObject;

		public GameObject m_SpotlightRoot;

		public GameObject m_BackButton;

		public GameObject m_ContinueButton;

		public GameObject m_CancelButton;

		public GameObject m_PayToOpenGiftPopupDialog;

		public GameObject m_CantAffordToOpenGiftDialog;

		public GameObject m_GenericDialogBlankingPanel;

		public GameObject m_FurbucksWalletDisplay;

		public GameObject m_ConfirmFurbuckSpendDialog;

		public ErrorMessageBox m_ComAirErrorDialog;

		public GameObject m_ShelfBackground;

		public GameObject m_TransferBackground;

		public GameObject m_BurstBackground;

		public GameObject m_SeasonalBackgrounds;

		private List<GameObject> m_SpotlightReferences;

		private GameObject[] m_InstancedButtons;

		private List<GameObject> m_GiftButtons;

		private WaitForGameEvent m_Waiter = new WaitForGameEvent();

		private Vector3 m_Starting_Scale;

		public float m_TweeningDurationSecs = 0.75f;

		public float m_TransferScreenUpscale = 2f;

		public float m_BurstScreenSpriteUpscale = 2f;

		public float m_GiftSpriteUpscaleWrappedGift = 0.7f;

		public float m_GiftSpriteUpscaleUnwrappedGift = 1.25f;

		public float m_TransferTimeoutSecs = 15f;

		public int m_FurbucksCostToOpenTheGift = 9999;

		private GiftSprite GetGiftSpriteForIndex(int index)
		{
			return m_AllocatedGiftSprites[index];
		}

		public void OnDestroy()
		{
			if (m_InstancedButtons != null)
			{
				GameObject[] instancedButtons = m_InstancedButtons;
				foreach (GameObject obj in instancedButtons)
				{
					Object.Destroy(obj);
				}
			}
		}

		public IEnumerator Start()
		{
			yield return null;
			InitializeScene();
			DeActivateSceneObject(m_ContinueButton);
			DeActivateSceneObject(m_BurstScreenRoot);
			DeActivateSceneObject(m_CancelButton);
			yield return StartCoroutine(GiftSelectionLoop());
		}

		private void InitializeScene()
		{
			ValidateSceneReferences();
			m_InstancedButtons = new GameObject[FurbyGlobals.GiftList.m_GiftItemData.Count];
			m_SpotlightReferences = new List<GameObject>();
			foreach (Transform item in m_SpotlightRoot.transform)
			{
				m_SpotlightReferences.Add(item.gameObject);
			}
			m_SpotlightReferences.Sort((GameObject x, GameObject y) => string.Compare(x.name, y.name));
			m_GiftButtons = new List<GameObject>();
			foreach (Transform item2 in m_GiftLocatorRoot.transform)
			{
				m_GiftButtons.Add(item2.gameObject);
			}
			m_GiftButtons.Sort((GameObject x, GameObject y) => string.Compare(x.name, y.name));
			for (int num = 0; num < FurbyGlobals.GiftList.m_GiftItemData.Count; num++)
			{
				m_InstancedButtons[num] = (GameObject)Object.Instantiate(m_GiftButtonPrefab);
				m_InstancedButtons[num].name = m_GiftButtonPrefab.name + "_Instance" + num;
			}
			m_AllocatedGiftSprites = new List<GiftSprite>();
			m_AllocatedGiftSprites.AddRange(m_ListOfGiftSpriteNames);
		}

		private void ValidateSceneReferences()
		{
			DebugUtils.Assert(m_GiftButtonPrefab, string.Empty);
			DebugUtils.Assert(m_BurstScreenRoot, string.Empty);
			DebugUtils.Assert(m_BackButton, string.Empty);
			DebugUtils.Assert(m_ContinueButton, string.Empty);
			DebugUtils.Assert(m_GiftButtonPrefab, string.Empty);
		}

		private IEnumerator GiftSelectionLoop()
		{
			while (true)
			{
				GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_ChooseGift_WaitForGiftToBeChosen);
				ActivateSceneObject(m_BackButton);
				ActivateSceneObject(m_ShelfBackground);
				ActivateSceneObject(m_SeasonalBackgrounds);
				yield return new WaitForEndOfFrame();
				DeActivateSceneObject(m_TransferBackground);
				DeActivateSceneObject(m_BurstBackground);
				RefreshGiftSelectUI();
				yield return StartCoroutine(m_Waiter.WaitForEvent(GiftUnlockingEvents.Gifting_ChooseGift_Chosen));
				int giftIndex = (int)m_Waiter.ReturnedParameters[0];
				if (Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode)
				{
					yield return StartCoroutine(PopulateAndShowGiftUnwrapPopup(giftIndex));
				}
				else
				{
					yield return StartCoroutine(MakeGiftInteractiveAndWaitUntilSentToFurby(giftIndex));
				}
				DeActivateSceneObject(m_BurstScreenRoot);
				DeActivateSceneObject(m_ContinueButton);
				ActivateSceneObject(m_GiftLocatorRoot);
			}
		}

		private IEnumerator MakeGiftInteractiveAndWaitUntilSentToFurby(int giftIndex)
		{
			yield return StartCoroutine(MakeGiftInteractive(giftIndex));
			yield return StartCoroutine(WaitUntilGiftSentToFurby(giftIndex));
		}

		private IEnumerator WaitUntilGiftSentToFurby(int giftIndex)
		{
			ActivateSceneObject(m_CancelButton);
			GetComponent<GiftUnlockingHints>().m_SuggestFlicking.Enable();
			yield return StartCoroutine(m_Waiter.WaitForEvent(GiftUnlockingEvents.Gifting_Interaction_ItemSentToFurby, GiftUnlockingEvents.Gifting_Interaction_TimedOut, GiftUnlockingEvents.Gifting_Interaction_Cancelled));
			GetComponent<GiftUnlockingHints>().m_SuggestFlicking.Disable();
			DeActivateSceneObject(m_CancelButton);
			BounceItem bounceItem = m_BounceItemGameObject.GetComponent<BounceItem>();
			bounceItem.OnGiven -= OnGiftGiven;
			bounceItem.OnBounce -= OnGiftBounced;
			bounceItem.OnTimeOut -= OnGiftTimedOut;
			switch ((GiftUnlockingEvents)(object)m_Waiter.ReturnedEvent)
			{
			case GiftUnlockingEvents.Gifting_Interaction_ItemSentToFurby:
			{
				DeActivateSceneObject(m_BounceItemGameObject);
				FurbyComAirWaiter furbyWaiter = new FurbyComAirWaiter();
				yield return StartCoroutine(furbyWaiter.SendComAirEventAndWaitForResponse(FurbyAction.SpittingOut_Chomp, (FurbyCommand)0, true, 6f));
				if (!furbyWaiter.ReceivedResponse())
				{
					ActivateSceneObject(m_ComAirErrorDialog.gameObject);
					m_ComAirErrorDialog.SetOKState("FURBYCOMMSERROR_MESSAGE", "MENU_OPTION_OK", SharedGuiEvents.DialogAccept);
					m_ComAirErrorDialog.Show(true);
					yield return StartCoroutine(m_Waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
					DeActivateSceneObject(m_ComAirErrorDialog.gameObject);
					RefreshGiftSelectUI();
				}
				else
				{
					yield return StartCoroutine(HandleBurstScreen(giftIndex));
				}
				break;
			}
			case GiftUnlockingEvents.Gifting_Interaction_TimedOut:
			case GiftUnlockingEvents.Gifting_Interaction_Cancelled:
			{
				DeActivateSceneObject(m_TransferBackground);
				GameObject targetGift = m_InstancedButtons[giftIndex];
				TweenScale.Begin(m_BounceItemGameObject, m_TweeningDurationSecs, m_Starting_Scale).method = UITweener.Method.EaseInOut;
				TweenPosition.Begin(pos: targetGift.transform.parent.gameObject.transform.localPosition, go: m_BounceItemGameObject, duration: m_TweeningDurationSecs).method = UITweener.Method.EaseInOut;
				yield return new WaitForSeconds(m_TweeningDurationSecs);
				DeActivateSceneObject(m_BounceItemGameObject);
				break;
			}
			}
		}

		private IEnumerator MakeGiftInteractive(int giftIndex)
		{
			yield return StartCoroutine(ShowTheInteractionScreen(giftIndex));
			m_Starting_Scale = m_BounceItemGameObject.transform.localScale;
			GameObject targetGift = m_InstancedButtons[giftIndex];
			Vector3 starting_Position = targetGift.transform.position;
			Vector3 destinationScaleVec = m_Starting_Scale * m_TransferScreenUpscale;
			Vector3 destinationV3 = m_BounceItemGameObject.transform.position;
			TweenScale.Begin(m_BounceItemGameObject, m_TweeningDurationSecs, destinationScaleVec).method = UITweener.Method.EaseInOut;
			m_BounceItemGameObject.transform.position = starting_Position;
			TweenPosition.Begin(m_BounceItemGameObject, m_TweeningDurationSecs, destinationV3).method = UITweener.Method.EaseInOut;
			BounceItem bounceItem = m_BounceItemGameObject.GetComponent<BounceItem>();
			bounceItem.InitializeManually(targetSprite: m_BounceItemGameObject.GetComponent<UISprite>(), initialPosition: m_GiftLocatorRoot.transform.position, timeOutSecs: m_TransferTimeoutSecs, scaler: m_TransferScreenUpscale);
			yield return new WaitForSeconds(m_TweeningDurationSecs);
			ActivateSceneObject(m_TransferBackground);
			ActivateSceneObject(m_BounceItemGameObject);
			bounceItem.OnGiven += OnGiftGiven;
			bounceItem.OnBounce += OnGiftBounced;
			bounceItem.OnTimeOut += OnGiftTimedOut;
		}

		private IEnumerator ShowTheInteractionScreen(int giftIndex)
		{
			DeActivateSceneObject(m_BackButton);
			for (int nthGift = 0; nthGift < FurbyGlobals.GiftList.m_GiftItemData.Count; nthGift++)
			{
				GiftSelect giftSelectScript = m_InstancedButtons[nthGift].GetComponentInChildren<GiftSelect>();
				if (nthGift == giftIndex)
				{
					giftSelectScript.GetTargetSprite().color = new Color(0f, 0f, 0f, 0f);
				}
				else
				{
					TweenAlpha.Begin(giftSelectScript.GetTargetSprite().gameObject, m_TweeningDurationSecs, 0f);
				}
				if (m_SpotlightReferences[nthGift].activeInHierarchy)
				{
					TweenAlpha.Begin(m_SpotlightReferences[nthGift], m_TweeningDurationSecs, 0f);
				}
			}
			GameObject targetGift = m_InstancedButtons[giftIndex];
			GiftSelect giftSelect = targetGift.GetComponentInChildren<GiftSelect>();
			UISprite targetSprite = giftSelect.GetTargetSprite();
			UISprite bounceItemSprite = m_BounceItemGameObject.GetComponent<UISprite>();
			bounceItemSprite.atlas = targetSprite.atlas;
			bounceItemSprite.spriteName = targetSprite.spriteName;
			bounceItemSprite.MakePixelPerfect();
			bounceItemSprite.transform.localScale *= m_GiftSpriteUpscaleWrappedGift;
			yield return null;
		}

		private void OnGiftBounced()
		{
			GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_Interaction_BouncedOffWall);
		}

		private void OnGiftGiven()
		{
			GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_Interaction_ItemSentToFurby);
		}

		private void OnGiftTimedOut()
		{
			GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_Interaction_TimedOut);
		}

		private IEnumerator HandleBurstScreen(int giftIndex)
		{
			GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_BurstScreen_SequenceStarted);
			yield return StartCoroutine(PopulateAndShowTheBurstScreen(giftIndex));
			GameEventRouter.SendEvent(GiftUnlockingEvents.Gifting_BurstScreen_SequenceCompleted);
			yield return StartCoroutine(m_Waiter.WaitForEvent(GiftUnlockingEvents.Gifting_BurstScreen_BackToGiftSelect));
			DeActivateSceneObject(m_BurstBackground);
		}

		private void RefreshGiftSelectUI()
		{
			for (int i = 0; i < FurbyGlobals.GiftList.m_GiftItemData.Count; i++)
			{
				GiftStatus giftStatus = GiftStatus.Locked;
				if (Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices.Contains(i))
				{
					giftStatus = GiftStatus.Opened;
				}
				if (Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Contains(i))
				{
					giftStatus = GiftStatus.Unopened;
				}
				GameObject gameObject = m_GiftButtons[i];
				m_InstancedButtons[i].transform.parent = gameObject.transform;
				m_InstancedButtons[i].transform.localPosition = Vector3.zero;
				m_InstancedButtons[i].transform.localScale = Vector3.one;
				m_InstancedButtons[i].transform.localRotation = Quaternion.identity;
				GiftSelect componentInChildren = m_InstancedButtons[i].GetComponentInChildren<GiftSelect>();
				componentInChildren.InitalizeGift(i, giftStatus);
				UISprite targetSprite = componentInChildren.GetTargetSprite();
				switch (giftStatus)
				{
				case GiftStatus.Locked:
					targetSprite.atlas = m_GiftUnlockingAtlas;
					targetSprite.spriteName = GetGiftSpriteForIndex(i).m_SpriteName;
					targetSprite.color = new Color(0f, 0f, 0f, 0f);
					m_SpotlightReferences[i].SetActive(false);
					m_InstancedButtons[i].transform.localPosition += GetGiftSpriteForIndex(i).m_TransformOffset;
					targetSprite.MakePixelPerfect();
					targetSprite.transform.localScale *= m_GiftSpriteUpscaleWrappedGift;
					break;
				case GiftStatus.Unopened:
					targetSprite.atlas = m_GiftUnlockingAtlas;
					targetSprite.spriteName = GetGiftSpriteForIndex(i).m_SpriteName;
					targetSprite.color = new Color(1f, 1f, 1f, 1f);
					m_SpotlightReferences[i].SetActive(true);
					m_InstancedButtons[i].transform.localPosition += GetGiftSpriteForIndex(i).m_TransformOffset;
					targetSprite.MakePixelPerfect();
					targetSprite.transform.localScale *= m_GiftSpriteUpscaleWrappedGift;
					break;
				case GiftStatus.Opened:
					targetSprite.atlas = m_GiftPresentation.GetCarouselAtlasForGift(i);
					targetSprite.spriteName = m_GiftPresentation.GetCarouselSpriteNameForGift(i);
					targetSprite.color = new Color(1f, 1f, 1f, 1f);
					m_SpotlightReferences[i].SetActive(true);
					targetSprite.MakePixelPerfect();
					targetSprite.transform.localScale *= m_GiftSpriteUpscaleUnwrappedGift;
					break;
				}
				if (m_SpotlightReferences[i].activeInHierarchy)
				{
					TweenAlpha.Begin(m_SpotlightReferences[i], 0f, 0.42f);
				}
			}
		}

		public void Helper_OpenGift(int nthGift)
		{
			if (Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Contains(nthGift))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Remove(nthGift);
			}
			if (!Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices.Contains(nthGift))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices.Add(nthGift);
			}
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		public void Helper_AwardGift(int nthGift)
		{
			if (!Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Contains(nthGift))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Add(nthGift);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private IEnumerator PopulateAndShowTheBurstScreen(int giftIndex)
		{
			Helper_OpenGift(giftIndex);
			DeActivateSceneObject(m_GiftLocatorRoot);
			DeActivateSceneObject(m_ShelfBackground);
			DeActivateSceneObject(m_SeasonalBackgrounds);
			ActivateSceneObject(m_BurstBackground);
			yield return new WaitForEndOfFrame();
			DeActivateSceneObject(m_TransferBackground);
			m_UnlockSpriteTarget.atlas = m_GiftPresentation.GetUnlockAtlasForGift(giftIndex);
			m_UnlockSpriteTarget.spriteName = m_GiftPresentation.GetUnlockSpriteNameForGift(giftIndex);
			m_UnlockSpriteTarget.MakePixelPerfect();
			m_UnlockSpriteTarget.transform.localScale *= m_BurstScreenSpriteUpscale;
			ActivateSceneObject(m_BurstScreenRoot);
			DeActivateSceneObject(m_ContinueButton);
			yield return new WaitForSeconds(5f);
			ActivateSceneObject(m_ContinueButton);
		}

		private IEnumerator PopulateAndShowTheCantAffordToOpenGiftDialog()
		{
			ActivateSceneObject(m_CantAffordToOpenGiftDialog);
			yield return StartCoroutine(m_Waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
			DeActivateSceneObject(m_CantAffordToOpenGiftDialog);
		}

		private IEnumerator PopulateAndShowTheConfirmFurbuckSpend()
		{
			UILabel targetLabel = m_ConfirmFurbuckSpendDialog.GetChildGameObject("Message").GetComponent<UILabel>();
			string format = Singleton<Localisation>.Instance.GetText("GIFTING_NOFURBY_CONFIRMPURCHASE");
			targetLabel.text = string.Format(format, m_FurbucksCostToOpenTheGift.ToString());
			ActivateSceneObject(m_ConfirmFurbuckSpendDialog);
			yield return StartCoroutine(m_Waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
			DeActivateSceneObject(m_ConfirmFurbuckSpendDialog);
		}

		private IEnumerator PopulateAndShowGiftUnwrapPopup(int giftIndex)
		{
			DeActivateSceneObject(m_BackButton);
			ActivateSceneObject(m_GenericDialogBlankingPanel);
			ActivateSceneObject(m_FurbucksWalletDisplay);
			ActivateSceneObject(m_PayToOpenGiftPopupDialog);
			m_PayToOpenGiftPopupDialog.GetChildGameObject("Cost").GetComponent<UILabel>().text = m_FurbucksCostToOpenTheGift.ToString();
			yield return StartCoroutine(m_Waiter.WaitForEvent(GiftUnlockingEvents.Gifting_PayToUnwrapGift_BuyFurbyBoom, GiftUnlockingEvents.Gifting_PayToUnwrapGift_Confirm, GiftUnlockingEvents.Gifting_PayToUnwrapGift_Decline));
			DeActivateSceneObject(m_PayToOpenGiftPopupDialog);
			GiftUnlockingEvents returnedEvent = (GiftUnlockingEvents)(object)m_Waiter.ReturnedEvent;
			bool showBurst = false;
			switch (returnedEvent)
			{
			case GiftUnlockingEvents.Gifting_PayToUnwrapGift_BuyFurbyBoom:
				DeActivateSceneObject(m_FurbucksWalletDisplay);
				yield return StartCoroutine(m_Waiter.WaitForEvent(SharedGuiEvents.DialogCancel, SharedGuiEvents.DialogAccept));
				break;
			case GiftUnlockingEvents.Gifting_PayToUnwrapGift_Confirm:
				if (FurbyGlobals.Wallet.Balance >= m_FurbucksCostToOpenTheGift)
				{
					yield return StartCoroutine(PopulateAndShowTheConfirmFurbuckSpend());
					if ((SharedGuiEvents)(object)m_Waiter.ReturnedEvent == SharedGuiEvents.DialogAccept)
					{
						FurbyGlobals.Wallet.Balance -= m_FurbucksCostToOpenTheGift;
						showBurst = true;
					}
				}
				else
				{
					yield return StartCoroutine(PopulateAndShowTheCantAffordToOpenGiftDialog());
				}
				break;
			}
			DeActivateSceneObject(m_FurbucksWalletDisplay);
			DeActivateSceneObject(m_GenericDialogBlankingPanel);
			if (!showBurst)
			{
				yield break;
			}
			foreach (GameObject spotlight in m_SpotlightReferences)
			{
				spotlight.SetActive(false);
			}
			yield return StartCoroutine(HandleBurstScreen(giftIndex));
		}

		private static void ActivateSceneObject(GameObject sceneGameObject)
		{
			if (!sceneGameObject.activeInHierarchy)
			{
				sceneGameObject.SetActive(true);
			}
		}

		private static void DeActivateSceneObject(GameObject sceneGameObject)
		{
			if (sceneGameObject.activeInHierarchy)
			{
				sceneGameObject.SetActive(false);
			}
		}
	}
}
