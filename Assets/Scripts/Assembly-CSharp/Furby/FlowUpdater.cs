using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FlowUpdater : MonoBehaviour
	{
		[Serializable]
		public class StageTrigger
		{
			public FlowStage Current;

			public LevelReference LevelTrigger;

			public FlowStage Next;
		}

		[EasyEditArray]
		[SerializeField]
		private StageTrigger[] m_stageTriggers;

		private void Awake()
		{
			GameEventRouter.AddDelegateForEnums(OnScreenSwitch, SpsEvent.ExitScreen);
		}

		private void OnScreenSwitch(Enum evt, GameObject gObj, params object[] parameters)
		{
			string screen = (string)parameters[1];
			StageTrigger stageTrigger = (from x in m_stageTriggers
				where x.Current == FurbyGlobals.Player.FlowStage
				where x.LevelTrigger.SceneName == screen
				select x).FirstOrDefault();
			if (stageTrigger != null)
			{
				FurbyGlobals.Player.FlowStage = stageTrigger.Next;
			}
		}
	}
}
