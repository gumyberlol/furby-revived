using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[ActionCategory("Audio")]
	[HutongGames.PlayMaker.Tooltip("Disables the Audio Listener")]
	public class DisableAudioListener2 : FsmStateAction
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
					component.enabled = false;
				}
			}
			Finish();
		}
	}
}
