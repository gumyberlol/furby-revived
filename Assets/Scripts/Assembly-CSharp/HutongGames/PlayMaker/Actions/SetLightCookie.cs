using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Lights)]
	[Tooltip("Sets the Texture projected by a Light.")]
	public class SetLightCookie : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Light))]
		public FsmOwnerDefault gameObject;

		public FsmTexture lightCookie;

		public override void Reset()
		{
			gameObject = null;
			lightCookie = null;
		}

		public override void OnEnter()
		{
			DoSetLightRange();
			Finish();
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
					light.cookie = lightCookie.Value;
				}
			}
		}
	}
}
