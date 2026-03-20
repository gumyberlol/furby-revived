using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

public class IncubatorClockHandler : MonoBehaviour
{
	[GameEventEnum]
	public enum IncubatorClockGameEvent
	{
		IncubatorClockDropsDown = 0,
		IncubatorClockFliesOff = 1,
		IncubatorClockTimeExpires = 2,
		IncubatorClockIdleLoop = 3,
		IncubatorClockInteraction_Variant1 = 4,
		IncubatorClockInteraction_Variant2 = 5
	}

	[Serializable]
	public class AnimationClipAndEvent
	{
		public AnimationClip m_Clip;

		public IncubatorClockGameEvent m_Event;

		public bool m_TriggerVFX;
	}

	[Serializable]
	public class AnimationVFX
	{
		public Animation m_AnimationTarget;

		public AnimationClip m_AnimationOn;

		public AnimationClip m_AnimationOff;
	}

	[Serializable]
	public class ClockAnimations
	{
		public AnimationClip m_AnimEnter;

		public AnimationClip m_AnimExit;

		public AnimationClipAndEvent m_AnimEndOfSequence;

		public List<AnimationClipAndEvent> m_AnimIdles;

		public List<AnimationClipAndEvent> m_AnimInteractions;
	}

	public CameraShakeScript m_CameraShakeScript;

	public ClockAnimations m_Animations;

	public Animation m_AnimationTarget;

	public CapsuleCollider m_Collider;

	public Camera m_ClockCamera;

	public AnimationVFX m_AnimationVFX;

	public AnimationVFX m_EndOfSequenceVFX;

	private int m_IndexOfLastIdleAnimation = -1;

	private int m_IndexOfLastInteractionAnimation = -1;

	private bool m_InteractionAllowed;

	private bool m_ClockIsAnimateable;

	public bool ClockIsAnimateable
	{
		get
		{
			return m_ClockIsAnimateable;
		}
		set
		{
			m_ClockIsAnimateable = value;
		}
	}

	public bool InteractionAllowed
	{
		get
		{
			return m_InteractionAllowed;
		}
		set
		{
			m_InteractionAllowed = value;
		}
	}

	public bool InputDetected { get; set; }

	public float InputContribution { get; set; }

	public float InputDuration { get; set; }

	public void OnEnable()
	{
		FingerGestures.OnFingerDown += OnFingerDown;
		FingerGestures.OnFingerUp += OnFingerUp;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= OnFingerDown;
		FingerGestures.OnFingerUp -= OnFingerUp;
	}

