using UnityEngine;

public class CrystalGoldenAnimationIndirection : MonoBehaviour
{
	public GameObject m_spritesRoot;

	public GameObject m_sprial;

	public GameObject m_ufoRoot;

	public Animation m_ufoAnimation;

	public Animation m_diamondAnimation;

	public AnimationClip m_diamondRotateClip;

	public void Play()
	{
		m_spritesRoot.SetActive(true);
		m_sprial.SetActive(true);
		m_ufoRoot.SetActive(true);
		m_ufoAnimation.Play();
		m_diamondAnimation.Play(m_diamondRotateClip.name);
	}
}
