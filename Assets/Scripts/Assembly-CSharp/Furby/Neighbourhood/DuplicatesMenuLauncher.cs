using System.Collections.Generic;
using System.IO;
using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class DuplicatesMenuLauncher : RelentlessMonoBehaviour
	{
		public List<FurbyBaby> m_Furblings = new List<FurbyBaby>();

		public GameObject m_TheDuplicatesMenu;

		public List<GameObject> m_DuplicateTiles;

		public Tribeset m_TribeSet;

		public Texture m_DefaultTextureOccupied;

		public UIAtlas m_atlas;

		private void OnClick()
		{
			PopulateTheMenu();
			ActivateTheMenu();
		}

		private void PopulateTheMenu()
		{
			Logging.Log("POPULATING MENU : " + m_DuplicateTiles.Count + " " + m_Furblings.Count);
			for (int num = m_Furblings.Count - 1; num >= 0; num--)
			{
				FurbyBaby furbyBaby = m_Furblings[num];
				if (num < m_DuplicateTiles.Count)
				{
					GameObject gameObject = m_DuplicateTiles[num];
					ShowBabyDNA[] componentsInChildren = gameObject.GetComponentsInChildren<ShowBabyDNA>(true);
					ShowBabyDNA[] array = componentsInChildren;
					foreach (ShowBabyDNA showBabyDNA in array)
					{
						showBabyDNA.SetTargetBaby(furbyBaby);
					}
					gameObject.SetActive(true);
					Transform transform = gameObject.transform.Find("Furbling");
					if (transform != null)
					{
						Object.Destroy(transform.gameObject);
					}
					GameObject gameObject2 = gameObject.transform.Find("LabelText").gameObject;
					UILabel uILabel = (UILabel)gameObject2.GetComponent(typeof(UILabel));
					uILabel.text = furbyBaby.Name;
					GameObject gameObject3 = gameObject.transform.Find("PanelTexture").gameObject;
					UITexture component = gameObject3.GetComponent<UITexture>();
					component.enabled = false;
					GameObject gameObject4 = new GameObject("Furbling");
					gameObject4.transform.parent = gameObject.transform;
					gameObject4.transform.localPosition = new Vector3(0f, 0f, 0f);
					gameObject4.transform.localScale = Vector3.one;
					gameObject4.layer = gameObject.layer;
					HoodController.AddSprite(m_atlas, gameObject4.transform, "FurblingSprite", Path.GetFileName(FurbyGlobals.BabyLibrary.GetBabyFurby(furbyBaby.Type).GetColoringAssetBundleName()), 1, new Vector3(256f, 256f), UIWidget.Pivot.Center);
					string[] flairs = furbyBaby.m_persistantData.flairs;
					foreach (string text in flairs)
					{
						HoodController.AddSprite(m_atlas, gameObject4.transform, "Flair", "FLAIR_" + text, 3, new Vector3(256f, 256f), UIWidget.Pivot.Center);
					}
					HoodController.AddSprite(m_atlas, gameObject4.transform, "Eyes", "furblingThumb_eyes", 2, new Vector3(256f, 256f), UIWidget.Pivot.Center);
					PlayroomCue_FromHood playroomCue_FromHood = gameObject.AddComponent<PlayroomCue_FromHood>();
					playroomCue_FromHood.FurbyBaby = furbyBaby;
				}
				else
				{
					Logging.Log("TOO MANY DUPLICATES!!! - Ignoring " + (m_Furblings.Count - m_DuplicateTiles.Count) + " duplicates.");
				}
			}
			for (int k = m_Furblings.Count; k < m_DuplicateTiles.Count; k++)
			{
				m_DuplicateTiles[k].SetActive(false);
			}
		}

		private void ActivateTheMenu()
		{
			m_TheDuplicatesMenu.SetActive(true);
			GameObject gameObject = GameObject.Find("CameraController");
			if ((bool)gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
