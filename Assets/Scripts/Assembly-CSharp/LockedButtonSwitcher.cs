using UnityEngine;

public class LockedButtonSwitcher : MonoBehaviour
{
	public GameObject m_label;

	public GameObject m_lockedLabel;

	public GameObject m_lockedIcon;

	public bool m_isLocked;

	private void OnEnable()
	{
		m_label.SetActive(!m_isLocked);
		m_lockedLabel.SetActive(m_isLocked);
		m_lockedIcon.SetActive(m_isLocked);
	}
}
