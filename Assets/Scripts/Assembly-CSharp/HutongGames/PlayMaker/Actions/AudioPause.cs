using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPause : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource audio = ownerDefaultTarget.audio;
				if (audio != null)
				{
					audio.Pause();
				}
			}
			Finish();
		}
	}
}
