using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class DiscoRailAnimations : RelentlessMonoBehaviour
	{
		[SerializeField]
		private AnimationClip m_offAnimation;

		[SerializeField]
		private AnimationClip m_inAnimation;

		[SerializeField]
		private AnimationClip m_outAnimation;

		public void SetDiscoModeStarting(float duration)
		{
			base.animation.Play(m_inAnimation.name);
			base.animation[m_inAnimation.name].speed = 3f;
			Invoke(PlayOffAnim, duration - m_outAnimation.length);
		}

		public float GetDiscoModeOutLength()
		{
			return m_outAnimation.length;
		}

		private void PlayOffAnim()
		{
			base.animation.Play(m_outAnimation.name);
		}

		public void SetDiscoModeOff()
		{
			base.animation.Play(m_offAnimation.name);
		}
	}
}
