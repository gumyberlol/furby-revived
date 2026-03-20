using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Pantry
{
	public class PantryFlow : RelentlessMonoBehaviour
	{
		[SerializeField]
		private PantryFoodDataList m_FoodList;

		[SerializeField]
		public PantryFoodDataList m_ReturnFoodList;

		[SerializeField]
		private PantryShelf_v03[] m_Shelves;

		[SerializeField]
		private GameObject m_FoodItem;

		private BounceItem m_FoodBounceItem;

		private bool m_IsFoodAvailable;

		private bool m_HasFoodBeenSent;

		private PantryFoodData m_CurrentFoodData;

		[SerializeField]
		private float m_BoredWaitTime = 10f;

		[SerializeField]
		private float m_FoodReactionTime = 2f;

		[SerializeField]
		private ErrorMessageBox m_ErrorBox;

		private FurbyComAirWaiter m_ComAirWaiter;

		[SerializeField]
		private GameObject m_ResetButton;

		[SerializeField]
		public HintState m_HintStateScroll = new HintState();

		[SerializeField]
		public HintState m_HintStateSelectFood = new HintState();

		[SerializeField]
		public HintState m_HintStateSendFood = new HintState();

		[SerializeField]
		public HintState m_HintStateGoBack = new HintState();

		private bool m_HaveScrolledThisSession;

		private GameEventSubscription m_DebugPanelSub;

		private bool m_ForceSpitbacks;

		public bool HaveScrolledThisSession
		{
			set
			{
				m_HaveScrolledThisSession = value;
			}
		}

		public GameObject StartFoodItem
		{
			get
			{
				return GameObject.Find("StartPantryFoodItem");
			}
		}

		private void Awake()
		{
			m_HaveScrolledThisSession = false;
			for (int i = 0; i < m_Shelves.Length; i++)
			{
				if (m_Shelves[i] == null)
				{
					Logging.LogError(string.Format("Shelf {0} is a null reference!", i));
				}
			}
			if (m_FoodItem == null)
			{
				Logging.LogError("No Food Item Reference!");
			}
			BounceItem bounceItem = (m_FoodBounceItem = m_FoodItem.GetComponentInChildren<BounceItem>());
			if (m_FoodBounceItem == null)
			{
				Logging.LogError("No Food BounceItem!");
			}
			bounceItem.OnGiven += OnFoodGiven;
			bounceItem.OnBounce += OnFoodBounced;
			bounceItem.OnFlicked += OnFoodFlicked;
			bounceItem.OnTimeOut += OnFoodTimedOut;
			StockShelves();
			m_ComAirWaiter = new FurbyComAirWaiter();
			if (m_ResetButton == null)
			{
				Logging.LogError("No Reset Button Reference!");
			}
			PantryResetButton componentInChildren = m_ResetButton.GetComponentInChildren<PantryResetButton>();
			if (componentInChildren == null)
			{
				Logging.LogError("No Pantry Reset Button on Reset Button!");
			}
			componentInChildren.OnReset += OnResetClicked;
		}

		private void StockShelves()
		{
			int num = m_Shelves.Length;
			List<PantryFoodData>[] array = new List<PantryFoodData>[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new List<PantryFoodData>();
			}
			int num2 = 0;
			foreach (PantryFoodData item in m_FoodList.Items)
			{
				array[num2 % num].Add(item);
				num2++;
			}
			for (int j = 0; j < num; j++)
			{
				array[j] = array[j].OrderBy((PantryFoodData a) => Guid.NewGuid()).ToList();
				m_Shelves[j].InitialiseCarousel(array[j].ToArray());
			}
		}

		private void ResetCommunications()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
		}

		private void ResetState()
		{
			ResetState(true);
		}

		private void ResetState(bool showShelves)
		{
			if (showShelves)
			{
				ShowShelves();
			}
			else
			{
				HideShelves(true);
			}
			HideFoodBounceItem();
			HideResetButton();
			m_ErrorBox.Hide();
			SetCurrentFoodData(null);
			m_HintStateSendFood.Disable();
			m_HintStateGoBack.Disable();
		}

		public void Update()
		{
			HandleInvocationOfHints();
		}

		private void HandleInvocationOfHints()
		{
			if (m_HintStateScroll.IsEnabled())
			{
				m_HintStateScroll.TestAndBroadcastState();
			}
			if (m_HintStateSelectFood.IsEnabled())
			{
				m_HintStateSelectFood.TestAndBroadcastState();
			}
		}

		private IEnumerator Start()
		{
			GameEventRouter.SendEvent(PantryEvent.PantryUtilityStarted, null, null);
			ResetCommunications();
			GameObject startingFoodItem = StartFoodItem;
			WaitForGameEvent waiter = new WaitForGameEvent();
			ResetState(startingFoodItem == null);
			while (true)
			{
				EnableAppropriateHints();
				if (startingFoodItem != null)
				{
					yield return StartCoroutine(ShowSelectedFoodItem(startingFoodItem));
					UnityEngine.Object.DestroyImmediate(startingFoodItem);
				}
				else
				{
					yield return StartCoroutine(WaitForFoodEvent(waiter));
				}
			}
		}

		private IEnumerator ShowSelectedFoodItem(GameObject startingFoodItem)
		{
			PantryFood_v03 foodItem = startingFoodItem.GetComponent<PantryFood_v03>();
			m_FoodBounceItem.ShouldTimeout = false;
			m_HintStateScroll.Disable();
			m_HintStateSelectFood.Disable();
			yield return StartCoroutine(HandleFoodItemClicked(foodItem));
		}

		private IEnumerator WaitForFoodEvent(WaitForGameEvent waiter)
		{
			yield return StartCoroutine(waiter.WaitForEvent(PantryEvent.FoodItemClicked));
			m_FoodBounceItem.ShouldTimeout = true;
			m_HintStateScroll.Disable();
			m_HintStateSelectFood.Disable();
			PantryEvent pantryEvent = (PantryEvent)(object)waiter.ReturnedEvent;
			if (pantryEvent == PantryEvent.FoodItemClicked)
			{
				yield return StartCoroutine(HandleFoodItemClickedFromEvent(waiter));
			}
			else
			{
				Logging.LogError(string.Format("Unexpected event in PantryFlow::Start : {0}", pantryEvent.ToString()));
			}
		}

		public void EnableAppropriateHints()
		{
			if (!m_IsFoodAvailable)
			{
				if (m_HaveScrolledThisSession)
				{
					m_HintStateSelectFood.Enable();
					m_HintStateScroll.Disable();
				}
				else
				{
					m_HintStateSelectFood.Disable();
					m_HintStateScroll.Enable();
				}
			}
		}

		private IEnumerator HandleFoodItemClickedFromEvent(WaitForGameEvent waiter)
		{
			PantryFood_v03 clickedFoodItem = (PantryFood_v03)waiter.ReturnedParameters[0];
			yield return StartCoroutine(HandleFoodItemClicked(clickedFoodItem));
		}

		private IEnumerator HandleFoodItemClicked(PantryFood_v03 foodItem)
		{
			GameEventRouter.SendEvent(PantryEvent.FoodItemSelected, null, null);
			HideShelves(false);
			ShowFoodBounceItem(foodItem.FoodData, foodItem.transform.position);
			ShowResetButton();
			yield return StartCoroutine("WaitForFoodToBeSent");
		}

		private IEnumerator WaitForFoodToBeSent()
		{
			StartCoroutine("BoredLoop");
			if (m_FoodBounceItem.ReturningItem)
			{
				m_HintStateGoBack.Enable();
				m_HintStateSendFood.Disable();
			}
			else
			{
				m_HintStateGoBack.Disable();
				m_HintStateSendFood.Enable();
			}
			while (m_IsFoodAvailable)
			{
				m_HintStateSendFood.TestAndBroadcastState();
				m_HintStateGoBack.TestAndBroadcastState();
				yield return null;
			}
			m_HintStateGoBack.Disable();
			m_HintStateSendFood.Disable();
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
			if (m_HasFoodBeenSent)
			{
				if (FurbyGlobals.Player.NoFurbyOnSaveGame() && !m_ForceSpitbacks)
				{
					yield return StartCoroutine(ShowNoFurbyMessage());
					ResetState();
				}
				else
				{
					yield return StartCoroutine(DoReturnItemSequence());
				}
			}
		}

		private IEnumerator BoredLoop()
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			while (true)
			{
				yield return new WaitForSeconds(m_BoredWaitTime);
				GameEventRouter.SendEvent(PantryEvent.FurbyBoredOfWaitingForFood, null, null);
				Logging.Log("Furby is bored waiting for the food!");
			}
		}

		private IEnumerator DoReturnItemSequence()
		{
			int personalityCode = (int)Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			FurbyAction foodReaction = (FurbyAction)m_CurrentFoodData.WatermarkForPersonality(personalityCode);
			GameEventRouter.SendEvent(PantryEvent.FurbyReactingToFood, null, null);
			bool shouldWaitForResponse = true;
			yield return StartCoroutine(SendComAirActionAndWaitForResponse(foodReaction, shouldWaitForResponse));
			yield return new WaitForSeconds(m_FoodReactionTime);
			if (m_ComAirWaiter.ReceivedResponse() || m_ForceSpitbacks)
			{
				GameEventRouter.SendEvent(PantryEvent.FoodWasActuallyReceived);
				string returnItemName = m_CurrentFoodData.ReturnForPersonality(personalityCode);
				PantryFoodData returnFoodData = m_ReturnFoodList.Find(returnItemName);
				if (returnFoodData == null)
				{
					ResetState();
					yield break;
				}
				ShowFoodBounceItem(returnFoodData, Vector3.zero);
				m_FoodBounceItem.Return();
				yield return StartCoroutine(WaitForFoodToBeSent());
			}
			else
			{
				Logging.Log("NO Response Received");
				yield return StartCoroutine(ShowCommunicationsErrorMessage());
				if (StartFoodItem != null)
				{
					OnResetFlow();
				}
				else
				{
					ResetState();
				}
			}
		}

		private IEnumerator SendComAirActionAndWaitForResponse(FurbyAction action, bool shouldWaitForResponse)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse(action, (FurbyCommand)0, shouldWaitForResponse));
		}

		private IEnumerator ShowCommunicationsErrorMessage()
		{
			GameEventRouter.SendEvent(PantryEvent.CommunicationsError, null, null);
			HideResetButton();
			m_ErrorBox.SetOKState("FURBYCOMMSERROR_MESSAGE", "MENU_OPTION_OK", PantryEvent.ErrorDialogOK);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(PantryEvent.ErrorDialogOK));
			m_ErrorBox.Hide();
		}

		private IEnumerator ShowNoFurbyMessage()
		{
			GameEventRouter.SendEvent(PantryEvent.NoFurby, null, null);
			yield break;
		}

		private void SetCurrentFoodData(PantryFoodData foodData)
		{
			m_CurrentFoodData = foodData;
		}

		private void ShowShelves()
		{
			for (int i = 0; i < m_Shelves.Length; i++)
			{
				PantryShelf_v03 pantryShelf_v = m_Shelves[i];
				pantryShelf_v.FadeShelf(true);
			}
		}

		private void HideShelves(bool instant)
		{
			for (int i = 0; i < m_Shelves.Length; i++)
			{
				PantryShelf_v03 pantryShelf_v = m_Shelves[i];
				if (instant)
				{
					pantryShelf_v.HideShelf();
				}
				else
				{
					pantryShelf_v.FadeShelf(false);
				}
			}
		}

		private void ShowFoodBounceItem(PantryFoodData foodData, Vector3 startPosition)
		{
			m_FoodBounceItem.Activate(foodData.GraphicAtlas, foodData.GraphicName, startPosition);
			m_IsFoodAvailable = true;
			m_HasFoodBeenSent = false;
			SetCurrentFoodData(foodData);
		}

		private void HideFoodBounceItem()
		{
			m_FoodBounceItem.Deactivate();
			m_IsFoodAvailable = false;
			m_HasFoodBeenSent = false;
		}

		private void ShowResetButton()
		{
			m_ResetButton.SetActive(true);
		}

		private void HideResetButton()
		{
			m_ResetButton.SetActive(false);
		}

		private void OnFoodGiven()
		{
			Logging.Log("OnFoodGiven");
			GameEventRouter.SendEvent(PantryEvent.FoodGiven, null, null);
			HideFoodBounceItem();
			m_HasFoodBeenSent = true;
			StopBoredLoop();
		}

		private void StopBoredLoop()
		{
			StopCoroutine("BoredLoop");
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
			FurbyGlobals.InputInactivity.ResetInactivity();
			m_HintStateGoBack.Disable();
			m_HintStateSendFood.Disable();
		}

		private void OnFoodBounced()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodBounced, null, null);
		}

		private void OnFoodFlicked()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodFlicked, null, null);
		}

		private void OnFoodTimedOut()
		{
			Logging.Log("OnFoodTimedOut");
			GameEventRouter.SendEvent(PantryEvent.FoodTimedOut, null, null);
			StopBoredLoop();
			ResetState();
		}

		private void OnResetFlow()
		{
			Logging.Log("OnResetFlow");
			StopAllCoroutines();
			ResetState(false);
			StartCoroutine(Start());
		}

		private void OnResetClicked()
		{
			Logging.Log("OnResetClicked");
			StopAllCoroutines();
			ResetState();
			GameObject startFoodItem = StartFoodItem;
			if (startFoodItem != null)
			{
				UnityEngine.Object.DestroyImmediate(startFoodItem);
			}
			StartCoroutine(Start());
		}

		private void OnDestroy()
		{
			if (StartFoodItem != null)
			{
				UnityEngine.Object.Destroy(StartFoodItem);
			}
			m_DebugPanelSub.Dispose();
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Pantry"))
			{
				GUILayout.BeginHorizontal();
				string text = string.Format("Force Spitbacks ({0})", (!m_ForceSpitbacks) ? "Off" : "On");
				if (GUILayout.Button(text))
				{
					m_ForceSpitbacks = !m_ForceSpitbacks;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Test Hardware Volume Stuff"))
				{
					Application.LoadLevel("_TestHardwareVolumeAndHeadphones");
				}
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
		}
	}
}
