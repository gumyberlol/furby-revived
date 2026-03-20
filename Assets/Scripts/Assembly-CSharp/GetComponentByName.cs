using Fabric;
using UnityEngine;

public class GetComponentByName : MonoBehaviour
{
	private AudioComponent component;

	private void Start()
	{
	}

	private void Update()
	{
		if (component == null)
		{
			component = FabricManager.Instance.GetComponentByName("Audio_Fabric_SFX_Test") as AudioComponent;
			component.Volume = 0f;
		}
	}
}
