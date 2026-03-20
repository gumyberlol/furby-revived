using Unibill.Impl;
using UnityEngine;

internal class WP8Eventhook : MonoBehaviour
{
	public WP8BillingService callback;

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