	private void OnFingerDown(int finger, Vector2 origin)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(m_ClockCamera.ScreenPointToRay(origin), out hitInfo))
		{
			InputDetected = hitInfo.collider == m_Collider;
		}
	}

	private void OnFingerUp(int finger, Vector2 origin, float time)
	{
		if (InputDetected)
		{
			InputDetected = false;
			if (time < 0.125f)
			{
				InputContribution = 1f;
				InputDuration = 0f;
			}
			else
			{
				InputContribution = time;
				InputDuration = time;
			}
		}
	}

	public void Awake()
	{
		StopVFX();
		StopEndOfSequenceVFX();
	}

	public void Resume()
	{
		m_AnimationTarget.animation.Stop();
		AnimationClip animEnter = m_Animations.m_AnimEnter;
		m_AnimationTarget.animation.Play(animEnter.name, PlayMode.StopAll);
		m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		GameEventRouter.SendEvent(IncubatorClockGameEvent.IncubatorClockDropsDown);
		TemporarilyPreventIdlingAndInteraction();
	}

	public void Suspend()
	{
		AnimationClip animExit = m_Animations.m_AnimExit;
		m_AnimationTarget.animation.Stop();
		m_AnimationTarget.animation.Play(animExit.name, PlayMode.StopAll);
		m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		GameEventRouter.SendEvent(IncubatorClockGameEvent.IncubatorClockFliesOff);
		PreventIdlingAndInteraction();
	}

	public void TemporarilyPreventIdlingAndInteraction()
	{
		PreventIdlingAndInteraction();
		Invoke("ReenableIdlingAndInteraction", 1f);
	}

	private void PreventIdlingAndInteraction()
	{
		ClockIsAnimateable = false;
		InteractionAllowed = false;
		m_Collider.enabled = false;
	}

	private void ReenableIdlingAndInteraction()
	{
		ClockIsAnimateable = true;
		InteractionAllowed = true;
		m_Collider.enabled = true;
	}

	public void Update()
	{
		if (!ClockIsAnimateable)
		{
			return;
		}
		if (IsAnyAnimationPlaying())
		{
			if (InteractionAllowed && InputDetected)
			{
				SelectAndInvokeAnInteractionAnimation();
				InteractionAllowed = false;
			}
		}
		else
		{
			SelectAndInvokeAnIdleAnimation();
			InteractionAllowed = true;
		}
	}

	private bool IsAnyAnimationPlaying()
	{
		return m_AnimationTarget.animation.isPlaying;
	}

	private void StartVFX()
	{
		m_AnimationVFX.m_AnimationTarget.animation.Stop();
		m_AnimationVFX.m_AnimationTarget.animation.Play(m_AnimationVFX.m_AnimationOn.name, PlayMode.StopAll);
		m_AnimationVFX.m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		Invoke("StopVFX", m_AnimationVFX.m_AnimationOn.length);
	}

	public void StopVFX()
	{
		m_AnimationVFX.m_AnimationTarget.animation.Stop();
		m_AnimationVFX.m_AnimationTarget.animation.Play(m_AnimationVFX.m_AnimationOff.name, PlayMode.StopAll);
		m_AnimationVFX.m_AnimationTarget.animation.wrapMode = WrapMode.Once;
	}

	private void StartEndOfSequenceVFX()
	{
		m_EndOfSequenceVFX.m_AnimationTarget.gameObject.SetActive(true);
		m_EndOfSequenceVFX.m_AnimationTarget.animation.Stop();
		m_EndOfSequenceVFX.m_AnimationTarget.animation.Play(m_EndOfSequenceVFX.m_AnimationOn.name, PlayMode.StopAll);
		m_EndOfSequenceVFX.m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		Invoke("StopEndOfSequenceVFX", 3.25f);
	}

	public void StopEndOfSequenceVFX()
	{
		m_EndOfSequenceVFX.m_AnimationTarget.animation.Stop();
		m_EndOfSequenceVFX.m_AnimationTarget.gameObject.SetActive(false);
	}

	public void SelectAndInvokeAnIdleAnimation()
	{
		int num = UnityEngine.Random.Range(0, m_Animations.m_AnimIdles.Count);
		if (m_Animations.m_AnimIdles.Count > 1)
		{
			while (num == m_IndexOfLastIdleAnimation)
			{
				num = UnityEngine.Random.Range(0, m_Animations.m_AnimIdles.Count);
			}
			m_IndexOfLastIdleAnimation = num;
		}
		AnimationClipAndEvent animationClipAndEvent = m_Animations.m_AnimIdles[num];
		AnimationClip clip = animationClipAndEvent.m_Clip;
		m_AnimationTarget.animation.Stop();
		m_AnimationTarget.animation.Play(clip.name, PlayMode.StopAll);
		m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		GameEventRouter.SendEvent(animationClipAndEvent.m_Event);
	}

	public void SelectAndInvokeAnInteractionAnimation()
	{
		int num = UnityEngine.Random.Range(0, m_Animations.m_AnimInteractions.Count);
		if (m_Animations.m_AnimInteractions.Count > 1)
		{
			while (num == m_IndexOfLastInteractionAnimation)
			{
				num = UnityEngine.Random.Range(0, m_Animations.m_AnimInteractions.Count);
			}
			m_IndexOfLastInteractionAnimation = num;
		}
		AnimationClipAndEvent animationClipAndEvent = m_Animations.m_AnimInteractions[num];
		AnimationClip clip = animationClipAndEvent.m_Clip;
		m_AnimationTarget.CrossFade(clip.name);
		GameEventRouter.SendEvent(animationClipAndEvent.m_Event);
		if (animationClipAndEvent.m_TriggerVFX)
		{
			StartVFX();
		}
	}

	public void ShakeTheCamera()
	{
		m_CameraShakeScript.DoShake(0.15f, 0.0025f);
	}

	public void InvokeEndOfSequenceAnimation()
	{
		StopVFX();
		AnimationClipAndEvent animEndOfSequence = m_Animations.m_AnimEndOfSequence;
		AnimationClip clip = animEndOfSequence.m_Clip;
		m_AnimationTarget.animation.Stop();
		m_AnimationTarget.animation.Play(clip.name, PlayMode.StopAll);
		m_AnimationTarget.animation.wrapMode = WrapMode.Once;
		Invoke("ShakeTheCamera", 2.3f);
		GameEventRouter.SendEvent(IncubatorClockGameEvent.IncubatorClockTimeExpires);
		m_AnimationTarget.animation.PlayQueued(m_Animations.m_AnimExit.name, QueueMode.CompleteOthers, PlayMode.StopSameLayer);
		PreventIdlingAndInteraction();
		if (animEndOfSequence.m_TriggerVFX)
		{
			StartEndOfSequenceVFX();
		}
		Invoke("Hide", clip.length + m_Animations.m_AnimExit.length);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
