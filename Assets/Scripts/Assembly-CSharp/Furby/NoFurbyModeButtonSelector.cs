using Relentless;
using UnityEngine;

namespace Furby
{
	public class NoFurbyModeButtonSelector : MonoBehaviour
	{
		[SerializeField]
		public ButtonRoots m_ButtonRoots;

		public void Awake()
		{
			bool flag = SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable();
			bool hasCompletedFirstTimeFlow = Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow;
			Logging.Log("NoFurbyModeButtonSelector: IAPsAvailable? " + flag);
			Logging.Log("NoFurbyModeButtonSelector: existingSave? " + hasCompletedFirstTimeFlow);
			if (flag)
			{
				if (hasCompletedFirstTimeFlow)
				{
					m_ButtonRoots.m_IAPsAvailableNewSave.SetActive(false);
					m_ButtonRoots.m_IAPsAvailableExistingSave.SetActive(true);
					m_ButtonRoots.m_IAPsUnavailableNewSave.SetActive(false);
					m_ButtonRoots.m_IAPsUnavailableExistingSave.SetActive(false);
				}
				else
				{
					m_ButtonRoots.m_IAPsAvailableNewSave.SetActive(true);
					m_ButtonRoots.m_IAPsAvailableExistingSave.SetActive(false);
					m_ButtonRoots.m_IAPsUnavailableNewSave.SetActive(false);
					m_ButtonRoots.m_IAPsUnavailableExistingSave.SetActive(false);
				}
			}
			else if (hasCompletedFirstTimeFlow)
			{
				m_ButtonRoots.m_IAPsAvailableNewSave.SetActive(false);
				m_ButtonRoots.m_IAPsAvailableExistingSave.SetActive(false);
				m_ButtonRoots.m_IAPsUnavailableNewSave.SetActive(false);
				m_ButtonRoots.m_IAPsUnavailableExistingSave.SetActive(true);
			}
			else
			{
				m_ButtonRoots.m_IAPsAvailableNewSave.SetActive(false);
				m_ButtonRoots.m_IAPsAvailableExistingSave.SetActive(false);
				m_ButtonRoots.m_IAPsUnavailableNewSave.SetActive(true);
				m_ButtonRoots.m_IAPsUnavailableExistingSave.SetActive(false);
			}
		}
	}
}
