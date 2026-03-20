using Relentless;
using UnityEngine;

public class EnableComponentIfObjectExists : RelentlessMonoBehaviour
{
	public string m_objectName;

	public MonoBehaviour m_targetComponent;

	public bool m_onExists;

	public bool m_onDoesNotExist;

	private void Awake()
	{
		GameObject gameObject = GameObject.Find(m_objectName);
		if ((bool)gameObject)
		{
			m_targetComponent.enabled = m_onExists;
		}
		else
		{
			m_targetComponent.enabled = m_onDoesNotExist;
		}
	}
}
