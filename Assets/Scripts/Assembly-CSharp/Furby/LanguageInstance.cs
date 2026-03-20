using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class LanguageInstance
	{
		[SerializeField]
		public Locale m_Locale;

		[SerializeField]
		public string m_SpriteName;
	}
}
