using UnityEngine;

public class AnimHelpers : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_objectsToUpdate;

	public void SetActiveFlagTrue()
	{
		GameObject[] objectsToUpdate = m_objectsToUpdate;
		foreach (GameObject gameObject in objectsToUpdate)
		{
			gameObject.SetActive(true);
		}
	}

	public void SetActiveFlagFalse()
	{
		GameObject[] objectsToUpdate = m_objectsToUpdate;
		foreach (GameObject gameObject in objectsToUpdate)
		{
			gameObject.SetActive(false);
		}
	}
}
