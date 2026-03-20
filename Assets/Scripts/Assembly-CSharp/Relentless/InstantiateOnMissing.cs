using UnityEngine;

namespace Relentless
{
	public class InstantiateOnMissing : MonoBehaviour
	{
		public enum ReparentType
		{
			None = 0,
			This = 1,
			Parent = 2
		}

		[SerializeField]
		private string m_objectName;

		[SerializeField]
		private GameObject m_prefab;

		[SerializeField]
		private ReparentType m_reparentTo;

		[SerializeField]
		private bool m_keepTransforms;

		private void Awake()
		{
			string text = ((!(m_objectName == string.Empty)) ? m_objectName : m_prefab.name);
			string text2 = string.Empty;
			switch (m_reparentTo)
			{
			case ReparentType.None:
				text2 = "/";
				break;
			case ReparentType.This:
				text2 = base.transform.GetPath();
				break;
			case ReparentType.Parent:
				text2 = base.transform.parent.GetPath();
				break;
			}
			GameObject gameObject = GameObject.Find(text2 + text);
			if (!gameObject)
			{
				gameObject = ((!m_keepTransforms) ? ((GameObject)Object.Instantiate(m_prefab, base.transform.position, base.transform.rotation)) : ((GameObject)Object.Instantiate(m_prefab)));
				gameObject.name = text;
				Vector3 position = gameObject.transform.position;
				Quaternion rotation = gameObject.transform.rotation;
				switch (m_reparentTo)
				{
				case ReparentType.This:
					gameObject.transform.parent = base.transform;
					break;
				case ReparentType.Parent:
					gameObject.transform.parent = base.transform.parent;
					break;
				}
				if (m_keepTransforms)
				{
					gameObject.transform.position = position;
					gameObject.transform.rotation = rotation;
				}
			}
		}
	}
}
