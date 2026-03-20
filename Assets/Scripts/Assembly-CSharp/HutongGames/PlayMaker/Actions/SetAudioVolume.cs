using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	[ActionCategory(ActionCategory.Audio)]
	public class SetAudioVolume : FsmStateAction
	{
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			volume = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAudioVolume();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetAudioVolume();
		}

		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource audio = ownerDefaultTarget.audio;
				if (audio != null && !volume.IsNone)
				{
					audio.volume = volume.Value;
				}
			}
		}
	}
}
