using System;
using System.IO;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ResourcesReference
	{
		private GameObject m_override;

		[SerializeField]
		private string m_path;

		public string Path
		{
			get
			{
				return m_path;
			}
			set
			{
				m_path = value;
			}
		}

		public string SceneName
		{
			get
			{
				return System.IO.Path.GetFileNameWithoutExtension(Path);
			}
		}

		public UnityEngine.Object Load()
		{
			return Resources.Load(m_path);
		}

		public void SetOverride(GameObject overrideObject)
		{
			m_override = overrideObject;
		}

		public GameObject LoadAsGameObject()
		{
			if (m_override != null)
			{
				return m_override;
			}
			return (GameObject)Load();
		}
	}
}
