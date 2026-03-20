using System;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/Fabric Game Event Translator")]
	public class FabricGameEventTranslator : RelentlessMonoBehaviour
	{
		[Serializable]
		public class GameToFabric
		{
			public string GameEventName = string.Empty;

			public EventAction Action;

			public string FabricEventName = string.Empty;

			public float FloatParameter;

			public string StringParameter = string.Empty;

			public bool IgnoreGameObject;

			public float Probability = 100f;

			public float Delay;

			public bool OverrideParameter;
		}

		public string Type;

		[SerializeField]
		private GameToFabric[] m_eventTable;

		private Dictionary<Enum, List<GameToFabric>> m_eventLookup;

		private Type m_type;

		public GameToFabric[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		private void Awake()
		{
			m_eventLookup = new Dictionary<Enum, List<GameToFabric>>();
			m_type = System.Type.GetType(Type);
			GameToFabric[] eventTable = m_eventTable;
			foreach (GameToFabric gameToFabric in eventTable)
			{
				Enum key = (Enum)Enum.Parse(m_type, gameToFabric.GameEventName);
				List<GameToFabric> value;
				if (!m_eventLookup.TryGetValue(key, out value))
				{
					value = new List<GameToFabric>();
					m_eventLookup.Add(key, value);
				}
				value.Add(gameToFabric);
			}
		}

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(m_type, OnEvent);
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(m_type, OnEvent);
			}
		}

		private void OnEvent(Enum enumValue, GameObject gameObject, params object[] list)
		{
			List<GameToFabric> value;
			if (!m_eventLookup.TryGetValue(enumValue, out value))
			{
				return;
			}
			foreach (GameToFabric item in value)
			{
				if (item.Probability < 100f)
				{
					System.Random random = new System.Random();
					if ((float)(int)(random.NextDouble() * 100.0) > item.Probability)
					{
						break;
					}
				}
				PostFabricEvent(item, gameObject, list);
			}
		}

		private void PostFabricEvent(GameToFabric eventData, GameObject gameObject, params object[] list)
		{
			Fabric.Event obj = new Fabric.Event();
			obj._eventName = eventData.FabricEventName;
			obj.EventAction = eventData.Action;
			obj.parentGameObject = ((!eventData.IgnoreGameObject) ? gameObject : null);
			obj._delay = eventData.Delay;
			switch (eventData.Action)
			{
			case EventAction.PlaySound:
			case EventAction.StopSound:
			case EventAction.PauseSound:
			case EventAction.UnpauseSound:
				obj._parameter = null;
				break;
			case EventAction.SetVolume:
			case EventAction.SetPitch:
			case EventAction.SetPan:
				obj._parameter = eventData.FloatParameter;
				break;
			case EventAction.AddPreset:
			case EventAction.RemovePreset:
				obj._parameter = eventData.StringParameter;
				break;
			case EventAction.RegisterGameObject:
				obj._parameter = null;
				break;
			case EventAction.ResetDynamicMixer:
				obj._parameter = null;
				break;
			case EventAction.SetSwitch:
				obj._parameter = eventData.StringParameter;
				break;
			case EventAction.SetParameter:
			{
				ParameterData parameterData = new ParameterData
				{
					_parameter = eventData.StringParameter,
					_value = eventData.FloatParameter
				};
				if (eventData.OverrideParameter)
				{
					parameterData._value = (float)list[0];
				}
				obj._parameter = parameterData;
				break;
			}
			default:
				Logging.LogWarning(string.Format("Fabric Event type {0} not supported", eventData.Action.ToString()));
				break;
			}
			EventManager.Instance.PostEvent(obj);
		}

		[ContextMenu("Convert to FabricGET2")]
		public void ConvertToVersion2()
		{
			FabricGameEventTranslator2 component = GetComponent<FabricGameEventTranslator2>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			component = base.gameObject.AddComponent<FabricGameEventTranslator2>();
			component.InitialiseFromOldFabricGET(this);
		}
	}
}
