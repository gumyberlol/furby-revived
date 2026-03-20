using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyGUI : Singleton<FurbyGUI>
	{
		[EasyEditArray]
		[SerializeField]
		public GameObject[] m_prefabbedScreens;

		private GameObject[] m_screens;

		public FurbyGUISection guiSection;

		public FurbyGUIController controller;

		public FurbyGUIEvents eventManager;

		private List<GameObject> m_instantiatedOverlays = new List<GameObject>();

		private void Awake()
		{
			GameObject[] prefabbedScreens = m_prefabbedScreens;
			foreach (GameObject gameObject in prefabbedScreens)
			{
				GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
				gameObject2.name = gameObject.name;
				gameObject2.transform.parent = base.transform;
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject2.layer = 14;
				gameObject2.SetActive(false);
				m_instantiatedOverlays.Add(gameObject2);
			}
			guiSection = GetComponent<FurbyGUISection>();
			guiSection.initGUI();
		}

		public void Show(GameObject temp)
		{
			temp.SetActive(true);
		}

		public void Hide(GameObject temp)
		{
			temp.SetActive(false);
		}

		private void OnLevelWasLoaded(int level)
		{
			foreach (GameObject instantiatedOverlay in m_instantiatedOverlays)
			{
				instantiatedOverlay.SetActive(false);
			}
		}
	}
}
