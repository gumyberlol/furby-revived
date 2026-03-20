using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Neighbourhood
{
	public class HoodDebug : RelentlessMonoBehaviour
	{
		private GameEventSubscription m_DebugPanelSub;

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			ShowControlSection();
		}

		private void ShowControlSection()
		{
			if (DebugPanel.StartSection("Hood Control"))
			{
				GUILayout.Label("WARNING: Modifies save data!", RelentlessGUIStyles.Style_Warning);
				GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(10f));
				GUILayout.Label("[Status]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				GUILayout.Label("Currently: " + FurbyGlobals.BabyRepositoryHelpers.GetPopulationCount(FurbyBabyProgresss.N) + " furblings");
				GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(10f));
				GUILayout.Label("[Controls]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				bool flag = GUILayout.Button("Clear Everything", GUILayout.ExpandWidth(true));
				bool flag2 = GUILayout.Button("Complete Everything", GUILayout.ExpandWidth(true));
				bool flag3 = GUILayout.Button("Complete Everything (+Duplicates)", GUILayout.ExpandWidth(true));
				GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(10f));
				GUILayout.Label("[Complete Neighborhoods]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				GUILayout.BeginVertical();
				bool flag4 = GUILayout.Button("Main Neighborhood", GUILayout.ExpandWidth(true));
				bool flag5 = GUILayout.Button("Spring Neighborhood", GUILayout.ExpandWidth(true));
				bool flag6 = GUILayout.Button("Promo Neighborhood", GUILayout.ExpandWidth(true));
				bool flag7 = GUILayout.Button("Crystal Neighborhood", GUILayout.ExpandWidth(true));
				bool flag8 = GUILayout.Button("Add Golden Furby", GUILayout.ExpandWidth(true));
				bool flag9 = GUILayout.Button("Add Crystal Golden Furby", GUILayout.ExpandWidth(true));
				GUILayout.EndVertical();
				GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(10f));
				GUILayout.Label("[Complete Tribes]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				FurbyTribeType[] array = FurbyGlobals.BabyLibrary.TribeList.List.ToArray();
				string[] array2 = new string[array.Length];
				int num = 0;
				FurbyTribeType[] array3 = array;
				foreach (FurbyTribeType furbyTribeType in array3)
				{
					array2[num++] = furbyTribeType.Name;
				}
				int num2 = GUILayout.SelectionGrid(-1, array2, 1, GUILayout.ExpandWidth(true));
				if (num2 != -1)
				{
					FurbyTribeType furbyTribeType2 = array[num2];
					List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
					List<FurbyBabyTypeInfo> list = new List<FurbyBabyTypeInfo>();
					foreach (FurbyBabyTypeInfo item in typeList)
					{
						if (item.TypeID.Tribe == furbyTribeType2)
						{
							list.Add(item);
						}
					}
					FurbyGlobals.BabyRepositoryHelpers.GenerateFurblingsFromTypeInfoList(list, FurbyBabyProgresss.N, 1);
					ReloadScene();
				}
				if (flag)
				{
					FurbyGlobals.BabyRepositoryHelpers.ClearEverything();
					ReloadScene();
				}
				if (flag4)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.MainTribes, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag5)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Spring, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag6)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Promo, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag7)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.CrystalGem, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag2)
				{
					FurbyGlobals.BabyRepositoryHelpers.ClearEverything();
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.MainTribes, FurbyBabyProgresss.N);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Spring, FurbyBabyProgresss.N);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Promo, FurbyBabyProgresss.N);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.CrystalGem, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag3)
				{
					FurbyGlobals.BabyRepositoryHelpers.ClearEverything();
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe_WithDuplicates(Tribeset.MainTribes, FurbyBabyProgresss.N, 5);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe_WithDuplicates(Tribeset.Spring, FurbyBabyProgresss.N, 5);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe_WithDuplicates(Tribeset.CrystalGem, FurbyBabyProgresss.N, 5);
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Promo, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag8)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.Golden, FurbyBabyProgresss.N);
					ReloadScene();
				}
				if (flag9)
				{
					FurbyGlobals.BabyRepositoryHelpers.GenerateFullTribe(Tribeset.CrystalGolden, FurbyBabyProgresss.N);
					ReloadScene();
				}
			}
			DebugPanel.EndSection();
		}

		private void ShowPopulatorSection()
		{
			if (DebugPanel.StartSection("Hood Populator"))
			{
				GUILayout.Label("[Neighbourhood Towers]", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(string.Empty, GUILayout.Height(10f));
				GUI.backgroundColor = new Color(0.25f, 0.4f, 0.88f);
				bool flag = GUILayout.Button("Clear Everything");
				GUI.backgroundColor = Color.white;
				GUILayout.Label(string.Empty, GUILayout.Height(10f));
				GUILayout.BeginHorizontal();
				foreach (FurbyTribeType item in FurbyGlobals.BabyLibrary.TribeList.List)
				{
					GUILayout.BeginVertical();
					GUILayout.Label("[" + item.Name + "]", RelentlessGUIStyles.Style_Header);
					GUILayout.Label(string.Empty, GUILayout.Height(10f));
					GUI.backgroundColor = new Color(0.25f, 0.88f, 0.25f);
					bool flag2 = GUILayout.Button("Reload scene", GUILayout.ExpandWidth(true));
					GUI.backgroundColor = Color.white;
					foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in item.UnlockLevels)
					{
						GUILayout.BeginVertical();
						FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
						foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
						{
							bool flag3 = false;
							int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
							for (int j = 0; j < numFurbyBabies; j++)
							{
								FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(j);
								if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Type.Equals(furbyBabyTypeInfo.TypeID))
								{
									flag3 = true;
									break;
								}
							}
							string text = furbyBabyTypeInfo.TypeID.Tribe.Name + ", " + furbyBabyTypeInfo.TypeID.Iteration;
							if (flag3)
							{
								GUILayout.Label(text, GUILayout.Height(30f));
							}
							else if (GUILayout.Button(text, GUILayout.Height(30f)))
							{
								FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(furbyBabyTypeInfo.TypeID);
								furbyBaby.Progress = FurbyBabyProgresss.N;
								furbyBaby.NeighbourhoodIndex = furbyBabyTypeInfo.Iteration - 1;
							}
						}
						GUILayout.EndVertical();
						GUILayout.Label(string.Empty, GUILayout.Height(10f));
					}
					GUILayout.Label(string.Empty);
					if (flag2)
					{
						ReloadScene();
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
				if (flag)
				{
					FurbyGlobals.BabyRepositoryHelpers.ClearEverything();
					ReloadScene();
				}
			}
			DebugPanel.EndSection();
		}

		private void ReloadScene()
		{
			Singleton<GameDataStoreObject>.Instance.Save();
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
