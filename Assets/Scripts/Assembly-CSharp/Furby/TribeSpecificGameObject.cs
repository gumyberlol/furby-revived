using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class TribeSpecificGameObject
	{
		[SerializeField]
		public GameObject m_TargetGo;

		[SerializeField]
		public Tribeset m_Tribe;
	}
}
