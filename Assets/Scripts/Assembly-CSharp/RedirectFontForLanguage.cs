using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

public class RedirectFontForLanguage : MonoBehaviour
{
	[Serializable]
	private class Redirection
	{
		public Locale language;

		public string fontName;
	}

	[SerializeField]
	private UIFont m_target;

	[SerializeField]
	private string m_default;

	[SerializeField]
	private List<Redirection> m_redirections;

	private Localisation m_localisation;

	private EventHandler m_langChangeListener;

	public void Start()
	{
		Localisation localisation = Singleton<Localisation>.Instance;
		m_localisation = localisation;
		m_langChangeListener = delegate
		{
			RedirectForLanguage(localisation.CurrentLocale);
		};
		m_localisation.LocaleSet += m_langChangeListener;
		m_langChangeListener(localisation, EventArgs.Empty);
	}

	public void OnDestroy()
	{
		m_localisation.LocaleSet -= m_langChangeListener;
	}

	private void RedirectForLanguage(Locale lang)
	{
		Redirection redirection = m_redirections.Find((Redirection x) => x.language == lang);
		string text = ((redirection == null) ? m_default : redirection.fontName);
		text = "Fonts/" + text;
		GameObject gameObject = Resources.Load(text, typeof(GameObject)) as GameObject;
		UIFont uIFont = ((!(gameObject != null)) ? null : gameObject.GetComponent<UIFont>());
		if (uIFont == null)
		{
			throw new ApplicationException(string.Format("Failed to load font \"{0}\" for Locale \"{1}\"", text, lang.ToString()));
		}
		m_target.replacement = uIFont;
		Resources.UnloadUnusedAssets();
	}
}
