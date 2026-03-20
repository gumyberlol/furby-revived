using Fabric;
using UnityEngine;

public class StopAll : MonoBehaviour
{
	public bool stopAll;

	private void Start()
	{
	}

	private void Update()
	{
		if (stopAll)
		{
			Fabric.Component component = FabricManager.Instance.GetComponentByName("Audio_Group1") as GroupComponent;
			component.Stop(true, false);
			stopAll = false;
		}
	}
}
