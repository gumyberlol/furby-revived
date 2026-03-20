using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class AllSelectableFeatures
	{
		[SerializeField]
		public List<SelectableFeature> m_Features;

		public List<SelectableFeature> Features
		{
			get
			{
				return m_Features;
			}
			set
			{
				m_Features = value;
			}
		}

		public int Count()
		{
			return Features.Count;
		}
	}
}
