using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class RepositionBasedOnLastScreen : RelentlessMonoBehaviour
	{
		public Vector3 m_offset;

		[SerializeField]
		private List<string> m_previousScenesToActivateOn;

		[SerializeField]
		private List<FlowStage> m_flowStagesToActivateOn;

		private void Awake()
		{
			string previousString = FurbyGlobals.ScreenSwitcher.GetPreviousScreenName();
			FlowStage currentFlowStage = FurbyGlobals.Player.FlowStage;
			bool flag = false;
			flag |= m_previousScenesToActivateOn.Exists((string x) => x == previousString);
			if (flag | m_flowStagesToActivateOn.Exists((FlowStage x) => x == currentFlowStage))
			{
				base.transform.localPosition = base.transform.localPosition + m_offset;
			}
		}
	}
}
