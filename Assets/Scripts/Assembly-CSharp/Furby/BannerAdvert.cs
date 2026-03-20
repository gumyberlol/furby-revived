using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class BannerAdvert
	{
		[SerializeField]
		public string m_BannerURL = string.Empty;

		[SerializeField]
		public string m_BannerIdent = string.Empty;

		[SerializeField]
		public float m_ScaleFactor = 1f;
	}
}
