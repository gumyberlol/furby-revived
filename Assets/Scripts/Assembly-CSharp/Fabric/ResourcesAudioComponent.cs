using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/ResourcesAudioComponent")]
	public class ResourcesAudioComponent : Component
	{
		private AudioSource _audioSource;

		private GameObject _audioSourceGameObject;

		private AudioComponentState _currentState;

		private AudioClip _audioClip;

		[SerializeField]
		[HideInInspector]
		private string _audioClipReference;

		[SerializeField]
		[HideInInspector]
		private int _delay;

		[HideInInspector]
		[SerializeField]
		private bool _loop;

		[SerializeField]
		[HideInInspector]
		private bool _dontPlay;

		[HideInInspector]
		[SerializeField]
		private bool _dontStopOnDestroy;

		[HideInInspector]
		[SerializeField]
		private bool _ignoreVirtualization;

		[SerializeField]
		private List<Marker> _markers = new List<Marker>();

		private bool _hasAudioSource;

		private int _pauseTimeInSamples;

		public Transform _transform;

		private Vector3 _cachedPosition;

		private GameObject m_streamLoader;

		public static HashSet<ResourcesAudioComponent> Instances = new HashSet<ResourcesAudioComponent>();

		private AudioLowPassFilter m_lowPassFilter;

		public GameObject AudioSourceGameObject
		{
			get
			{
				return _audioSourceGameObject;
			}
		}

		public AudioComponentState CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public string AudioClipReference
		{
			get
			{
				return _audioClipReference;
			}
			set
			{
				_audioClipReference = value;
			}
		}

		public int Delay
		{
			get
			{
				return _delay;
			}
			set
			{
				_delay = value;
			}
		}

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public bool DontPlay
		{
			get
			{
				return _dontPlay;
			}
			set
			{
				_dontPlay = value;
			}
		}

		public bool DontStopOnDestroy
		{
			get
			{
				return _dontStopOnDestroy;
			}
			set
			{
				_dontStopOnDestroy = value;
			}
		}

		public List<Marker> Markers
		{
			get
			{
				return _markers;
			}
		}

		public static void PreLoad(string preLoadName)
		{
			foreach (ResourcesAudioComponent instance in Instances)
			{
				if (instance.name == preLoadName)
				{
					instance.Load();
				}
			}
		}

		public static bool Purge()
		{
			bool result = false;
			foreach (ResourcesAudioComponent instance in Instances)
			{
				if (instance._currentState != AudioComponentState.Playing && instance.Unload())
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void OnLevelWasLoaded()
		{
			if (_currentState != AudioComponentState.Playing && _currentState != AudioComponentState.WaitingToPlay)
			{
				Unload();
			}
		}

		protected override void OnInitialise()
		{
			Instances.Add(this);
			_audioSource = base.gameObject.GetComponent<AudioSource>();
			if ((bool)_audioSource)
			{
				Logging.LogWarning("Fabric: Adding an AudioSource and AudioComponent [" + base.name + "] in the same gameObject will impact performance, move AudioSource in a new gameObject underneath");
				_audioSource.enabled = false;
				_hasAudioSource = true;
			}
			else if (!AudioSourcePool.IsInitialised() || AudioSourcePool.Instance.Size() == 0)
			{
				bool flag = false;
				int childCount = base.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					GameObject gameObject = base.transform.GetChild(i).gameObject;
					AudioSource component = gameObject.GetComponent<AudioSource>();
					if (component != null)
					{
						_audioSourceGameObject = gameObject;
						_audioSource = component;
						flag = true;
					}
				}
				if (!flag)
				{
					_audioSourceGameObject = new GameObject("AudioSource");
					_audioSource = _audioSourceGameObject.AddComponent<AudioSource>();
					_audioSourceGameObject.transform.parent = base.gameObject.transform;
				}
				if (_audioSource == null)
				{
					DebugLog.Print("AudioComponent failed to create an AudioSource", DebugLevel.Error);
					return;
				}
				_audioSource.playOnAwake = false;
				_audioSource.loop = _loop;
				_audioSource.enabled = false;
				_audioSourceGameObject.SetActive(false);
				_hasAudioSource = true;
				_transform = _audioSource.transform;
			}
			_currentState = AudioComponentState.Stopped;
		}

		private void OnDestroy()
		{
			if (EventManager.IsInitialised() && _eventListeners != null)
			{
				for (int i = 0; i < _eventListeners.Length; i++)
				{
					EventManager.Instance.UnregisterListener(this, _eventListeners[i]._eventName);
				}
			}
			Instances.Remove(this);
			Unload();
		}

		private void Load()
		{
			if (_audioClip == null)
			{
				string text = _audioClipReference.Replace(".wav", string.Empty) + "_SL";
				GameObject gameObject = (GameObject)Resources.Load(text, typeof(GameObject));
				if ((bool)gameObject)
				{
					m_streamLoader = (GameObject)UnityEngine.Object.Instantiate(gameObject);
					AudioSource component = m_streamLoader.GetComponent<AudioSource>();
					_audioClip = component.clip;
				}
				else
				{
					Logging.LogWarning("Couldn't load stream loader for " + text);
				}
			}
			if (_audioSource != null)
			{
				_audioSource.clip = _audioClip;
			}
		}

		private bool Unload()
		{
			bool result = false;
			if (_audioSource != null)
			{
				AudioClip clip = _audioSource.clip;
				_audioSource.clip = null;
				if (clip != null)
				{
					result = true;
					Resources.UnloadAsset(clip);
				}
			}
			if (m_streamLoader != null)
			{
				UnityEngine.Object.Destroy(m_streamLoader);
			}
			m_streamLoader = null;
			_audioClip = null;
			return result;
		}

		public override void ApplyPropertiesToInstances()
		{
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				if (_componentInstances[i]._instance.IsInstance)
				{
					AudioComponent audioComponent = _componentInstances[i]._instance as AudioComponent;
					if ((bool)audioComponent)
					{
						audioComponent.Loop = _loop;
						audioComponent.DontPlay = _dontPlay;
					}
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();
			Unload();
			_currentState = AudioComponentState.Stopped;
		}

		public override void Play(ComponentInstance zComponentInstance)
		{
			Logging.Log("Play requested " + base.name);
			if (_dontPlay)
			{
				return;
			}
			Reset();
			_componentInstance = zComponentInstance;
			SetComponentActive(true);
			if (_componentInstance != null)
			{
				base.DelaySamples = _componentInstance._instance.DelaySamples;
			}
			if (_audioSource == null)
			{
				_audioSource = AudioSourcePool.Instance.Alloc();
				if (_audioSource == null)
				{
					DebugLog.Print("AudioComponent failed to create an AudioSource", DebugLevel.Error);
					return;
				}
				m_lowPassFilter = _audioSource.gameObject.AddComponent<AudioLowPassFilter>();
				m_lowPassFilter.enabled = false;
			}
			Load();
			_audioSource.playOnAwake = false;
			_audioSource.loop = _loop;
			if (_componentInstance._parentGameObject != null)
			{
				_cachedPosition = _componentInstance._parentGameObject.transform.position;
			}
			_currentState = AudioComponentState.WaitingToPlay;
		}

		public void SetFilterEnabled(bool shouldEnable, float freq)
		{
			m_lowPassFilter.enabled = shouldEnable;
			m_lowPassFilter.cutoffFrequency = freq;
		}

		public override bool IsPlaying()
		{
			bool result = true;
			if (_currentState == AudioComponentState.Stopped)
			{
				result = false;
			}
			return result;
		}

		public override bool IsOneShot()
		{
			return !_loop;
		}

		public override int GetTimeSamples()
		{
			if (_audioSource != null && _audioSource.clip != null)
			{
				return _audioSource.clip.samples - _audioSource.timeSamples;
			}
			return -1;
		}

		public int GetTimeSamplesFromStart()
		{
			if (_audioSource != null && _audioSource.clip != null)
			{
				return _audioSource.timeSamples;
			}
			return -1;
		}

		public override int GetSampleRate()
		{
			if (_audioClip != null)
			{
				return _audioClip.frequency;
			}
			return -1;
		}

		public override bool HasReachedEnd()
		{
			if (_audioSource != null && _audioSource.clip != null && _audioSource.isPlaying && Math.Abs(_audioSource.timeSamples - _audioClip.samples) < 1000)
			{
				return true;
			}
			return false;
		}

		private bool CheckIsVirtual()
		{
			bool result = false;
			if (!_ignoreVirtualization && FabricManager.Instance._enableVirtualization && Camera.main != null)
			{
				float num = Vector3.Distance(_componentInstance._transform.position, Camera.main.transform.position);
				if (num > _audioSource.maxDistance)
				{
					if (_currentState != AudioComponentState.Virtual)
					{
						_pauseTimeInSamples = _audioSource.timeSamples;
						_audioSource.Stop();
						_audioSource.enabled = false;
						if (_audioSourceGameObject != null)
						{
							_audioSourceGameObject.SetActive(false);
						}
						_currentState = AudioComponentState.Virtual;
						result = true;
					}
				}
				else if (_currentState == AudioComponentState.Virtual)
				{
					if (_audioSourceGameObject != null)
					{
						_audioSourceGameObject.SetActive(false);
					}
					_audioSource.enabled = true;
					_audioSource.timeSamples = _pauseTimeInSamples;
					_pauseTimeInSamples = 0;
					_audioSource.Play();
					_currentState = AudioComponentState.Playing;
					result = false;
				}
			}
			return result;
		}

		private void OnApplicationPause(bool pause)
		{
		}

		public override bool UpdateInternal(ref Context context)
		{
			profiler.Begin();
			if (_componentInstance == null || (_componentInstance._parentGameObject == null && !_dontStopOnDestroy) || _audioSource == null)
			{
				Stop(false, true, false, 0f);
				return false;
			}
			CheckIsVirtual();
			switch (_currentState)
			{
			case AudioComponentState.WaitingToPlay:
				UpdatePosition();
				if (_audioSourceGameObject != null)
				{
					_audioSourceGameObject.SetActive(true);
				}
				_audioSource.enabled = true;
				_audioSource.Play((ulong)base.DelaySamples + (ulong)_delay);
				_audioSource.volume = 0f;
				_audioSource.pitch = 1f;
				_currentState = AudioComponentState.Playing;
				break;
			case AudioComponentState.Playing:
				if (!_audioSource.isPlaying)
				{
					StopInternal();
				}
				break;
			case AudioComponentState.WaitingToStop:
				if (_updateContext._fadeParameter == 0f)
				{
					StopInternal();
				}
				else if (_audioSource != null && !_audioSource.isPlaying)
				{
					StopInternal();
				}
				break;
			}
			UpdateProperties(context);
			_isComponentActive = false;
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance.IsInstance && componentInstance._instance.IsPlaying())
					{
						_isComponentActive |= componentInstance._instance.UpdateInternal(ref context);
					}
				}
			}
			if (_currentState != AudioComponentState.Stopped)
			{
				_isComponentActive |= true;
			}
			profiler.End();
			return _isComponentActive;
		}

		private void UpdateProperties(Context context)
		{
			if (_audioSource == null || !_audioSource.isPlaying)
			{
				return;
			}
			if (_RTPManager != null)
			{
				_RTPManager.Update(this);
			}
			if (_overrideParentVolume || _parentComponent == null)
			{
				_updateContext._volume = _volume;
			}
			else
			{
				_updateContext._volume = context._volume * _volume;
			}
			_updateContext._volume -= _volumeOffset;
			_updateContext._volume *= _sideChainGain;
			if (_overrideFadeProperties || _parentComponent == null)
			{
				_updateContext._fadeParameter = _fadeParameter.Get(FabricTimer.Get());
			}
			else
			{
				_updateContext._fadeParameter = _fadeParameter.Get(FabricTimer.Get()) * context._fadeParameter;
			}
			_updateContext._volume *= _updateContext._fadeParameter;
			if (_audioSource.volume != _updateContext._volume)
			{
				_audioSource.volume = _updateContext._volume;
			}
			if (_overrideParentPitch || _parentComponent == null)
			{
				_updateContext._pitch = _pitch;
			}
			else
			{
				_updateContext._pitch = context._pitch * _pitch;
			}
			_updateContext._pitch += _pitchOffset;
			if (_audioSource.pitch != _updateContext._pitch)
			{
				_audioSource.pitch = _updateContext._pitch;
			}
			if (_override2DProperties || _parentComponent == null)
			{
				if (_audioSource.pan != _pan2D)
				{
					_audioSource.pan = _pan2D;
				}
			}
			else if (_audioSource.pan != context._pan2D)
			{
				_audioSource.pan = context._pan2D;
			}
			if (_override3DProperties || _parentComponent == null)
			{
				if (_audioSource.minDistance != _minDistance)
				{
					_audioSource.minDistance = _minDistance;
				}
				if (_audioSource.maxDistance != _maxDistance)
				{
					_audioSource.maxDistance = _maxDistance;
				}
				if (_audioSource.priority != _priority)
				{
					_audioSource.priority = _priority;
				}
				if (_audioSource.rolloffMode != _rolloffMode)
				{
					_audioSource.rolloffMode = _rolloffMode;
				}
				if (_audioSource.panLevel != _panLevel)
				{
					_audioSource.panLevel = _panLevel;
				}
				if (_audioSource.spread != _spreadLevel)
				{
					_audioSource.spread = _spreadLevel;
				}
				if (_audioSource.dopplerLevel != _dopplerLevel)
				{
					_audioSource.dopplerLevel = _dopplerLevel;
				}
			}
			else
			{
				if (_audioSource.minDistance != context._minDistance)
				{
					_audioSource.minDistance = context._minDistance;
				}
				if (_audioSource.maxDistance != context._maxDistance)
				{
					_audioSource.maxDistance = context._maxDistance;
				}
				if (_audioSource.priority != context._priority)
				{
					_audioSource.priority = context._priority;
				}
				if (_audioSource.rolloffMode != context._rolloffMode)
				{
					_audioSource.rolloffMode = context._rolloffMode;
				}
				if (_audioSource.panLevel != context._panLevel)
				{
					_audioSource.panLevel = context._panLevel;
				}
				if (_audioSource.spread != context._spreadLevel)
				{
					_audioSource.spread = context._spreadLevel;
				}
				if (_audioSource.dopplerLevel != context._dopplerLevel)
				{
					_audioSource.dopplerLevel = context._dopplerLevel;
				}
			}
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			if (_componentInstance != null && _componentInstance._parentGameObject != null)
			{
				if (_audioSource.transform.position != _componentInstance._transform.position)
				{
					_audioSource.transform.position = _componentInstance._transform.position;
					_cachedPosition = _audioSource.transform.position;
				}
			}
			else
			{
				_audioSource.transform.position = _cachedPosition;
			}
		}

		private void StopInternal()
		{
			if (_audioSource == null)
			{
				_currentState = AudioComponentState.Stopped;
			}
			else
			{
				if (_currentState == AudioComponentState.Stopped)
				{
					return;
				}
				if (!_hasAudioSource)
				{
					AudioSourcePool.Instance.Free(_audioSource);
					UnityEngine.Object.Destroy(m_lowPassFilter);
					Unload();
					_audioSource = null;
				}
				else
				{
					_audioSource.enabled = false;
					if (_audioSourceGameObject != null)
					{
						_audioSourceGameObject.SetActive(false);
					}
					Unload();
				}
				_currentState = AudioComponentState.Stopped;
			}
		}

		public override void Stop(bool stopInstances, bool forceStop, bool ignoreFade, float fadeTime)
		{
			if (_audioSource == null)
			{
				_currentState = AudioComponentState.Stopped;
			}
			else
			{
				if (_currentState == AudioComponentState.Stopped)
				{
					return;
				}
				if (forceStop)
				{
					if (!_hasAudioSource)
					{
						AudioSourcePool.Instance.Free(_audioSource, true);
						_audioSource = null;
					}
					else
					{
						_audioSource.Stop();
						_audioSource.enabled = false;
						if (_audioSourceGameObject != null)
						{
							_audioSourceGameObject.SetActive(false);
						}
					}
					_currentState = AudioComponentState.Stopped;
					_isComponentActive = false;
					Unload();
				}
				else if (_currentState != AudioComponentState.WaitingToStop)
				{
					if (!ignoreFade || _overrideFadeProperties || fadeTime > 0f)
					{
						_fadeParameter.SetTarget(FabricTimer.Get(), 0f, (!(fadeTime > 0f)) ? _fadeOutTime : fadeTime, _fadeOutCurve);
					}
					_currentState = AudioComponentState.WaitingToStop;
				}
				if (_componentInstances == null || !stopInstances)
				{
					return;
				}
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance.IsInstance && componentInstance._instance.IsPlaying())
					{
						componentInstance._instance.Stop(stopInstances, forceStop);
					}
				}
			}
		}

		public override void Pause(bool pause)
		{
			if (_audioSource == null)
			{
				return;
			}
			if (_currentState == AudioComponentState.Paused && !pause)
			{
				_currentState = AudioComponentState.Playing;
				if (_audioSourceGameObject != null)
				{
					_audioSourceGameObject.SetActive(true);
				}
				_audioSource.enabled = true;
				_audioSource.timeSamples = _pauseTimeInSamples;
				_audioSource.Play();
			}
			else if (pause && _currentState != AudioComponentState.Stopped)
			{
				_currentState = AudioComponentState.Paused;
				_pauseTimeInSamples = _audioSource.timeSamples;
				_audioSource.Pause();
			}
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance._instance.IsInstance)
				{
					componentInstance._instance.Pause(pause);
				}
			}
		}

		public void SetAudioClip(string audioClipReference, GameObject parentGameObject)
		{
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				ResourcesAudioComponent resourcesAudioComponent = componentInstance._instance as ResourcesAudioComponent;
				if (resourcesAudioComponent != null)
				{
					if (parentGameObject == null)
					{
						resourcesAudioComponent._audioClipReference = audioClipReference;
					}
					else if (parentGameObject == componentInstance._parentGameObject)
					{
						resourcesAudioComponent._audioClipReference = audioClipReference;
					}
				}
			}
		}
	}
}
