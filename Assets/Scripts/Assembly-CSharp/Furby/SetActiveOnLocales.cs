using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SetActiveOnLocales : MonoBehaviour
	{
		public List<Locale> m_Locales = new List<Locale>();

		public bool m_Show;

		private void Start()
		{
			Locale currentLocale = Singleton<Localisation>.Instance.CurrentLocale;
			if (m_Locales.Contains(currentLocale))
			{
				base.gameObject.SetActive(m_Show);
			}
			else
			{
				base.gameObject.SetActive(!m_Show);
			}
		}
	}
}
