using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class FabricHierarchyReference
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
	}
}
