using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GoToFlowStageWhenDone : MonoBehaviour
	{
		[SerializeField]
		private FlowStage m_stage;

		[SerializeField]
		private List<TutorialInstructionBox> m_tutorialBoxes;

		private void Start()
		{
			GameData gameData = Singleton<GameDataStoreObject>.Instance.Data;
			foreach (TutorialInstructionBox tutorialBox in m_tutorialBoxes)
			{
				tutorialBox.Exiting += delegate
				{
					gameData.FlowStage = m_stage;
				};
			}
		}
	}
}
