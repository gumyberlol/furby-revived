using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[HutongGames.PlayMaker.Tooltip("Play Animation Repeatedly")]
	[ActionCategory("Animation")]
	public class ContinueAnimation : FsmStateAction
	{
		public GameObject m_GameObject;

		private Animation m_AnimationObject;

		[UIHint(UIHint.Animation)]
		public string m_AnimationName;

		public float m_BlendTime;

		public override void OnEnter()
		{
			m_AnimationObject = m_GameObject.animation;
		}

		public override void OnUpdate()
		{
			if (m_AnimationObject.isPlaying)
			{
				if (!m_AnimationObject.IsPlaying(m_AnimationName))
				{
					m_AnimationObject.CrossFade(m_AnimationName, m_BlendTime);
				}
			}
			else
			{
				m_AnimationObject.Play(m_AnimationName);
			}
		}
	}
}
