using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Spot Angle of a Light.")]
	public class SetLightSpotAngle : FsmStateAction
	{
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat lightSpotAngle;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			lightSpotAngle = 20f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetLightRange();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetLightRange();
		}

		private void DoSetLightRange()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Light light = ownerDefaultTarget.light;
				if (light == null)
				{
					LogError("Missing Light Component!");
				}
				else
				{
					light.spotAngle = lightSpotAngle.Value;
				}
			}
		}
	}
}
