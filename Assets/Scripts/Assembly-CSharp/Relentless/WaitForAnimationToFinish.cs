using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[ActionCategory(ActionCategory.Animation)]
	public class WaitForAnimationToFinish : FsmStateAction
	{
		public FsmGameObject m_target;

		public FsmEvent m_finishEvent;

		public override void OnUpdate()
		{
			GameObject gameObject = m_target.Value;
			ModelInstance component = gameObject.GetComponent<ModelInstance>();
			if (component != null)
			{
				gameObject = component.Instance;
			}
			if (gameObject.animation != null && !gameObject.animation.isPlaying)
			{
				base.Fsm.Event(m_finishEvent);
				Finish();
			}
		}
	}
}
