using HutongGames.PlayMaker;
using UnityEngine;

namespace Furby
{
	[HutongGames.PlayMaker.Tooltip("Disables/Enables a list of GameObjects")]
	[ActionCategory(ActionCategory.GameObject)]
	public class SetActiveObjects : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The objects to be disabled")]
		public FsmGameObject[] m_ObjectsToDisable;

		[HutongGames.PlayMaker.Tooltip("The objects to be enabled")]
		public FsmGameObject[] m_ObjectsToEnable;

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
		}

		public override void OnUpdate()
		{
			if (InternalUpdate(Time.deltaTime))
			{
				Finish();
			}
		}

		private bool InternalUpdate(float deltaTime)
		{
			bool result = true;
			FsmGameObject[] objectsToDisable = m_ObjectsToDisable;
			foreach (FsmGameObject fsmGameObject in objectsToDisable)
			{
				fsmGameObject.Value.SetActive(false);
			}
			FsmGameObject[] objectsToEnable = m_ObjectsToEnable;
			foreach (FsmGameObject fsmGameObject2 in objectsToEnable)
			{
				fsmGameObject2.Value.SetActive(true);
			}
			return result;
		}
	}
}
