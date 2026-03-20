using Relentless;
using UnityEngine;

public class FurbishTranslatorOscilloscope : MonoBehaviour
{
	public enum MicrophoneMode
	{
		DisableComAirAndAccessMicrophoneDirectly = 0,
		WorkWithComAirViaIOSSpecificPlugin = 1
	}

	public LineRenderer m_lineRenderer;

	private int m_numSamples = 128;

	private float[] m_samples;

	private int m_micPosition;

	public int m_numLineRendererPositions = 32;

	public float m_volumeMultiplier = 40f;

	private AudioClip m_audioClip;

	private float m_volume = 1f;

	private float m_smoothVolume;

	private float m_currentVolume;

	private float m_currentVolumeVelocity;

	public float m_volumeSmoothTime = 0.25f;

	public AnimationCurve m_waveCurve;

	private float m_maxHeight;

	private float m_widthInterval;

	public Transform m_startPosTransform;

	public Transform m_endPosTransform;

	public Transform m_maxHeightTransform;

	private int m_FrequencyMinimum;

	private int m_FrequencyMaximum;

	public string m_CachedDeviceName = string.Empty;

	public MicrophoneMode m_MicrophoneMode = MicrophoneMode.WorkWithComAirViaIOSSpecificPlugin;

	private void Start()
	{
		InitializeMicrophoneSampler();
		InitializeMicrophoneListener();
	}

	private void OnDestroy()
	{
		MicrophoneMode microphoneMode = m_MicrophoneMode;
		if (microphoneMode != MicrophoneMode.DisableComAirAndAccessMicrophoneDirectly && microphoneMode == MicrophoneMode.WorkWithComAirViaIOSSpecificPlugin)
		{
			MicrophoneMeter.Shutdown();
		}
	}

	private void InitializeMicrophoneListener()
	{
		switch (m_MicrophoneMode)
		{
		case MicrophoneMode.DisableComAirAndAccessMicrophoneDirectly:
			StartRecordingMicrophoneClip();
			break;
		case MicrophoneMode.WorkWithComAirViaIOSSpecificPlugin:
			MicrophoneMeter.Initialise();
			break;
		}
	}

	private void Update()
	{
		switch (m_MicrophoneMode)
		{
		case MicrophoneMode.DisableComAirAndAccessMicrophoneDirectly:
			if (Microphone.IsRecording(m_CachedDeviceName))
			{
				m_volume = GetRawVolume();
				break;
			}
			StartRecordingMicrophoneClip();
			m_volume = 0f;
			break;
		case MicrophoneMode.WorkWithComAirViaIOSSpecificPlugin:
			m_volume = TranslatePeakVolumeToClampedValue();
			break;
		}
		UpdateOscilloscope();
	}

	private float TranslatePeakVolumeToClampedValue()
	{
		float peakVolume = MicrophoneMeter.GetPeakVolume();
		m_volume = Mathf.Pow(10f, peakVolume / 20f);
		return m_volume;
	}

	private void StartRecordingMicrophoneClip()
	{
		Microphone.GetDeviceCaps(null, out m_FrequencyMinimum, out m_FrequencyMaximum);
		if (m_FrequencyMinimum == 0 && m_FrequencyMaximum == 0)
		{
			m_FrequencyMaximum = 44100;
		}
		m_audioClip = Microphone.Start(null, true, 1, m_FrequencyMaximum);
	}

	private void InitializeMicrophoneSampler()
	{
		m_samples = new float[m_numSamples];
		m_widthInterval = Vector3.Distance(m_startPosTransform.localPosition, m_endPosTransform.localPosition) / (float)m_numLineRendererPositions;
		m_maxHeight = Mathf.Abs(m_maxHeightTransform.localPosition.y);
	}

	private float GetRawVolume()
	{
		if ((bool)m_audioClip)
		{
			m_micPosition = Microphone.GetPosition(m_CachedDeviceName);
			if (m_micPosition >= 0)
			{
				m_audioClip.GetData(m_samples, Mathf.Clamp(m_micPosition - m_samples.Length, 0, 44100));
				float num = 0f;
				m_volume = 0f;
				for (int i = 0; i < m_numSamples; i++)
				{
					num = m_samples[i];
					if (num > m_volume)
					{
						m_volume = num;
					}
				}
				m_volume = Mathf.Clamp(m_volume * m_volumeMultiplier, 0f, 1f);
			}
		}
		else
		{
			m_volume = 0f;
		}
		return m_volume;
	}

	private void UpdateOscilloscope()
	{
		SmoothAndDampenVolume();
		SetLineRendererPositions();
	}

	private void SmoothAndDampenVolume()
	{
		m_smoothVolume = Mathf.SmoothDamp(m_currentVolume, m_volume, ref m_currentVolumeVelocity, m_volumeSmoothTime);
		m_currentVolume = m_smoothVolume;
	}

	private void SetLineRendererPositions()
	{
		for (int i = 0; i < m_numLineRendererPositions; i++)
		{
			float x = (0f - m_widthInterval) * (float)(m_numLineRendererPositions / 2) + (m_widthInterval + m_widthInterval * (float)i);
			float num = Random.Range(0f - m_maxHeight, m_maxHeight);
			float y = num * m_smoothVolume * m_waveCurve.Evaluate((float)i / (float)m_numLineRendererPositions);
			m_lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
		}
	}
}
