using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
	[ActionCategory(ActionCategory.Audio)]
	public class SetAudioPitch : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		public FsmFloat pitch;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			pitch = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAudioPitch();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetAudioPitch();
		}

		private void DoSetAudioPitch()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource audio = ownerDefaultTarget.audio;
				if (audio != null && !pitch.IsNone)
				{
					audio.pitch = pitch.Value;
				}
			}
		}
	}
}
