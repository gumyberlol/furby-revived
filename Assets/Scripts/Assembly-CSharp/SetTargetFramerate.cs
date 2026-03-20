using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
	public int m_targetFramerate = 60;

	private void Awake()
	{
		Application.targetFrameRate = m_targetFramerate;
	}
}
