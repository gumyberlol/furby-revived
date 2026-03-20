using Relentless;
using UnityEngine;

public class OnClickLaunchURL : MonoBehaviour
{
	[SerializeField]
	public string m_URL = string.Empty;

	private void OnClick()
	{
		Logging.Log("OnClickLaunchURL::Received OnClick() -> URL: " + m_URL.ToString());
		if (m_URL != string.Empty)
		{
			Application.OpenURL(m_URL);
		}
	}
}
