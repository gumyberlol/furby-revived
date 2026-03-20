using Fabric;
using UnityEngine;

public class AddRemovePreset : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			EventManager.Instance.PostEvent("DynamicMixer", EventAction.AddPreset, "MuteAll", null);
			Debug.Log("Event: Add Preset");
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			EventManager.Instance.PostEvent("DynamicMixer", EventAction.RemovePreset, "MuteAll", null);
			Debug.Log("Event: Remove Preset");
		}
	}
}
