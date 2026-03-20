using System;
using UnityEngine;

namespace Relentless
{
	public class DisplayAsMaskDropdown : PropertyAttribute
	{
		private Type m_flagEnum;

		public Type FlagEnumType
		{
			get
			{
				return m_flagEnum;
			}
		}

		public DisplayAsMaskDropdown(Type flagEnum)
		{
			m_flagEnum = flagEnum;
		}
	}
}
