using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class PrefabData
	{
		[Serializable]
		public class PropertyModification
		{
			public string m_path;

			public UnityEngine.Object m_objectRef;

			public string m_value;

			public UnityEngine.Object m_target;
		}

		[SerializeField]
		public UnityEngine.Object m_prefab;

		[SerializeField]
		public string m_nodePath;

		[SerializeField]
		public PropertyModification[] m_propertyModifications;
	}
}
