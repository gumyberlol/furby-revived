using System.Collections.Generic;
using UnityEngine;

public class FurblingInfoScreenToggle : MonoBehaviour
{
	public GameObject furblingTagRoot;

	public GameObject furblingInfoScreenRoot;

	public List<GameObject> objectsToDisable;

	public void Toggle()
	{
		furblingTagRoot.SetActive(!furblingTagRoot.activeSelf);
		furblingInfoScreenRoot.SetActive(!furblingTagRoot.activeSelf);
		foreach (GameObject item in objectsToDisable)
		{
			item.SetActive(furblingTagRoot.activeSelf);
		}
	}
}
