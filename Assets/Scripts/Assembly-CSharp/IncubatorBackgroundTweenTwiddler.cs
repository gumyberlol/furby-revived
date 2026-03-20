using UnityEngine;

public class IncubatorBackgroundTweenTwiddler : MonoBehaviour
{
	public TweenRotation m_TweenerScript;

	public Quaternion m_CachedRotation;

	public float m_DurationSecs = 3.3f;

	private void Awake()
	{
		CacheRotation();
	}

	public void EnableRotation()
	{
		InvokeEaseIn();
		Invoke("InvokeLoop", m_DurationSecs);
	}

	public void DisableRotation_WithEaseOut()
	{
		InvokeEaseOut();
	}

	public void DisableRotation()
	{
		m_TweenerScript.enabled = false;
		ReassertRotation();
	}

	private void CacheRotation()
	{
		m_CachedRotation = base.transform.rotation;
	}

	private void ReassertRotation()
	{
		base.transform.rotation = m_CachedRotation;
	}

	private void InvokeEaseIn()
	{
		ReassertRotation();
		m_TweenerScript.enabled = false;
		m_TweenerScript.from = new Vector3(90f, 0f, 0f);
		m_TweenerScript.to = new Vector3(90f, 180f, 0f);
		m_TweenerScript.duration = m_DurationSecs;
		m_TweenerScript.method = UITweener.Method.EaseIn;
		m_TweenerScript.style = UITweener.Style.Once;
		m_TweenerScript.enabled = true;
	}

	private void InvokeLoop()
	{
		ReassertRotation();
		m_TweenerScript.enabled = true;
		m_TweenerScript.from = new Vector3(90f, 0f, 0f);
		m_TweenerScript.to = new Vector3(90f, 180f, 0f);
		m_TweenerScript.duration = m_DurationSecs / 2f;
		m_TweenerScript.method = UITweener.Method.Linear;
		m_TweenerScript.style = UITweener.Style.Loop;
		m_TweenerScript.enabled = true;
	}

	private void InvokeEaseOut()
	{
		ReassertRotation();
		m_TweenerScript.enabled = false;
		m_TweenerScript.from = new Vector3(90f, 0f, 0f);
		m_TweenerScript.to = new Vector3(90f, 180f, 0f);
		m_TweenerScript.duration = m_DurationSecs;
		m_TweenerScript.method = UITweener.Method.EaseOut;
		m_TweenerScript.style = UITweener.Style.Once;
		m_TweenerScript.enabled = true;
	}
}
