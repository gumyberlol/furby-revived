using Relentless;
using UnityEngine;

public class ObjectReferences : MonoBehaviour
{
	public SerializableDictionary<string, GameObject> m_references;

	private void Start()
	{
		if (m_references == null)
		{
			m_references = new SerializableDictionary<string, GameObject>();
		}
	}
}
