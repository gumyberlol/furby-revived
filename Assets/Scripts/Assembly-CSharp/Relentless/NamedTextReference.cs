using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class NamedTextReference
	{
		[SerializeField]
		private string m_localisedStringKey;

		public string Key
		{
			get
			{
				return m_localisedStringKey;
			}
			set
			{
				m_localisedStringKey = value;
			}
		}

		public string NamedText
		{
			get
			{
				return Singleton<Localisation>.Instance.GetText(m_localisedStringKey);
			}
		}
	}
}
