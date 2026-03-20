using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FlairCollection : ScriptableObject
	{
		[SerializeField]
		[EasyEditArray]
		private Flair[] m_flairs;

		public ICollection<Flair> Flairs
		{
			get
			{
				return m_flairs;
			}
		}
	}
}
