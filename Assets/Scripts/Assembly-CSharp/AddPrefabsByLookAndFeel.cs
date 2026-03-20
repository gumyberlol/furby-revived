using System.Collections.Generic;
using UnityEngine;

public class AddPrefabsByLookAndFeel : ChangeLookAndFeel
{
	public List<GameObject> m_prefabs;

	protected override void OnChangeTheme()
	{
		foreach (GameObject prefab in m_prefabs)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(prefab);
			Vector3 localPosition = gameObject.transform.localPosition;
			Vector3 localScale = gameObject.transform.localScale;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localScale = localScale;
		}
	}
}
