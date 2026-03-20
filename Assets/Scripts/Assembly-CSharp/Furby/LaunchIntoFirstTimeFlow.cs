using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LaunchIntoFirstTimeFlow : Singleton<LaunchIntoFirstTimeFlow>
	{
		[Serializable]
		public class TargetScreen
		{
			public FlowStage m_flowStage;

			[LevelReferenceRootFolder("Furby/Scenes")]
			public LevelReference m_level;
		}

		[SerializeField]
		private TargetScreen[] m_targetScreens;

		[SerializeField]
		private LevelReference m_defaultLevel;

		public void SwitchToStartScreen()
		{
			TargetScreen targetScreen = m_targetScreens.Where((TargetScreen x) => x.m_flowStage == FurbyGlobals.Player.FlowStage).FirstOrDefault();
			FurbyGlobals.ScreenSwitcher.SwitchScreen((targetScreen != null) ? targetScreen.m_level : m_defaultLevel, false);
			FurbyGlobals.ScreenSwitcher.Clear();
		}
	}
}
