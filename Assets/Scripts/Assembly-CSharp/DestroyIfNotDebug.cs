using UnityEngine;

public class DestroyIfNotDebug : MonoBehaviour
{
	private void Start()
	{
		if (!Debug.isDebugBuild)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
