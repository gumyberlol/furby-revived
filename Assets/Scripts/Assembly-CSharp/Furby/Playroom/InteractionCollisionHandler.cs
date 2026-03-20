using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class InteractionCollisionHandler : RelentlessMonoBehaviour
	{
		public GameObject m_CollisionRoot;

		private List<GameObject> m_CollisionObjects = new List<GameObject>();

		private Vector2 m_PreviousFingerPos;

		private Vector2 m_OriginalFingerPosition;

		private float m_CumulativeDistanceMoved;

		[HideInInspector]
		public List<ThresholdTrigger> m_Thresholds = new List<ThresholdTrigger>();

		private void Awake()
		{
			foreach (Transform item in m_CollisionRoot.transform)
			{
				m_CollisionObjects.Add(item.gameObject);
			}
		}

		private void OnEnable()
		{
			FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
			FingerGestures.OnFingerSwipe += FingerGestures_OnFingerSwipe;
			FingerGestures.OnFingerDragBegin += FingerGestures_OnFingerDragBegin;
			FingerGestures.OnFingerDragMove += FingerGestures_OnFingerDragMove;
			FingerGestures.OnFingerDragEnd += FingerGestures_OnFingerDragEnd;
			PlayroomInteractionMediator playroomInteractionMediator = base.gameObject.GetComponent(typeof(PlayroomInteractionMediator)) as PlayroomInteractionMediator;
			playroomInteractionMediator.Initialise();
		}

		private void OnDisable()
		{
			FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
			FingerGestures.OnFingerSwipe -= FingerGestures_OnFingerSwipe;
			FingerGestures.OnFingerDragBegin -= FingerGestures_OnFingerDragBegin;
			FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDragMove;
			FingerGestures.OnFingerDragEnd -= FingerGestures_OnFingerDragEnd;
		}

		private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
		{
			SyndicateCollisionAndReaction(fingerPos, ActionType.Touch, 0f);
		}

		private void FingerGestures_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
		{
			m_CumulativeDistanceMoved = 0f;
			m_PreviousFingerPos = fingerPos;
			m_OriginalFingerPosition = fingerPos;
			foreach (ThresholdTrigger threshold in m_Thresholds)
			{
				threshold.m_Handled = false;
			}
		}

		private void FingerGestures_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
		{
			float num = Vector2.Distance(m_PreviousFingerPos, fingerPos);
			m_CumulativeDistanceMoved += num;
			foreach (ThresholdTrigger threshold in m_Thresholds)
			{
				if (!threshold.m_Handled && m_CumulativeDistanceMoved > threshold.m_ThresholdValue)
				{
					SyndicateCollisionAndReaction(m_OriginalFingerPosition, ActionType.Tickle, m_CumulativeDistanceMoved);
				}
			}
			m_PreviousFingerPos = fingerPos;
		}

		private void FingerGestures_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
		{
			SyndicateCollisionAndReaction(m_OriginalFingerPosition, ActionType.Tickle, m_CumulativeDistanceMoved);
			m_CumulativeDistanceMoved = 0f;
		}

		private void FingerGestures_OnFingerSwipe(int fingerIndex, Vector2 fingerPos, FingerGestures.SwipeDirection direction, float velocity)
		{
			SyndicateCollisionAndReaction(fingerPos, ActionType.Drag, 0f);
		}

		private void SyndicateCollisionAndReaction(Vector2 fingerPos, ActionType action, float userValue)
		{
			if (Singleton<PlayroomModeController>.Instance.GameMode != PlayroomMode.Interaction)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(fingerPos);
			RaycastHit[] array = Physics.RaycastAll(ray);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i].collider.gameObject;
				if (m_CollisionObjects.Contains(gameObject))
				{
					PlayroomInteractionMediator playroomInteractionMediator = base.gameObject.GetComponent(typeof(PlayroomInteractionMediator)) as PlayroomInteractionMediator;
					playroomInteractionMediator.HandleCollision(gameObject, action, userValue);
				}
			}
		}
	}
}
