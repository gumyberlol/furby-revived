using Fabric;
using UnityEngine;

public class InitialiseParameters : MonoBehaviour
{
	private Fabric.InitialiseParameters parameters = new Fabric.InitialiseParameters();

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			parameters._delaySamples.Value = 50000;
			EventManager.Instance.PostEvent("Simple", EventAction.PlaySound, null, base.gameObject, parameters);
		}
	}
}
