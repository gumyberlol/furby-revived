using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[HutongGames.PlayMaker.Tooltip("Enables the Audio Listener")]
	[ActionCategory("Audio")]
	public class EnableAudioListener2 : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioListener component = ownerDefaultTarget.GetComponent<AudioListener>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
			Finish();
		}
	}
}
