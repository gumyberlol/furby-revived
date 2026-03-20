using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Mute/unmute the Audio Clip played by an Audio Source component on a Game Object.")]
	[ActionCategory(ActionCategory.Audio)]
	public class AudioMute : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool mute;

		public override void Reset()
		{
			gameObject = null;
			mute = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource audio = ownerDefaultTarget.audio;
				if (audio != null)
				{
					audio.mute = mute.Value;
				}
			}
			Finish();
		}
	}
}
