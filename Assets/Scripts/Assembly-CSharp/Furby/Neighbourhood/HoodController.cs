using System.Collections.Generic;
using System.IO;
using System.Linq;
using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class HoodController : RelentlessMonoBehaviour
	{
		public Tribeset m_NeighbourhoodTribeSet;

		public List<TowerIdent> m_Towers = new List<TowerIdent>();

		[SerializeField]
		public Texture m_DefaultTextureEmpty;

		[SerializeField]
		public Texture m_DefaultTextureOccupied;

		[SerializeField]
		public GameObject m_TheDuplicatesMenu;

		[SerializeField]
		public List<GameObject> m_DuplicateTiles;

		[SerializeField]
		public GameObject m_DuplicatesMenuPrefab;

		[SerializeField]
		private UIAtlas m_furblingAtlas;

		[SerializeField]
		private UIAtlas m_RoomAtlas;

		[SerializeField]
		private Vector3 m_windowSize = new Vector3(52f, 68f, 1f);

		[HideInInspector]
		public FurbyBaby m_TheFurblingInFocus;

		[SerializeField]
		private GameObject m_furblingThumbPrefab;

		[HideInInspector]
		private bool m_AmInRevealSequence;

		public List<FurbyBaby> m_HoodFurblings;

		[HideInInspector]
		public GameObject m_newArrivalFurblingWindow;

		public TowerIdent GetTowerForTribe(FurbyTribeType tribeType)
		{
			foreach (TowerIdent tower in m_Towers)
			{
				if (tower.m_TribeType == tribeType)
				{
					return tower;
				}
			}
			Logging.Log("Couldn't find tower for tribe " + tribeType.ToString());
			return null;
		}

		private void Start()
		{
			if (m_TheFurblingInFocus == null)
			{
				m_TheFurblingInFocus = FurbyGlobals.Player.InProgressFurbyBaby;
			}
			GameEventRouter.SendEvent(HoodEvents.Hood_Opened);
			CacheTowerIdentities();
			PopulateCurrentDistrict();
			m_AmInRevealSequence = GameObject.Find("SENTINEL_ShowRevealSequence") != null;
			if ((m_NeighbourhoodTribeSet == Tribeset.MainTribes || m_NeighbourhoodTribeSet == Tribeset.Spring) && !m_AmInRevealSequence && FurbyGlobals.BabyRepositoryHelpers.HaveCompletedTheGame())
			{
				Logging.Log("Have completed, activating GOLD!");
				PreActivateGoldenTowers();
			}
		}

		private void OnDestroy()
		{
			GameEventRouter.SendEvent(HoodEvents.Hood_Closed);
		}

		private void PreActivateGoldenTowers()
		{
			GameEventRouter.SendEvent(HoodEvents.Hood_GoldenTowers_End);
		}

		private void PopulateCurrentDistrict()
		{
			m_HoodFurblings = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeSet(m_NeighbourhoodTribeSet);
			Logging.Log(string.Concat("PopulateCurrentDistrict Found ", m_HoodFurblings.Count, " furblings, in tribeset ", m_NeighbourhoodTribeSet, " to house..."));
			int num = 0;
			int num2 = m_HoodFurblings.Count - 1;
			for (int num3 = num2; num3 >= 0; num3--)
			{
				FurbyBaby furbling = m_HoodFurblings[num3];
				HouseFurbling(furbling);
				num++;
			}
			CheckAllTowersAndRaiseFlagsIfRequired(true, HoodEvents.Hood_FlagRisesAtStart);
			Logging.Log("PopulateCurrentDistrict found and housed " + m_HoodFurblings.Count + " furblings");
		}

		private void CheckAllTowersAndRaiseFlagsIfRequired(bool excludeTheCurrentFurbling, HoodEvents onRaiseFlagEvent)
		{
			foreach (TowerIdent tower in m_Towers)
			{
				if (tower.m_flagRoot != null && !tower.m_flagRoot.activeInHierarchy)
				{
					if (ShouldFlagBeEnabled(tower, excludeTheCurrentFurbling))
					{
						GameEventRouter.SendEvent(onRaiseFlagEvent);
						tower.m_flagRoot.SetActive(true);
					}
					else
					{
						tower.m_flagRoot.SetActive(false);
					}
				}
			}
		}

		public void ReDoFlags()
		{
			CheckAllTowersAndRaiseFlagsIfRequired(false, HoodEvents.Hood_FlagRisesAfterVanDelivery);
		}

		public static Transform AddSprite(UIAtlas atlas, Transform parentNode, string objectName, string spriteName, int depth, Vector3 size, UIWidget.Pivot pivot)
		{
			GameObject gameObject = new GameObject(objectName);
			gameObject.layer = parentNode.gameObject.layer;
			gameObject.transform.parent = parentNode;
			gameObject.transform.localRotation = Quaternion.identity;
			UISprite uISprite = gameObject.AddComponent<UISprite>();
			uISprite.atlas = atlas;
			uISprite.spriteName = spriteName;
			uISprite.depth = depth;
			UIAtlas.Sprite sprite = uISprite.sprite;
			Vector3 vector = new Vector3(sprite.inner.width, sprite.inner.height, 1f);
			Vector3 vector2 = new Vector3(vector.x * (1f + sprite.paddingLeft + sprite.paddingRight), vector.y * (1f + sprite.paddingTop + sprite.paddingBottom));
			Vector3 vector3 = new Vector3(size.x / vector2.x, size.y / vector2.y, 1f);
			gameObject.transform.localScale = new Vector3(uISprite.sprite.inner.width * vector3.x, uISprite.sprite.inner.height * vector3.y, 1f);
			gameObject.transform.localPosition = Vector3.zero;
			uISprite.pivot = pivot;
			return gameObject.transform;
		}

		private void HouseFurbling(FurbyBaby furbling)
		{
			bool flag = true;
			if (flag)
			{
				Logging.Log("HouseFurbling: " + furbling.ToString());
			}
			TowerIdent towerForTribe = GetTowerForTribe(furbling.Tribe);
			if (towerForTribe == null)
			{
				Logging.Log("Couldn't find tower: " + furbling.Tribe.ToString());
			}
			if (towerForTribe != null)
			{
				if (flag)
				{
					Logging.Log("HouseFurbling() Tower:" + towerForTribe.m_TribeType);
				}
				GameObject theWindowCommensurateWithLevel = GetTheWindowCommensurateWithLevel(towerForTribe, furbling);
				if (flag)
				{
					Logging.Log("HouseFurbling() targetWindow:" + theWindowCommensurateWithLevel.name);
				}
				PlayroomCue_FromHood[] componentsInChildren = theWindowCommensurateWithLevel.GetComponentsInChildren<PlayroomCue_FromHood>(true);
				PlayroomCue_FromHood playroomCue_FromHood = ((componentsInChildren.Length != 0) ? componentsInChildren[0] : null);
				if (playroomCue_FromHood != null)
				{
					if (flag)
					{
						Logging.Log("Room already occupied...");
					}
					EnableWindow(theWindowCommensurateWithLevel);
					DuplicatesMenuLauncher duplicatesMenuLauncher = theWindowCommensurateWithLevel.GetComponentInChildren<DuplicatesMenuLauncher>();
					if (duplicatesMenuLauncher == null)
					{
						if (flag)
						{
							Logging.Log("However, only once before...");
						}
						GameObject gameObject = theWindowCommensurateWithLevel.GetComponentsInChildren<BoxCollider>(true)[0].gameObject;
						playroomCue_FromHood.IsActive = false;
						duplicatesMenuLauncher = gameObject.AddComponent<DuplicatesMenuLauncher>();
						duplicatesMenuLauncher.m_TheDuplicatesMenu = m_TheDuplicatesMenu;
						duplicatesMenuLauncher.m_DuplicateTiles = m_DuplicateTiles;
						duplicatesMenuLauncher.m_Furblings.Add(playroomCue_FromHood.FurbyBaby);
						duplicatesMenuLauncher.m_TribeSet = m_NeighbourhoodTribeSet;
						duplicatesMenuLauncher.m_DefaultTextureOccupied = m_DefaultTextureOccupied;
						duplicatesMenuLauncher.m_atlas = m_furblingAtlas;
					}
					if (flag)
					{
						Logging.Log("Adding duplicate to launcher: " + duplicatesMenuLauncher.m_Furblings.Count);
					}
					duplicatesMenuLauncher.m_Furblings.Add(furbling);
					if (furbling == m_TheFurblingInFocus || duplicatesMenuLauncher.m_Furblings.Contains(m_TheFurblingInFocus))
					{
						Transform transform = theWindowCommensurateWithLevel.transform.Find("NeighbourhoodFurby");
						transform.gameObject.SetActive(false);
						m_newArrivalFurblingWindow = theWindowCommensurateWithLevel;
						Transform transform2 = theWindowCommensurateWithLevel.transform.Find("Tower_window");
						if (transform2 != null)
						{
							transform2.gameObject.SetActive(true);
						}
					}
					else
					{
						EnableWindow(theWindowCommensurateWithLevel);
					}
				}
				else
				{
					theWindowCommensurateWithLevel.name = furbling.Name;
					GameObject gameObject2 = null;
					GameObject gameObject3 = new GameObject("WindowButton");
					switch (m_NeighbourhoodTribeSet)
					{
					case Tribeset.MainTribes:
					case Tribeset.Promo:
					case Tribeset.Spring:
					case Tribeset.CrystalGem:
						gameObject2 = House_RegularFurbling(theWindowCommensurateWithLevel, furbling, gameObject3);
						break;
					case Tribeset.Golden:
					case Tribeset.CrystalGolden:
						gameObject2 = House_GoldFurbling(theWindowCommensurateWithLevel, gameObject3);
						break;
					}
					PlayroomCue_FromHood playroomCue_FromHood2 = gameObject3.AddComponent<PlayroomCue_FromHood>();
					gameObject3.AddComponent<BoxCollider>();
					UIDragCamera uIDragCamera = gameObject3.AddComponent<UIDragCamera>();
					uIDragCamera.draggableCamera = (UIDraggableCamera)Object.FindObjectOfType(typeof(UIDraggableCamera));
					playroomCue_FromHood2.FurbyBaby = furbling;
					playroomCue_FromHood2.IsActive = true;
					if (furbling == m_TheFurblingInFocus)
					{
						gameObject2.SetActive(false);
						m_newArrivalFurblingWindow = theWindowCommensurateWithLevel;
					}
					else
					{
						EnableWindow(theWindowCommensurateWithLevel);
					}
				}
			}
			if (furbling.Tribe.Name == "Gold")
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_GoldFurblingPresent);
			}
			if (furbling.Tribe.Name == "CrystalGem")
			{
				GameEventRouter.SendEvent(HoodEvents.Hood_CrystalGemFurblingPresent);
			}
		}

		private GameObject House_GoldFurbling(GameObject targetWindow, GameObject windowButton)
		{
			GameObject gameObject = targetWindow.transform.Find("NeighbourhoodFurby").gameObject;
			gameObject.layer = targetWindow.layer;
			gameObject.transform.parent = targetWindow.transform;
			gameObject.transform.localPosition = Vector3.zero;
			windowButton.transform.parent = gameObject.transform;
			windowButton.transform.localPosition = Vector3.zero;
			windowButton.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			windowButton.transform.localScale = new Vector3(10f, 10f, 1f);
			windowButton.layer = targetWindow.layer;
			return gameObject;
		}

		private GameObject House_RegularFurbling(GameObject targetWindow, FurbyBaby furbling, GameObject windowButton)
		{
			GameObject gameObject = new GameObject("NeighbourhoodFurby");
			gameObject.layer = targetWindow.layer;
			gameObject.transform.parent = targetWindow.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			string text = furbling.PlayroomCustomizations.Theme;
			if (string.IsNullOrEmpty(text))
			{
				text = ((furbling.Tribe.TribeSet != Tribeset.CrystalGem) ? "PLAYROOMITEM_THEME_FURBYPATTERN" : "PLAYROOMITEM_THEME_CRYSTALFURBY");
			}
			Transform transform = AddSprite(m_RoomAtlas, gameObject.transform, "WindowBG", text, 0, m_windowSize, UIWidget.Pivot.Center);
			transform.transform.localPosition = new Vector3(1f, 0f, 5f);
			GameObject gameObject2 = (GameObject)Object.Instantiate(m_furblingThumbPrefab);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, -22f, 0f);
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.layer = targetWindow.layer;
			FurblingThumbPrefab component = gameObject2.GetComponent<FurblingThumbPrefab>();
			component.SetFurbySprite(Path.GetFileName(FurbyGlobals.BabyLibrary.GetBabyFurby(furbling.Type).GetColoringAssetBundleName()));
			string[] flairs = furbling.m_persistantData.flairs;
			foreach (string text2 in flairs)
			{
				Transform transform2 = AddSprite(m_furblingAtlas, gameObject2.transform, "Flair", "FLAIR_" + text2, 2, component.GetPixelSize(), UIWidget.Pivot.Bottom);
				transform2.localPosition = component.GetOffset();
			}
			windowButton.transform.parent = gameObject.transform;
			windowButton.transform.localPosition = Vector3.zero;
			windowButton.transform.localRotation = gameObject.transform.localRotation;
			windowButton.transform.localScale = m_windowSize;
			windowButton.layer = targetWindow.layer;
			return gameObject;
		}

		public void EnableWindow(GameObject targetWindow)
		{
			Transform transform = targetWindow.transform.Find("Tower_window");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			Transform transform2 = targetWindow.transform.Find("NeighbourhoodFurby");
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(true);
			}
		}

		private void CacheTowerIdentities()
		{
			foreach (TowerIdent tower in m_Towers)
			{
				tower.IterationWindows = new List<GameObject>();
				foreach (Transform item in tower.m_TribeWindowRoot.transform)
				{
					tower.IterationWindows.Add(item.gameObject);
				}
			}
		}

		private GameObject GetTheWindowCommensurateWithLevel(TowerIdent targetTower, FurbyBaby b)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform item in targetTower.m_TribeWindowRoot.transform)
			{
				list.Add(item.gameObject);
			}
			if (b.Tribe.TribeSet == Tribeset.Promo)
			{
				int index = b.Iteration - 1;
				return list[index];
			}
			return list[b.NeighbourhoodIndex];
		}

		private bool ShouldFlagBeEnabled(TowerIdent targetTower, bool excludeCurrentFurbling)
		{
			List<FurbyBaby> babiesInHoodOfTribe = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribe(targetTower.m_TribeType);
			if (excludeCurrentFurbling && targetTower.m_TribeType.Equals(m_TheFurblingInFocus.Tribe))
			{
				babiesInHoodOfTribe.Remove(m_TheFurblingInFocus);
			}
			List<FurbyBabyTypeID> list = babiesInHoodOfTribe.Select((FurbyBaby furbling) => furbling.Type).ToList();
			bool result = true;
			foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in targetTower.m_TribeType.UnlockLevels)
			{
				FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
				foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
				{
					if (!list.Contains(furbyBabyTypeInfo.TypeID))
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public void DEBUG_StartSequence()
		{
			GameObject gameObject = GameObject.Find("SENTINEL_ShowRevealSequence");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			gameObject = new GameObject("SENTINEL_ShowRevealSequence");
			HoodStartup_FromEoG hoodStartup_FromEoG = gameObject.AddComponent<HoodStartup_FromEoG>();
			if (m_NeighbourhoodTribeSet == Tribeset.MainTribes)
			{
				hoodStartup_FromEoG.DEBUG_InvokeSequenceMain();
			}
			if (m_NeighbourhoodTribeSet == Tribeset.Spring)
			{
				hoodStartup_FromEoG.DEBUG_InvokeSequenceMain();
			}
		}
	}
}
