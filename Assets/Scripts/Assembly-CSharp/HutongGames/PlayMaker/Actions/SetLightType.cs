using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Set Spot, Directional, or Point Light type.")]
	public class SetLightType : FsmStateAction
	{
		[CheckForComponent(typeof(Light))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public LightType lightType;

		public override void Reset()
		{
			gameObject = null;
			lightType = LightType.Point;
		}

		public override void OnEnter()
		{
			DoSetLightType();
			Finish();
		}

		private void DoSetLightType()
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
					light.type = lightType;
				}
			}
		}
	}
}
