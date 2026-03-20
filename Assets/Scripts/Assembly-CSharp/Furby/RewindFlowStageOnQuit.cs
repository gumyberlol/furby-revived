using Relentless;
using UnityEngine;

namespace Furby
{
	public class RewindFlowStageOnQuit : MonoBehaviour
	{
		[SerializeField]
		private SpsBackToScreen m_goingBack;

		[SerializeField]
		private FlowStage m_flowStageToRewindTo;

		private SpsBackToScreen.GoingBackHandler m_handler;

		public RewindFlowStageOnQuit()
		{
			m_handler = delegate
			{
				Logging.Log(string.Format("RewindFlowStageOnQuit:  Resetting flow stage to {0}", m_flowStageToRewindTo));
				FurbyGlobals.Player.FlowStage = m_flowStageToRewindTo;
			};
		}

		private void OnEnable()
		{
			m_goingBack.GoingBack += m_handler;
		}

		private void OnDisable()
		{
			m_goingBack.GoingBack -= m_handler;
		}
	}
}
