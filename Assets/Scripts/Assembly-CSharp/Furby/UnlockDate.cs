using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class UnlockDate
	{
		[SerializeField]
		private int m_UnlockYear = 1970;

		[SerializeField]
		private int m_UnlockMonth = 1;

		[SerializeField]
		private int m_UnlockDay = 1;

		public DateTime UnlockDateTime
		{
			get
			{
				return new DateTime(m_UnlockYear, m_UnlockMonth, m_UnlockDay);
			}
		}
	}
}
