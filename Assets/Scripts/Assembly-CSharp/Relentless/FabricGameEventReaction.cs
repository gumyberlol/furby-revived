using System;
using Fabric;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class FabricGameEventReaction : GameEventReaction
	{
		public EventAction Action;

		public string FabricEventName = string.Empty;

		public float FloatParameter;

		public string StringParameter = string.Empty;

		public bool IgnoreGameObject;

		public float Probability = 100f;

		public float Delay;

		public bool OverrideParameter;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			if (Probability < 100f)
			{
				System.Random random = new System.Random();
				if ((float)(int)(random.NextDouble() * 100.0) < Probability)
				{
					return;
				}
			}
			Fabric.Event obj = new Fabric.Event();
			obj._eventName = FabricEventName;
			obj.EventAction = Action;
			obj.parentGameObject = ((!IgnoreGameObject) ? gameObject : null);
			obj._delay = Delay;
			switch (Action)
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
				obj._parameter = FloatParameter;
				break;
			case EventAction.AddPreset:
			case EventAction.RemovePreset:
				obj._parameter = StringParameter;
				break;
			case EventAction.RegisterGameObject:
				obj._parameter = null;
				break;
			case EventAction.ResetDynamicMixer:
				obj._parameter = null;
				break;
			case EventAction.SetSwitch:
				obj._parameter = StringParameter;
				break;
			case EventAction.SetParameter:
			{
				ParameterData parameterData = new ParameterData
				{
					_parameter = StringParameter,
					_value = FloatParameter
				};
				if (OverrideParameter)
				{
					parameterData._value = (float)paramlist[0];
				}
				obj._parameter = parameterData;
				break;
			}
			default:
				Logging.LogWarning(string.Format("Fabric Event type {0} not supported", Action.ToString()));
				break;
			}
			EventManager.Instance.PostEvent(obj);
		}
	}
}
