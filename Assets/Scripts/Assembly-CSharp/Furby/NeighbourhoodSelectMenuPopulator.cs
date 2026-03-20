using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class NeighbourhoodSelectMenuPopulator : MonoBehaviour
	{
		[SerializeField]
		public TribeSpecificGameObject[] m_TribeSpecifics;

		public UIGrid m_Grid;

		[SerializeField]
		public List<GameObject> m_SourceObjects = new List<GameObject>();

		[SerializeField]
		public GameObject m_Target;

		private void OnEnable()
		{
			Logging.Log("NeighbourhoodSelectMenuPopulator.OnEnable");
			PopulateMenu();
		}

		public void PopulateMenu()
		{
			Logging.Log("NeighbourhoodSelectMenuPopulator.PopulateMenu");
			TribeSpecificGameObject[] tribeSpecifics = m_TribeSpecifics;
			foreach (TribeSpecificGameObject tribeSpecificGameObject in tribeSpecifics)
			{
				if (IsTribeEligible(tribeSpecificGameObject.m_Tribe))
				{
					Logging.Log("NeighbourhoodSelectMenuPopulator.OnEnable -> Tribe " + tribeSpecificGameObject.m_Tribe.ToString() + " is eligible...");
					tribeSpecificGameObject.m_TargetGo.SetActive(true);
				}
				else
				{
					Logging.Log("NeighbourhoodSelectMenuPopulator.OnEnable -> Tribe " + tribeSpecificGameObject.m_Tribe.ToString() + " is ineligible (Deactivating)");
					tribeSpecificGameObject.m_TargetGo.SetActive(false);
				}
			}
			ReparentObjects();
			if (m_Grid != null)
			{
				m_Grid.Reposition();
			}
		}

		private void ReparentObjects()
		{
			foreach (GameObject sourceObject in m_SourceObjects)
			{
				if (sourceObject.activeInHierarchy)
				{
					sourceObject.transform.parent = m_Target.transform;
				}
			}
		}

		private bool IsTribeEligible(Tribeset tribeset)
		{
			bool result = true;
			switch (tribeset)
			{
			case Tribeset.Spring:
				result = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring;
				break;
			case Tribeset.CrystalGem:
				result = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
				break;
			}
			return result;
		}
	}
}
