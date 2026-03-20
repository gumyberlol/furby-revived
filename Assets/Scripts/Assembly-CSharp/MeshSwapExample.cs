using UnityEngine;

public class MeshSwapExample : MonoBehaviour
{
	public GameObject highPolyObject;

	public GameObject lowPolyObject;

	private void Awake()
	{
		if (Platforms.platform == Platform.iPhone)
		{
			Object.Instantiate(lowPolyObject, base.transform.position, Quaternion.identity);
		}
		else
		{
			Object.Instantiate(highPolyObject, base.transform.position, Quaternion.identity);
		}
	}
}
