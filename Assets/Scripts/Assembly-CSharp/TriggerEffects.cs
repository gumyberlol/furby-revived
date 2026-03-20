using UnityEngine;

public class TriggerEffects : MonoBehaviour
{
	public AnimationClip m_animEmit;

	public AnimationClip m_animReset;

	public void EffectsEmit()
	{
		base.animation.Play(m_animEmit.name);
	}

	public void EffectsReset()
	{
		if (m_animReset != null)
		{
			base.animation.Play(m_animReset.name);
		}
	}
}
