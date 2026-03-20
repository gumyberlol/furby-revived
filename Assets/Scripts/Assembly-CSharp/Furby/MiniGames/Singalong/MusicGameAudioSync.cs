using System.Collections;
using System.Collections.Generic;
using Fabric;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.Singalong
{
	public class MusicGameAudioSync : MonoBehaviour
	{
		private List<AudioSource> m_audioSources = new List<AudioSource>();

		private Dictionary<AudioClip, AudioComponent> m_lookUpTable = new Dictionary<AudioClip, AudioComponent>();

		private void Start()
		{
			AudioComponent[] componentsInChildren = GetComponentsInChildren<AudioComponent>(true);
			AudioComponent[] array = componentsInChildren;
			foreach (AudioComponent audioComponent in array)
			{
				m_lookUpTable[audioComponent.AudioClip] = audioComponent;
			}
			AudioSource[] componentsInChildren2 = EventManager.Instance.GetComponentsInChildren<AudioSource>(true);
			AudioSource[] array2 = componentsInChildren2;
			foreach (AudioSource audioSource in array2)
			{
				if (audioSource.name != "AudioSourcesToLoad")
				{
					m_audioSources.Add(audioSource);
				}
			}
			Logging.Log("Audio sources found: " + m_audioSources.Count);
		}

		private float GetTimeInToBar(float bpm)
		{
			MusicGameFabricQuery instance = Singleton<MusicGameFabricQuery>.Instance;
			if (instance != null)
			{
				float num = (float)instance.GetCurrentSongProgressSamples() / (float)instance.GetSampleRate();
				float num2 = 60f / bpm * 4f;
				float num3 = num / num2 - Mathf.Floor(num / num2);
				return num3 * num2;
			}
			return 0f;
		}

		public void ReSync(float bpm)
		{
			StartCoroutine(ReSyncCo(bpm));
		}

		private IEnumerator ReSyncCo(float bpm)
		{
			float tempTimeOffset = GetTimeInToBar(bpm);
			float timeOffset = tempTimeOffset;
			int spinCounter = 0;
			while (true)
			{
				timeOffset = GetTimeInToBar(bpm);
				if (timeOffset != tempTimeOffset)
				{
					break;
				}
				yield return null;
				spinCounter++;
			}
			yield return null;
			timeOffset += Time.deltaTime;
			foreach (AudioSource aSource in m_audioSources)
			{
				AudioComponent matchingComp = null;
				m_lookUpTable.TryGetValue(aSource.clip, out matchingComp);
				if (matchingComp != null && matchingComp.IsPlaying())
				{
					Logging.Log("!!!! Re-syncing audio to " + timeOffset);
					aSource.timeSamples = (int)(timeOffset * (float)aSource.clip.frequency) % aSource.clip.samples;
				}
			}
		}
	}
}
