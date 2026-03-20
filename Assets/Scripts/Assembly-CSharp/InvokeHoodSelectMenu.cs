using Furby;
using UnityEngine;

public class InvokeHoodSelectMenu : MonoBehaviour
{
	[SerializeField]
	public NeighbourhoodSelectMenuPopulator m_MenuPopulator;

	private void OnClick()
	{
		if (m_MenuPopulator == null)
		{
			GameObject gameObject = GameObject.Find("NeighbourhoodSelect_Menu");
			if (gameObject != null)
			{
				m_MenuPopulator = gameObject.GetComponent<NeighbourhoodSelectMenuPopulator>();
			}
		}
		if (m_MenuPopulator != null)
		{
			m_MenuPopulator.gameObject.SetActive(!m_MenuPopulator.gameObject.activeSelf);
			if (m_MenuPopulator != null)
			{
				m_MenuPopulator.PopulateMenu();
			}
		}
	}
}
