using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class FurblingMusicGameAnimation : GameEventReceiver
	{
		[Serializable]
		private class FurblingReaction
		{
			[SerializeField]
			public MusicGameEvent m_eventToReactTo = MusicGameEvent.DiscoModeAvailable;

			[SerializeField]
			public AnimationClip m_animationToPlay;

			[SerializeField]
			public float m_timingOffset;

			[SerializeField]
			public string[] m_mixingTransforms;
		}

		[SerializeField]
		private FurblingReaction[] m_furblingReactions;

		[SerializeField]
		private FurblingReaction m_defaultAnim;

		[SerializeField]
		private BabyInstance m_babyInstance;

		private FurblingReaction m_nextAnimation;

		private MusicGameSongPlayer m_songPlayer;

		private Dictionary<string, float> m_timingOffsets = new Dictionary<string, float>();

		private float m_timeUntilCorrection;

		public override Type EventType
		{
			get
			{
				return typeof(MusicGameEvent);
			}
		}

		private void Start()
		{
			m_songPlayer = (MusicGameSongPlayer)UnityEngine.Object.FindObjectOfType(typeof(MusicGameSongPlayer));
		}

		private void LateUpdate()
		{
			if (!Singleton<MusicGameFabricQuery>.Exists)
			{
				return;
			}
			MusicGameSongData currentSong = m_songPlayer.GetCurrentSong();
			if (!(currentSong != null))
			{
				return;
			}
			float bPM = currentSong.GetBPM();
			float num = 60f / bPM;
			float num2 = num * 4f;
			int currentSongProgressSamples = Singleton<MusicGameFabricQuery>.Instance.GetCurrentSongProgressSamples();
			if (currentSongProgressSamples == -1)
			{
				return;
			}
			float num3 = (float)currentSongProgressSamples / (float)m_songPlayer.GetSampleRate();
			float num4 = num3 / num2 - Mathf.Floor(num3 / num2);
			if (m_nextAnimation != null)
			{
				string key = m_nextAnimation.m_animationToPlay.name;
				m_timingOffsets[key] = m_nextAnimation.m_timingOffset;
				bool flag = false;
				if (m_nextAnimation.m_mixingTransforms.Length == 0 || !flag)
				{
					if (m_babyInstance.Instance.animation[key].wrapMode != WrapMode.Loop)
					{
						if (!m_babyInstance.Instance.animation.IsPlaying(key))
						{
							m_babyInstance.Instance.animation.CrossFade(key, 0.2f);
							Invoke(QueueDefault, m_nextAnimation.m_animationToPlay.length - 0.2f);
						}
					}
					else
					{
						m_babyInstance.Instance.animation.CrossFade(key, 0.2f);
					}
				}
				m_nextAnimation = null;
				m_timeUntilCorrection = 0f;
			}
			m_timeUntilCorrection -= Time.deltaTime;
			if (!(m_timeUntilCorrection < 0f))
			{
				return;
			}
			m_timeUntilCorrection = 0.5f;
			foreach (AnimationState item in m_babyInstance.Instance.animation)
			{
				if ((m_babyInstance.Instance.animation.IsPlaying(item.name) || item.enabled) && item.wrapMode == WrapMode.Loop)
				{
					int num5 = Mathf.RoundToInt(num2 / item.length);
					float value = 0f;
					m_timingOffsets.TryGetValue(item.name, out value);
					float normalizedTime = num4 * (float)num5 + value - Mathf.Floor(num4 * (float)num5 + value);
					item.normalizedTime = normalizedTime;
					item.normalizedSpeed = (float)num5 / num2;
				}
			}
		}

		private void QueueDefault()
		{
			m_nextAnimation = m_defaultAnim;
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			MusicGameEvent musicGameEvent = (MusicGameEvent)(object)enumValue;
			FurblingReaction[] furblingReactions = m_furblingReactions;
			foreach (FurblingReaction furblingReaction in furblingReactions)
			{
				if (furblingReaction.m_eventToReactTo == musicGameEvent)
				{
					m_nextAnimation = furblingReaction;
				}
			}
		}
	}
}
