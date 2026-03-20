using UnityEngine;

public class DestroyObject : MonoBehaviour
{
	public bool destroy;

	private void Start()
	{
	}

	private void OnCollisionEnter()
	{
		Object.DestroyObject(base.gameObject);
	}

	private void Update()
	{
		if (destroy)
		{
			Object.DestroyObject(base.gameObject);
			destroy = false;
		}
	}
}
