using UnityEngine;

public class MoveAndHideScreen : MonoBehaviour
{
	private bool m_loaded;

	private void Awake()
	{
		if (Application.loadedLevelName == "Main")
		{
			base.transform.position = new Vector3(10000f, 10000f, 0f);
		}
	}

	public void OnSubsceneLoaded()
	{
		base.transform.position = Vector3.zero;
	}
}
