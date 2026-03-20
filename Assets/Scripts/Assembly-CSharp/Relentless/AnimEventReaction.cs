using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class AnimEventReaction : GameEventReaction
	{
		public enum Operation
		{
			Play = 0,
			Stop = 1,
			CrossFade = 2,
			Blend = 3
		}

		public string AnimName;

		public string[] AnimNameArray = new string[0];

		public Operation AnimOperation;

		public bool AnimQueued;

		public QueueMode AnimQueueMode;

		public PlayMode AnimPlayMode;

		public AnimationBlendMode AnimBlendMode;

		public WrapMode AnimWrapMode;

		public float AnimFadeLength = 0.3f;

		public float AnimTargetWeight = 1f;

		public bool OverrideGameobject = true;

		public float StartPos;

		public float Speed = 1f;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			string text = AnimName;
			if (!OverrideGameobject)
			{
				gameObject = (GameObject)paramlist[0];
			}
			if (gameObject == null)
			{
				return;
			}
			ModelInstance component = gameObject.GetComponent<ModelInstance>();
			if (component != null)
			{
				gameObject = component.Instance;
			}
			if (gameObject == null || gameObject.animation == null)
			{
				return;
			}
			if (AnimNameArray.Length > 1)
			{
				text = AnimNameArray[UnityEngine.Random.Range(0, AnimNameArray.Length)];
			}
			switch (AnimOperation)
			{
			case Operation.Play:
				if (AnimQueued)
				{
					gameObject.animation.PlayQueued(text, AnimQueueMode, AnimPlayMode);
				}
				else
				{
					gameObject.animation.Play(text, AnimPlayMode);
				}
				gameObject.animation[text].wrapMode = AnimWrapMode;
				gameObject.animation[text].time = StartPos;
				gameObject.animation[text].speed = Speed;
				break;
			case Operation.Stop:
			{
				string[] animNameArray = AnimNameArray;
				foreach (string name in animNameArray)
				{
					gameObject.animation.Stop(name);
				}
				gameObject.animation.Stop(text);
				break;
			}
			case Operation.CrossFade:
				if (AnimQueued)
				{
					gameObject.animation.CrossFadeQueued(text, AnimFadeLength, AnimQueueMode, AnimPlayMode);
				}
				else
				{
					gameObject.animation.CrossFade(text, AnimFadeLength);
				}
				gameObject.animation[text].wrapMode = AnimWrapMode;
				break;
			case Operation.Blend:
				gameObject.animation[text].blendMode = AnimBlendMode;
				gameObject.animation.Blend(text, AnimTargetWeight, AnimFadeLength);
				break;
			}
		}
	}
}
