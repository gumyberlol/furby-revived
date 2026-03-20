using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/VolumeMeter Modified")]
	public class ModifiedVolumeMeter : MonoBehaviour
	{
		[HideInInspector]
		public List<AudioComponent> sources = new List<AudioComponent>();

		[HideInInspector]
		public VolumeMeterState volumeMeterState = new VolumeMeterState();

		[HideInInspector]
		public bool _isInitialised;

		[HideInInspector]
		public bool _is3D = true;

		[HideInInspector]
		public CodeProfiler profiler = new CodeProfiler();

		private float[,] samples;

		private float[] tempSamples;

		private AudioListener listener;

		public void Start()
		{
			for (int i = 0; i < 5; i++)
			{
				volumeMeterState.mHistory[i] = new VolumeMeterState.stSpeakers();
			}
			samples = new float[2, 256];
			tempSamples = new float[256];
			listener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
			CollectAudioSources();
		}

		public void CollectAudioSources()
		{
			sources.Clear();
			AudioComponent[] componentsInChildren = base.gameObject.GetComponentsInChildren<AudioComponent>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				sources.Add(componentsInChildren[i]);
			}
		}

		private float distanceAttenuation(float distance, float minDistance, float maxDistance, AudioRolloffMode rolloffMode)
		{
			if (distance <= minDistance)
			{
				return 1f;
			}
			if (distance > maxDistance)
			{
				distance = maxDistance;
			}
			switch (rolloffMode)
			{
			case AudioRolloffMode.Custom:
				return 1f;
			case AudioRolloffMode.Linear:
			{
				float num = ((!(minDistance < maxDistance)) ? 1f : ((maxDistance - distance) / (maxDistance - minDistance)));
				return (rolloffMode != AudioRolloffMode.Linear) ? (num * num) : num;
			}
			default:
				return (!(distance > 0f)) ? 1f : (minDistance / scaledRolloffDistance(distance, minDistance, maxDistance));
			}
		}

		private float scaledRolloffDistance(float distance, float minDistance, float maxDistance)
		{
			float num = 1f;
			return (!(distance > minDistance) || num == 1f) ? distance : ((distance - minDistance) * num + minDistance);
		}

		private void VolumeMeterProcess(ref VolumeMeterState outState)
		{
			VolumeMeterState.stSpeakers stSpeakers = outState.mHistory[outState.mHistoryIndex];
			outState.mHistoryIndex++;
			outState.mHistoryIndex %= 5;
			stSpeakers.Clear();
			int length = samples.Length;
			float num = 0f;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					float num2 = samples[i, j];
					float b = ((!(num2 < 0f)) ? num2 : (0f - num2));
					float a = stSpeakers.mChannels[i];
					stSpeakers.mChannels[i] = Mathf.Max(a, b);
					num += num2 * num2;
				}
			}
			if (length > 0)
			{
				num /= 256f;
				num = Mathf.Sqrt(num);
				num = Mathf.Max(num, 0f);
				num = Mathf.Min(num, 1f);
			}
			stSpeakers.mRMS = num;
			outState.mPeaks.Clear();
			outState.mRMS = 0f;
			for (int k = 0; k < 5; k++)
			{
				VolumeMeterState.stSpeakers stSpeakers2 = outState.mHistory[k];
				outState.mRMS += stSpeakers2.mRMS;
				for (int l = 0; l < 2; l++)
				{
					outState.mPeaks.mChannels[l] += stSpeakers2.mChannels[l];
				}
			}
			float num3 = 0.2f;
			outState.mRMS *= num3;
			for (int m = 0; m < 2; m++)
			{
				outState.mPeaks.mChannels[m] *= num3;
			}
		}

		public void Update()
		{
			profiler.Begin();
			Array.Clear(samples, 0, samples.Length);
			for (int i = 0; i < sources.Count; i++)
			{
				AudioSource audioSource = sources[i].AudioSource;
				if (!(audioSource != null) || !audioSource.isPlaying)
				{
					continue;
				}
				float num = 0f;
				if (_is3D)
				{
					float distance = 0f;
					if (listener != null)
					{
						distance = (listener.transform.position - audioSource.transform.position).magnitude;
					}
					num = distanceAttenuation(distance, audioSource.minDistance, audioSource.maxDistance, audioSource.rolloffMode) * audioSource.volume;
				}
				else
				{
					num = audioSource.volume;
				}
				for (int j = 0; j < 2; j++)
				{
					audioSource.GetOutputData(tempSamples, j);
					for (int k = 0; k < 256; k++)
					{
						samples[j, k] += tempSamples[k] * num;
					}
				}
			}
			VolumeMeterProcess(ref volumeMeterState);
			profiler.End();
		}
	}
}
