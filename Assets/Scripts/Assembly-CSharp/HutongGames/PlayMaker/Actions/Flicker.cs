using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Flickers a Game Object on/off.")]
	public class Flicker : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat frequency;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat amountOn;

		public bool rendererOnly;

		public bool realTime;

		private float startTime;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			frequency = 0.1f;
			amountOn = 0.5f;
			rendererOnly = true;
			realTime = false;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			timer = 0f;
		}

		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (realTime)
			{
				timer = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				timer += Time.deltaTime;
			}
			if (!(timer > frequency.Value))
			{
				return;
			}
			bool active = Random.Range(0f, 1f) < amountOn.Value;
			if (rendererOnly)
			{
				if (ownerDefaultTarget.renderer != null)
				{
					ownerDefaultTarget.renderer.enabled = active;
				}
			}
			else
			{
				ownerDefaultTarget.SetActive(active);
			}
			startTime = timer;
			timer = 0f;
		}
	}
}
