using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class DuplicatesMenuPopulator : RelentlessMonoBehaviour
	{
		public List<GameObject> m_Duplicates;

		public List<FurbyBaby> m_currentFurblings;

		public void InstantiateCells()
		{
			for (int i = 0; i < m_currentFurblings.Count; i++)
			{
				FurbyBaby furbyBaby = m_currentFurblings[i];
				GameObject gameObject = m_Duplicates[i];
				gameObject.SetActive(true);
				GameObject gameObject2 = gameObject.transform.Find("LabelText").gameObject;
				UILabel uILabel = (UILabel)gameObject2.GetComponent(typeof(UILabel));
				uILabel.text = furbyBaby.Name;
			}
			for (int j = m_currentFurblings.Count; j < m_Duplicates.Count; j++)
			{
				m_Duplicates[j].SetActive(false);
			}
		}
	}
}
