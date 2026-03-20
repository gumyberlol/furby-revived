using System;
using Fabric;
using Relentless;
using UnityEngine;

namespace Kaijudo
{
	public class CodeTriggeredAudioEvent : MonoBehaviour
	{
		private Fabric.Event _event = new Fabric.Event();

		private ParameterData parameter = default(ParameterData);

		private DSPParameterData dspParameter = new DSPParameterData();

		[HideInInspector]
		public string _eventName = string.Empty;

		public FabricEventReference _eventReference;

		public EventAction _eventAction;

		[HideInInspector]
		public object _parameter;

		[HideInInspector]
		public bool _trigger;

		[HideInInspector]
		public string _eventValue = string.Empty;

		[HideInInspector]
		public float _eventParameter = 1f;

		[HideInInspector]
		public string _eventParameterName = string.Empty;

		public float _delay;

		public int _probability = 100;

		public bool _ignoreGameObject;

		[HideInInspector]
		public float _min;

		[HideInInspector]
		public float _max = 1f;

		[HideInInspector]
		public DSPType _dspType;

		[HideInInspector]
		public float _timeToTarget;

		[HideInInspector]
		public float _curve = 0.5f;

		private void PostEvent()
		{
			if (_probability < 100)
			{
				System.Random random = new System.Random();
				if ((int)(random.NextDouble() * 100.0) < _probability)
				{
					return;
				}
			}
			if (_eventReference.m_eventName == string.Empty)
			{
				_event._eventName = _eventName;
			}
			else
			{
				_event._eventName = _eventReference.m_eventName;
			}
			_event.EventAction = _eventAction;
			_event._delay = _delay;
			if (!_ignoreGameObject)
			{
				_event.parentGameObject = base.gameObject;
			}
			else
			{
				_event.parentGameObject = null;
			}
			if (_eventAction == EventAction.SetPitch || _eventAction == EventAction.SetVolume || _eventAction == EventAction.SetPan)
			{
				_event._parameter = _eventParameter;
			}
			else if (_eventAction == EventAction.SetParameter)
			{
				parameter._parameter = _eventParameterName;
				parameter._value = _eventParameter;
				_event._parameter = parameter;
			}
			else if (_eventAction == EventAction.SetDSPParameter)
			{
				dspParameter._dspType = _dspType;
				dspParameter._parameter = _eventParameterName;
				dspParameter._value = _eventParameter;
				dspParameter._time = _timeToTarget;
				dspParameter._curve = _curve;
				_event._parameter = dspParameter;
			}
			else
			{
				_event._parameter = _eventValue;
			}
			EventManager.Instance.PostEvent(_event);
		}

		public static void TriggerAudioEventsOnObject(GameObject group)
		{
			if (group != null)
			{
				group.SendMessage("TriggerAudioEvent", SendMessageOptions.DontRequireReceiver);
			}
		}

		public void TriggerAudioEvent()
		{
			PostEvent();
		}
	}
}
