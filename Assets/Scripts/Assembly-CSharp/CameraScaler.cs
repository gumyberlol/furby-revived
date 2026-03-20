using System.Collections;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
	public int MinHeight = 960;

	public int MaxHeight = 1024;

	private int mScreenHeight;

	private void Awake()
	{
		Resize();
	}

	private IEnumerator Start()
	{
		for (int i = 0; i < 10; i++)
		{
			yield return new WaitForFixedUpdate();
			if (Screen.height != mScreenHeight)
			{
				Resize();
			}
		}
	}

	private void Resize()
	{
		Camera camera = base.camera;
		mScreenHeight = Screen.height;
		camera.orthographicSize = (float)Mathf.Min(Mathf.Max(Screen.height, MinHeight), MaxHeight) / 2f;
	}
}
