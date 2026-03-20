using Fabric;
using UnityEngine;

public class ChangeVolume : MonoBehaviour
{
	private void Start()
	{
		EventManager.Instance.PostEvent("TestEvent1", base.gameObject);
		EventManager.Instance.PostEvent("TestEvent2", base.gameObject);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			EventManager.Instance.PostEvent("TestEvent1", EventAction.SetVolume, 1f, base.gameObject);
			Debug.Log("Change Group Component to volume: 1.0");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			EventManager.Instance.PostEvent("TestEvent1", EventAction.SetVolume, 0.2f, base.gameObject);
			Debug.Log("Change Group Component to volume: 0.2");
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			EventManager.Instance.PostEvent("TestEvent2", EventAction.SetVolume, 1f, base.gameObject);
			Debug.Log("Change Audio Component to volume: 1.0");
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			EventManager.Instance.PostEvent("TestEvent2", EventAction.SetVolume, 0.2f, base.gameObject);
			Debug.Log("Change Audio Component to volume: 0.2");
		}
	}
}
