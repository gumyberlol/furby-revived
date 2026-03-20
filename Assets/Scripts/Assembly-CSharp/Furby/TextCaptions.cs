using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class TextCaptions : MonoBehaviour
	{
		public GameObject m_TextLayer;

		[SerializeField]
		public CaptionSequence m_CaptionSequence = new CaptionSequence();

		public float m_MaxVideoDurationSeconds = 60f;

		private GameEventSubscription m_EventSubscription;

		private bool m_UpdateCaptions;

		private float m_TimeVideoStarted;

		private int m_LastCaptionIndex = -1;

		public Color m_TextColor = default(Color);

		private void Awake()
		{
			ActivateSubtitlesOnceVideoStarts();
		}

		private void OnEnable()
		{
			ActivateSubtitlesOnceVideoStarts();
		}

		private void ActivateSubtitlesOnceVideoStarts()
		{
			m_EventSubscription = new GameEventSubscription(typeof(VideoPlayerGameEvents), OnHandleVideoPlayerGameEvents);
			if ((bool)m_TextLayer)
			{
				m_TextLayer.SetActive(false);
			}
		}

		private void OnDisable()
		{
			m_EventSubscription.Dispose();
			StopSubtitles();
		}

		private void OnHandleVideoPlayerGameEvents(Enum evtType, GameObject originator, params object[] parameters)
		{
			VideoPlayerGameEvents videoPlayerGameEvents = (VideoPlayerGameEvents)(object)evtType;
			if (m_TextLayer != null)
			{
				if (videoPlayerGameEvents == VideoPlayerGameEvents.VideoHasStarted)
				{
					StartSubtitles();
				}
				else
				{
					StopSubtitles();
				}
			}
		}

		public void ActivateManually()
		{
			OnDisable();
			StartSubtitles();
		}

		private void StopSubtitles()
		{
			m_UpdateCaptions = false;
			if ((bool)m_TextLayer)
			{
				m_TextLayer.SetActive(false);
			}
		}

		private void StartSubtitles()
		{
			m_TimeVideoStarted = Time.realtimeSinceStartup;
			m_UpdateCaptions = true;
			if ((bool)m_TextLayer)
			{
				Helper_UpdateText(string.Empty);
			}
		}

		private void Update()
		{
			if (!m_UpdateCaptions)
			{
				return;
			}
			float timeStamp = Time.realtimeSinceStartup - m_TimeVideoStarted;
			int appropriateCaptionIndex = m_CaptionSequence.GetAppropriateCaptionIndex(timeStamp);
			if (appropriateCaptionIndex == m_LastCaptionIndex)
			{
				return;
			}
			Caption caption = m_CaptionSequence.GetCaption(appropriateCaptionIndex);
			if (caption == null)
			{
				Helper_UpdateText(string.Empty);
			}
			else
			{
				if ((bool)m_TextLayer)
				{
					m_TextLayer.SetActive(true);
				}
				Helper_UpdateText(Singleton<Localisation>.Instance.GetText(caption.m_LocalizedKey));
			}
			m_LastCaptionIndex = appropriateCaptionIndex;
		}

		private void Helper_UpdateText(string text)
		{
			UILabel componentInChildren = m_TextLayer.GetComponentInChildren<UILabel>();
			if (componentInChildren != null && componentInChildren.text != text)
			{
				componentInChildren.color = m_TextColor;
				componentInChildren.text = text;
			}
		}
	}
}
