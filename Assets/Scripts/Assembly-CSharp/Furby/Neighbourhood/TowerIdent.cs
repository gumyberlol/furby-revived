using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Neighbourhood
{
	[Serializable]
	public class TowerIdent
	{
		[SerializeField]
		public FurbyTribeType m_TribeType;

		[SerializeField]
		public GameObject m_TribeWindowRoot;

		[SerializeField]
		public GameObject m_flagRoot;

		[SerializeField]
		private List<GameObject> m_IterationWindows = new List<GameObject>();

		public List<GameObject> IterationWindows
		{
			get
			{
				return m_IterationWindows;
			}
			set
			{
				m_IterationWindows = value;
			}
		}
	}
}
