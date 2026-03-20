using HutongGames.PlayMaker;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class ObjectHitManager : Singleton<ObjectHitManager>
	{
		public GameObject m_FurbyBaby;

		public Camera m_3DCamera;

		public Camera m_2DCamera;

		private PlayMakerFSM m_GameStateMachine;

		private FsmEventTarget m_EventTarget = new FsmEventTarget();

		public void OnObjectHit(GameObject hitObject)
		{
			if (!(m_GameStateMachine.ActiveStateName != "Ready"))
			{
				HexGridPosition component = m_FurbyBaby.GetComponent<HexGridPosition>();
				HexGridPosition component2 = hitObject.GetComponent<HexGridPosition>();
				int num = Mathf.RoundToInt(component.GetDistance(component2));
				if (num == 0)
				{
					m_GameStateMachine.Fsm.Event(m_EventTarget, "Found");
					SingletonInstance<PrefabPool>.Instance.ReturnToPool(hitObject);
					return;
				}
				if (Singleton<HideAndSeekState>.Instance.TriesLeft == 1)
				{
					Singleton<HideAndSeekState>.Instance.HandleTurns();
					return;
				}
				string[] array = new string[6] { "Found", "VeryHot", "Hot", "Warm", "Cold", "Freezing" };
				m_GameStateMachine.Fsm.Event(m_EventTarget, array[num]);
				Singleton<HideAndSeekUtlity>.Instance.LastHitObject = hitObject;
				m_GameStateMachine.Fsm.Event(m_EventTarget, "ObjectHit");
				Singleton<SpecialObjectManager>.Instance.OnObjectHit(hitObject);
				SingletonInstance<PrefabPool>.Instance.ReturnToPool(hitObject);
			}
		}

		private void Start()
		{
			m_GameStateMachine = GetComponent<PlayMakerFSM>();
			m_EventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
		}
	}
}
