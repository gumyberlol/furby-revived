using System;
using System.Collections;
using UnityEngine;

public class ArriveAndDisperse : MonoBehaviour
{
	public delegate void Handler();

	[SerializeField]
	private Animation m_anim;

	[SerializeField]
	private AnimationClip m_arrivalClip;

	[SerializeField]
	private AnimationClip m_idleClip;

	[SerializeField]
	private AnimationClip m_dispersalClip;

	private bool m_arrived;

	private bool m_dispersalRequested;

	public bool HasArrived
	{
		get
		{
			return m_arrived;
		}
	}

	public event Handler ArrivalStarted;

	public event Handler ArrivalCompleted;

	public event Handler DispersalStarted;

	public event Handler DispersalCompleted;

	public void SetAnim(Animation anim)
	{
		if (anim == null)
		{
			throw new ApplicationException(string.Format("{0} can't have a SetAnim call with null", base.gameObject.name));
		}
		m_anim = anim;
	}

	public void Arrive()
	{
		StartCoroutine(ArrivalFlow());
	}

	public void DisperseWhenReady()
	{
		m_dispersalRequested = true;
	}

	public void CancelDispersalRequest()
	{
		if (HasArrived)
		{
			throw new ApplicationException("Dispersal cannot be cancelled once started.");
		}
		m_dispersalRequested = false;
	}

	private IEnumerator ArrivalFlow()
	{
		while (m_anim == null)
		{
			yield return null;
		}
		if (this.ArrivalStarted != null)
		{
			this.ArrivalStarted();
		}
		PlayAndCheck(m_anim, m_arrivalClip);
		while (m_anim.IsPlaying(m_arrivalClip.name))
		{
			yield return null;
		}
		m_arrived = true;
		if (this.ArrivalCompleted != null)
		{
			this.ArrivalCompleted();
		}
		if (m_idleClip != null)
		{
			PlayAndCheck(m_anim, m_idleClip);
		}
		while (!m_dispersalRequested)
		{
			yield return null;
		}
		if (this.DispersalStarted != null)
		{
			this.DispersalStarted();
		}
		PlayAndCheck(m_anim, m_dispersalClip);
		while (m_anim.IsPlaying(m_dispersalClip.name))
		{
			yield return null;
		}
		if (this.DispersalCompleted != null)
		{
			this.DispersalCompleted();
		}
	}

	private static void PlayAndCheck(Animation anim, AnimationClip clip)
	{
		if (!anim.Play(clip.name))
		{
			throw new ApplicationException(string.Format("Failed to trigger anim clip {0} on object {1}", clip.name, anim.name));
		}
	}
}
