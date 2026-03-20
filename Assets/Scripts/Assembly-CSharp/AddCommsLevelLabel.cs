using System.Collections;
using Furby;
using Relentless;
using UnityEngine;

public class AddCommsLevelLabel : MonoBehaviour
{
	private UILabel m_UILabel;

	public string m_LocalisedStringKey = string.Empty;

	[SerializeField]
	private string m_CachedTranslation = string.Empty;

	private void Awake()
	{
		m_UILabel = GetComponent<UILabel>();
		StartCoroutine(InterceptAndPresentLevelChanges());
	}

	public void OnEnable()
	{
		CacheTranslation();
		m_UILabel.text = m_CachedTranslation;
	}

	public void Start()
	{
		CacheTranslation();
		m_UILabel.text = m_CachedTranslation;
	}

	private void UpdateLabelWithValue(float rawValue)
	{
		if (m_CachedTranslation == string.Empty)
		{
			CacheTranslation();
		}
		if (m_UILabel != null)
		{
			string text = (rawValue * 100f).ToString("N0") + "%";
			m_UILabel.text = m_CachedTranslation + " - " + text;
		}
	}

	private void CacheTranslation()
	{
		m_CachedTranslation = Singleton<Localisation>.Instance.GetText(m_LocalisedStringKey);
	}

	private IEnumerator InterceptAndPresentLevelChanges()
	{
		WaitForGameEvent waiter = new WaitForGameEvent();
		while (true)
		{
			yield return StartCoroutine(waiter.WaitForEvent(SettingsPageEvents.CommsSliderValueChanged));
			float rawValue = (float)waiter.ReturnedParameters[0];
			UpdateLabelWithValue(rawValue);
		}
	}
}
