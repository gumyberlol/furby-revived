using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class DefaultGameConfigs
	{
		[SerializeField]
		public string m_AndroidGooglePlay = string.Empty;

		[SerializeField]
		public string m_AndroidAmazon = string.Empty;

		[SerializeField]
		public string m_iOSAppStore = string.Empty;

		[SerializeField]
		public string m_Fallback = string.Empty;
	}
}
