using Relentless;
using UnityEngine;

namespace Furby
{
	public class SetActiveOnSaveState : MonoBehaviour
	{
		[SerializeField]
		public SaveState m_SaveState;

		public void Awake()
		{
			if (m_SaveState == SaveState.SwitchOnIfNewSave && !DoesValidSaveDataExist())
			{
				base.gameObject.SetActive(true);
			}
			if (m_SaveState == SaveState.SwitchOffIfNewSave && !DoesValidSaveDataExist())
			{
				base.gameObject.SetActive(false);
			}
			if (m_SaveState == SaveState.SwitchOnIfExistingSave && DoesValidSaveDataExist())
			{
				base.gameObject.SetActive(true);
			}
			if (m_SaveState == SaveState.SwitchOffIfExistingSave && DoesValidSaveDataExist())
			{
				base.gameObject.SetActive(false);
			}
		}

		private bool DoesValidSaveDataExist()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow;
		}
	}
}
