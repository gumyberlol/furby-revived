using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the current Time of an Animation, Normalize time means 0 (start) to 1 (end); useful if you don't care about the exact time. Check Every Frame to update the time continuosly.")]
	[ActionCategory(ActionCategory.Animation)]
	public class SetAnimationTime : FsmStateAction
	{
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Animation)]
		[RequiredField]
		public FsmString animName;

		public FsmFloat time;

		public bool normalized;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			time = null;
			normalized = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAnimationTime((gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? gameObject.GameObject.Value : base.Owner);
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetAnimationTime((gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? gameObject.GameObject.Value : base.Owner);
		}

		private void DoSetAnimationTime(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			if (go.animation == null)
			{
				LogWarning("Missing animation component: " + go.name);
				return;
			}
			go.animation.Play(animName.Value);
			AnimationState animationState = go.animation[animName.Value];
			if (animationState == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				return;
			}
			if (normalized)
			{
				animationState.normalizedTime = time.Value;
			}
			else
			{
				animationState.time = time.Value;
			}
			if (everyFrame)
			{
				animationState.speed = 0f;
			}
		}
	}
}
