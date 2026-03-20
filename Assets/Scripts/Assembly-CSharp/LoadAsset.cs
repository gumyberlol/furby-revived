using Fabric;
using UnityEngine;

public class LoadAsset : MonoBehaviour
{
	public string asset;

	public string targetComponent;

	public bool loadAsset;

	public bool unloadAsset;

	private void Start()
	{
	}

	private void Update()
	{
		if (loadAsset)
		{
			FabricManager.Instance.LoadAsset(asset, targetComponent);
			loadAsset = false;
		}
		if (unloadAsset)
		{
			FabricManager.Instance.UnloadAsset(targetComponent + "_" + asset);
			unloadAsset = false;
		}
	}
}
