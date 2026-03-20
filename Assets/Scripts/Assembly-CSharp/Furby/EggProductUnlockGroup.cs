using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class EggProductUnlockGroup
	{
		public string m_ScannableQRCode = string.Empty;

		[SerializeField]
		public string[] m_UnlockableQRCodes = new string[3];
	}
}
