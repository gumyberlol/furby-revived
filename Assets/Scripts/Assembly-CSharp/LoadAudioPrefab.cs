using UnityEngine;

public class LoadAudioPrefab : MonoBehaviour
{
	public string _audioPrefabName = "Audio";

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Audio");
		if (gameObject == null)
		{
			gameObject = Resources.Load(_audioPrefabName) as GameObject;
			if (gameObject != null)
			{
				Object.Instantiate(gameObject);
			}
			else
			{
				Debug.LogError("Audio prefab not available");
			}
		}
	}

	private void Update()
	{
	}
}
