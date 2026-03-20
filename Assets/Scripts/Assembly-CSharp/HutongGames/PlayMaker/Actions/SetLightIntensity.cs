using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Intensity of a Light.")]
	public class SetLightIntensity : FsmStateAction
	{
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat lightIntensity;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			lightIntensity = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetLightIntensity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetLightIntensity();
		}

		private void DoSetLightIntensity()
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
					light.intensity = lightIntensity.Value;
				}
			}
		}
	}
}
