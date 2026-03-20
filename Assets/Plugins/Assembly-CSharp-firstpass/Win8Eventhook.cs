using Unibill.Impl;
using UnityEngine;

internal class Win8Eventhook : MonoBehaviour
{
	public Win8_1BillingService callback;

	public void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OnApplicationPause(bool paused)
	{
		if (!paused && callback != null)
		{
			callback.enumerateLicenses();
		}
	}
}
