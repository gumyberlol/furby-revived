using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
	public float m_minAnimSpeed = 0.75f;

	public float m_maxAnimSpeed = 1.25f;

	private void Start()
	{
		AnimationClip clip = base.animation.clip;
		base.animation[clip.name].speed = Random.Range(m_minAnimSpeed, m_maxAnimSpeed);
	}
}
