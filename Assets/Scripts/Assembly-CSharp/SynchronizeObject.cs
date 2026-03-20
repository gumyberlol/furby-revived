using UnityEngine;

public class SynchronizeObject : MonoBehaviour
{
	public enum SynchronizeType
	{
		Match = 0,
		Oppose = 1
	}

	public enum ReferenceType
	{
		ObjectReference = 0,
		ByName = 1
	}

	public string m_ObjectName = string.Empty;

	public GameObject m_Object;

	public SynchronizeType m_SynchronizeType;

	public ReferenceType m_ReferenceType;

	private GameObject m_CachedObject;

	public GameObject CachedObject
	{
		get
		{
			if (m_CachedObject == null)
			{
				switch (m_ReferenceType)
				{
				case ReferenceType.ObjectReference:
					m_CachedObject = m_Object;
					break;
				case ReferenceType.ByName:
					m_CachedObject = GameObject.Find(m_ObjectName);
					break;
				}
			}
			return m_CachedObject;
		}
	}

	private void OnEnable()
	{
		ActionTarget(true);
	}

	private void OnDisable()
	{
		ActionTarget(false);
	}

	private void ActionTarget(bool forwards)
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		switch (m_SynchronizeType)
		{
		case SynchronizeType.Match:
			CachedObject.SetActive(forwards && activeInHierarchy);
			break;
		case SynchronizeType.Oppose:
			CachedObject.SetActive(!forwards || !activeInHierarchy);
			break;
		}
	}
}
