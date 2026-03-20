using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class Caption
	{
		[SerializeField]
		public float m_TimeStamp_Start;

		[SerializeField]
		public float m_TimeStamp_End;

		[SerializeField]
		public string m_LocalizedKey;
	}
}
