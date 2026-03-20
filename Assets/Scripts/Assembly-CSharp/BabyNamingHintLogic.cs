using System;
using System.Collections;
using Furby;
using Relentless;
using UnityEngine;

public class BabyNamingHintLogic : RelentlessMonoBehaviour
{
	public float m_IntervalBeforePromptingConfirm = 4f;

	public float m_IntervalBeforePromptingScroll = 4f;

	private bool m_ScrollbarsMovedThisSession;

	private IEnumerator Start()
	{
		FurbyGlobals.InputInactivity.ResetInactivity();
		m_ScrollbarsMovedThisSession = false;
		yield return new WaitForSeconds(0.5f);
		AttachDelegates();
		yield return null;
	}

	public void OnDisable()
	{
		DetachDelegates();
	}

	private void AttachDelegates()
	{
		GameEventRouter.AddDelegateForEnums(HandleScrollbarMovement, BabyNameGameEvent.NamingScreenRightScrollSnapCentre);
		GameEventRouter.AddDelegateForEnums(HandleScrollbarMovement, BabyNameGameEvent.NamingScreenLeftScrollSnapCentre);
	}

	private void DetachDelegates()
	{
		GameEventRouter.RemoveDelegateForEnums(HandleScrollbarMovement, BabyNameGameEvent.NamingScreenRightScrollSnapCentre);
		GameEventRouter.RemoveDelegateForEnums(HandleScrollbarMovement, BabyNameGameEvent.NamingScreenLeftScrollSnapCentre);
	}

	private void HandleScrollbarMovement(Enum enumValue, GameObject gameObject, params object[] list)
	{
		m_ScrollbarsMovedThisSession = true;
	}

	public void OnGUI()
	{
		if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
		}
	}

	private void Update()
	{
		if (m_ScrollbarsMovedThisSession)
		{
			if (!FurbyGlobals.Player.InProgressFurbyBaby.HasBeenNamed)
			{
				HandleConfirmationPrompt();
			}
		}
		else
		{
			HandleScrollPrompt();
		}
	}

	private void HandleScrollPrompt()
	{
		if (FurbyGlobals.InputInactivity.HasIntervalPassed(m_IntervalBeforePromptingScroll))
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			GameEventRouter.SendEvent(HintEvents.Naming_SuggestScrollingWheels);
		}
	}

	private void HandleConfirmationPrompt()
	{
		if (FurbyGlobals.InputInactivity.HasIntervalPassed(m_IntervalBeforePromptingConfirm))
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			GameEventRouter.SendEvent(HintEvents.Naming_SuggestConfirmingName);
		}
	}
}
