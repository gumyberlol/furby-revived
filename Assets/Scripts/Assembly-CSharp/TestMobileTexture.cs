using MMT;
using UnityEngine;

[RequireComponent(typeof(MobileMovieTexture))]
public class TestMobileTexture : MonoBehaviour
{
	private MobileMovieTexture m_movieTexture;

	private void Awake()
	{
		m_movieTexture = GetComponent<MobileMovieTexture>();
		m_movieTexture.onFinished += OnFinished;
	}

	private void OnFinished(MobileMovieTexture sender)
	{
		Debug.Log(sender.Path + " has finished ");
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		float num = (float)m_movieTexture.playPosition;
		float num2 = GUILayout.HorizontalSlider(num, 0f, (float)m_movieTexture.duration);
		if (num2 != num)
		{
			m_movieTexture.playPosition = num2;
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Play"))
		{
			m_movieTexture.Play();
		}
		if (GUILayout.Button("Play/Pause"))
		{
			m_movieTexture.pause = !m_movieTexture.pause;
		}
		if (GUILayout.Button("Stop"))
		{
			m_movieTexture.Stop();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
