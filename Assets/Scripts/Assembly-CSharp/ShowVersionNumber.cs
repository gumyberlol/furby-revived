using System.Collections;
using UnityEngine;

public class ShowVersionNumber : MonoBehaviour
{
	public float m_TimeSecondsToWait;

	public AppVersionNumber m_AppVersionNumber;

	private void Start()
	{
		StartCoroutine(InitializeSelf());
	}

	private IEnumerator InitializeSelf()
	{
		yield return new WaitForSeconds(m_TimeSecondsToWait);
		UILabel targetLabel = base.gameObject.GetComponent<UILabel>();
		if ((bool)targetLabel)
		{
			targetLabel.text = m_AppVersionNumber.m_VersionNumberString;
		}
	}
}
