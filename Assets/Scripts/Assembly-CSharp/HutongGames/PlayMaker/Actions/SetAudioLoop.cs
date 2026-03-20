using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets looping on the AudioSource component on a Game Object.")]
	[ActionCategory(ActionCategory.Audio)]
	public class SetAudioLoop : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		public FsmBool loop;

		public override void Reset()
		{
			gameObject = null;
			loop = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource audio = ownerDefaultTarget.audio;
				if (audio != null)
				{
					audio.loop = loop.Value;
				}
			}
			Finish();
		}
	}
}
