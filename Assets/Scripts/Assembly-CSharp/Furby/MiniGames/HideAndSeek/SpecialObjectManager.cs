using System;
using HutongGames.PlayMaker;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class SpecialObjectManager : Singleton<SpecialObjectManager>
	{
		public SpecialTriesPerLevel[] m_SpecialTries;

		private int[] m_ActivationTries;

		private GameObject m_CurrentSpecialObject;

		public GameObject m_SpecialObjectLabel;

		private int m_CurrentLevel;

		private PlayMakerFSM m_GameStateMachine;

		private FsmEventTarget m_EventTarget = new FsmEventTarget();

		public Camera m_2DCamera;

		public Camera m_3DCamera;

		public GameObject CurrentSpecialObject
		{
			get
			{
				return m_CurrentSpecialObject;
			}
		}

		private void Start()
		{
			m_CurrentLevel = Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel % Singleton<HideAndSeekState>.Instance.TotalLevels;
			SpecialTriesPerLevel specialTriesPerLevel = m_SpecialTries[m_CurrentLevel];
			m_ActivationTries = new int[specialTriesPerLevel.m_SpecialTriesRanges.Length];
			int num = 0;
			SpecialTryRange[] specialTriesRanges = specialTriesPerLevel.m_SpecialTriesRanges;
			foreach (SpecialTryRange specialTryRange in specialTriesRanges)
			{
				m_ActivationTries[num++] = UnityEngine.Random.Range(specialTryRange.m_MinTry, specialTryRange.m_MaxTry);
			}
			m_GameStateMachine = GetComponent<PlayMakerFSM>();
			m_EventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		}

		public void OnObjectHit(GameObject hitObject)
		{
			if ((bool)m_CurrentSpecialObject)
			{
				SpecialObject component = m_CurrentSpecialObject.GetComponent<SpecialObject>();
				UnityEngine.Object.Destroy(component);
				m_CurrentSpecialObject = null;
				m_SpecialObjectLabel.SetActive(false);
			}
			int currentTry = Singleton<HideAndSeekState>.Instance.CurrentTry;
			if (Array.IndexOf(m_ActivationTries, currentTry) < 0)
			{
				m_CurrentSpecialObject = null;
				m_SpecialObjectLabel.SetActive(false);
				return;
			}
			m_CurrentSpecialObject = GetSpecialObject(hitObject);
			if (!(m_CurrentSpecialObject == null))
			{
				SpecialObject specialObject = m_CurrentSpecialObject.AddComponent<SpecialObject>();
				int max = 4;
				specialObject.SpecialObjectType = (SpecialHitObjectType)UnityEngine.Random.Range(0, max);
				Vector3 position = m_3DCamera.WorldToScreenPoint(m_CurrentSpecialObject.transform.position);
				position.z = m_SpecialObjectLabel.transform.position.z;
				m_SpecialObjectLabel.transform.position = m_2DCamera.ScreenToWorldPoint(position);
				m_SpecialObjectLabel.SetActive(true);
			}
		}

		public void OnSpecialObjectHit(GameObject hitObject)
		{
			if (!(m_GameStateMachine.ActiveStateName != "Ready"))
			{
				m_SpecialObjectLabel.SetActive(false);
				if (m_CurrentSpecialObject == hitObject)
				{
					HandleSpecialHitEvent();
				}
				Singleton<HideAndSeekUtlity>.Instance.LastHitObject = hitObject;
				m_GameStateMachine.Fsm.Event(m_EventTarget, "SpecialObjectHit");
				OnObjectHit(hitObject);
				SingletonInstance<PrefabPool>.Instance.ReturnToPool(hitObject);
			}
		}

		private void HandleSpecialHitEvent()
		{
			SpecialObject component = m_CurrentSpecialObject.GetComponent<SpecialObject>();
			switch (component.SpecialObjectType)
			{
			case SpecialHitObjectType.ReHideBaby:
				m_GameStateMachine.Fsm.Event(m_EventTarget, "ReHideBaby");
				break;
			case SpecialHitObjectType.AddTries:
				m_GameStateMachine.Fsm.Event(m_EventTarget, "AddTries");
				break;
			case SpecialHitObjectType.SubtractTries:
				m_GameStateMachine.Fsm.Event(m_EventTarget, "SubtractTries");
				break;
			case SpecialHitObjectType.ChainReaction:
				m_GameStateMachine.Fsm.Event(m_EventTarget, "ChainReaction");
				break;
			}
			m_SpecialObjectLabel.SetActive(false);
			UnityEngine.Object.Destroy(component);
			m_CurrentSpecialObject = null;
		}

		public GameObject GetSpecialObject(GameObject objectToAvoid)
		{
			return Singleton<HideAndSeekUtlity>.Instance.GetRandomHideObject(objectToAvoid);
		}
	}
}
