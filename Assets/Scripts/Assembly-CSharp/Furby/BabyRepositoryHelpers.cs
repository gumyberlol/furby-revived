using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyRepositoryHelpers : RelentlessMonoBehaviour
	{
		private GameEventSubscription m_debugPanelSub;

		[SerializeField]
		private FurbyNamingData m_namingData;

		public IEnumerable<FurbyBaby> AllFurblings
		{
			get
			{
				int numFurblings = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
				for (int index = 0; index < numFurblings; index++)
				{
					yield return Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(index);
				}
			}
		}

		public IEnumerable<FurbyBaby> EggCarton
		{
			get
			{
				return from x in GetBabiesOfProgress(FurbyBabyProgresss.E)
					where x != FurbyGlobals.Player.InProgressFurbyBaby
					select x;
			}
		}

		public IEnumerable<FurbyBaby> Playroom
		{
			get
			{
				return GetBabiesOfProgress(FurbyBabyProgresss.P);
			}
		}

		public IEnumerable<FurbyBaby> Neighbourhood
		{
			get
			{
				return GetBabiesOfProgress(FurbyBabyProgresss.N);
			}
		}

		private void OnEnable()
		{
			m_debugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_debugPanelSub.Dispose();
		}

		private static Type[] EnumValues<Type>()
		{
			return Enum.GetValues(typeof(Type)) as Type[];
		}

		private FurbyBabyProgresss TEST_MakeProgression(int randomIndex)
		{
			FurbyBabyProgresss[] array = EnumValues<FurbyBabyProgresss>();
			if (randomIndex < 0)
			{
				randomIndex = UnityEngine.Random.Range(0, array.Length);
			}
			return array[randomIndex];
		}

		public string TEST_MakeBabyNameLeft()
		{
			string[] array = new string[13]
			{
				"AH", "BEE", "DAH", "DAY", "DEE", "DOO", "KEE", "LOO", "MAY", "NOO",
				"TAY", "TOH", "WAY"
			};
			int num = UnityEngine.Random.Range(0, array.Length - 1);
			return array[num];
		}

		public bool HasCompletedTribe(FurbyTribeType tribeToLookUp)
		{
			bool result = true;
			List<FurbyBaby> babiesInHoodOfTribe = GetBabiesInHoodOfTribe(tribeToLookUp);
			foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in tribeToLookUp.UnlockLevels)
			{
				FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
				foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
				{
					bool flag = false;
					foreach (FurbyBaby item in babiesInHoodOfTribe)
					{
						if (item.Type.Equals(furbyBabyTypeInfo.TypeID))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public string TEST_MakeBabyNameRight()
		{
			string[] array = new string[10] { "BAY", "BOH", "BOO", "DAH", "DOH", "DOO", "KAH", "KOH", "TAH", "TOH" };
			int num = UnityEngine.Random.Range(0, array.Length - 1);
			return array[num];
		}

		public void TEST_MakeOneOfEachBaby(FurbyBabyProgresss in_progress, bool includeGold)
		{
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			ClearAll();
			FurbyGlobals.Player.InProgressFurbyBaby = null;
			int count = typeList.Count;
			for (int i = 0; i < count; i++)
			{
				if (includeGold || typeList[i].Tribe.TribeSet != Tribeset.Golden)
				{
					FurbyBaby furbyBaby = CreateNewBaby(typeList[i].TypeID);
					furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
					furbyBaby.NameRight = TEST_MakeBabyNameRight();
					furbyBaby.Progress = in_progress;
					switch (typeList[i].Tribe.TribeSet)
					{
					case Tribeset.MainTribes:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					case Tribeset.Promo:
						furbyBaby.NeighbourhoodIndex = i % 12;
						break;
					case Tribeset.Golden:
						furbyBaby.NeighbourhoodIndex = 0;
						break;
					case Tribeset.Spring:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					case Tribeset.CrystalGem:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					}
					furbyBaby.Iteration = typeList[i].Iteration;
					FurbyBabyPersonality[] array = Enum.GetValues(typeof(FurbyBabyPersonality)).OfType<FurbyBabyPersonality>().Skip(1)
						.ToArray();
					furbyBaby.SetPersonality(array[UnityEngine.Random.Range(0, array.Length)], furbyBaby.Level);
				}
			}
			TEST_CreatePlayroomFurby();
		}

		public void TEST_CreatePlayroomFurby()
		{
			FurbyBabyTypeInfo furbyBabyTypeInfo = FurbyGlobals.BabyLibrary.TypeList[UnityEngine.Random.Range(0, FurbyGlobals.BabyLibrary.TypeList.Count)];
			FurbyBaby furbyBaby = CreateNewBaby(furbyBabyTypeInfo.TypeID);
			furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
			furbyBaby.NameRight = TEST_MakeBabyNameRight();
			furbyBaby.Progress = FurbyBabyProgresss.P;
			furbyBaby.Iteration = furbyBabyTypeInfo.Iteration;
			FurbyBabyPersonality[] array = Enum.GetValues(typeof(FurbyBabyPersonality)).OfType<FurbyBabyPersonality>().Skip(1)
				.ToArray();
			furbyBaby.SetPersonality(array[UnityEngine.Random.Range(0, array.Length)], furbyBaby.Level);
			FurbyGlobals.Player.InProgressFurbyBaby = furbyBaby;
		}

		public void TEST_MakeWorstCaseScenario(FurbyBabyProgresss in_progress, int numDuplicates)
		{
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			ClearAll();
			int count = typeList.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < numDuplicates; j++)
				{
					FurbyBaby furbyBaby = CreateNewBaby(typeList[i].TypeID);
					furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
					furbyBaby.NameRight = TEST_MakeBabyNameRight();
					furbyBaby.Progress = in_progress;
					furbyBaby.NeighbourhoodIndex = i % 8;
					furbyBaby.Iteration = typeList[i].Iteration;
				}
			}
			TEST_CreatePlayroomFurby();
		}

		public void TEST_MakeAFullTribe(FurbyBabyProgresss in_progress, int numDuplicates)
		{
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			ClearAll();
			int num = UnityEngine.Random.Range(0, 6);
			int num2 = num * 8;
			for (int i = num2; i < num2 + 8; i++)
			{
				for (int j = 0; j < numDuplicates; j++)
				{
					FurbyBaby furbyBaby = CreateNewBaby(typeList[i].TypeID);
					furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
					furbyBaby.NameRight = TEST_MakeBabyNameRight();
					furbyBaby.Progress = in_progress;
					furbyBaby.NeighbourhoodIndex = i % 8;
					furbyBaby.Iteration = typeList[i].Iteration;
				}
			}
		}

		public int GetPopulationCount(FurbyBabyProgresss in_progress)
		{
			int num = 0;
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == in_progress)
				{
					num++;
				}
			}
			return num;
		}

		private List<FurbyBabyTypeInfo> GetTribeset(Tribeset tribe)
		{
			List<FurbyBabyTypeInfo> list = new List<FurbyBabyTypeInfo>();
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			foreach (FurbyBabyTypeInfo item in typeList)
			{
				if (item.Tribe.TribeSet.Equals(tribe))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public void GenerateEverything(FurbyBabyProgresss desiredProgress)
		{
			ClearAll();
			GenerateFurblingsFromTypeInfoList(FurbyGlobals.BabyLibrary.TypeList, desiredProgress, 1);
			TEST_CreatePlayroomFurby();
		}

		public void ClearEverything()
		{
			ClearAll();
		}

		public void GenerateFullTribe(Tribeset tribe, FurbyBabyProgresss desiredProgress)
		{
			List<FurbyBabyTypeInfo> tribeset = GetTribeset(tribe);
			GenerateFurblingsFromTypeInfoList(tribeset, desiredProgress, 1);
		}

		public void GenerateFullTribe_WithDuplicates(Tribeset tribe, FurbyBabyProgresss desiredProgress, int numberOfEachType)
		{
			List<FurbyBabyTypeInfo> tribeset = GetTribeset(tribe);
			GenerateFurblingsFromTypeInfoList(tribeset, desiredProgress, numberOfEachType);
		}

		public void GenerateFurblingsFromTypeInfoList(List<FurbyBabyTypeInfo> list, FurbyBabyProgresss in_progress, int numberOfEachType)
		{
			FurbyBabyPersonality[] array = Enum.GetValues(typeof(FurbyBabyPersonality)).OfType<FurbyBabyPersonality>().Skip(1)
				.ToArray();
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < numberOfEachType; j++)
				{
					FurbyBabyTypeInfo furbyBabyTypeInfo = list[i];
					FurbyBabyTypeID typeID = furbyBabyTypeInfo.TypeID;
					FurbyBaby furbyBaby = CreateNewBaby(typeID);
					furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
					furbyBaby.NameRight = TEST_MakeBabyNameRight();
					furbyBaby.Iteration = furbyBabyTypeInfo.Iteration;
					furbyBaby.Progress = in_progress;
					furbyBaby.SetPersonality(array[UnityEngine.Random.Range(0, array.Length)], furbyBaby.Level);
					switch (list[i].Tribe.TribeSet)
					{
					case Tribeset.MainTribes:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					case Tribeset.Promo:
						furbyBaby.NeighbourhoodIndex = i % 12;
						break;
					case Tribeset.Golden:
						furbyBaby.NeighbourhoodIndex = 0;
						break;
					case Tribeset.Spring:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					case Tribeset.CrystalGem:
						furbyBaby.NeighbourhoodIndex = i % 8;
						break;
					}
				}
			}
		}

		public void TEST_MakeBabies(int numBabies)
		{
			numBabies--;
			int seed = UnityEngine.Random.seed;
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			ClearAll();
			UnityEngine.Random.seed = (int)(DateTime.Now.Ticks % int.MaxValue);
			for (int i = 0; i < numBabies; i++)
			{
				int index = UnityEngine.Random.Range(0, typeList.Count);
				FurbyBabyTypeInfo furbyBabyTypeInfo = typeList[index];
				FurbyBaby furbyBaby = CreateNewBaby(furbyBabyTypeInfo.TypeID);
				furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
				furbyBaby.NameRight = TEST_MakeBabyNameRight();
				furbyBaby.SetPersonality((FurbyBabyPersonality)UnityEngine.Random.Range(1, 16), furbyBaby.Level);
				furbyBaby.Progress = FurbyBabyProgresss.P;
				Logging.Log(string.Format("Ooops, I just gave birth to {0}", furbyBaby.Name));
			}
			UnityEngine.Random.seed = seed;
			TEST_CreatePlayroomFurby();
		}

		public void TEST_MakeBabies_OneTribe(int nthTribe)
		{
			List<FurbyBabyTypeInfo> typeList = FurbyGlobals.BabyLibrary.TypeList;
			int num = nthTribe * 8;
			for (int i = num; i < num + 8; i++)
			{
				FurbyBaby furbyBaby = CreateNewBaby(typeList[i].TypeID);
				furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
				furbyBaby.NameRight = TEST_MakeBabyNameRight();
				furbyBaby.Progress = FurbyBabyProgresss.N;
				furbyBaby.NeighbourhoodIndex = i % 8;
				furbyBaby.Iteration = typeList[i].Iteration;
				furbyBaby.SetPersonality((FurbyBabyPersonality)UnityEngine.Random.Range(1, 16), furbyBaby.Level);
			}
		}

		public void TEST_MakeBabies_Manifest()
		{
			int seed = UnityEngine.Random.seed;
			int num = 0;
			UnityEngine.Random.seed = (int)(DateTime.Now.Ticks % int.MaxValue);
			ClearAll();
			foreach (FurbyBabyTypeInfo type in FurbyGlobals.BabyLibrary.TypeList)
			{
				FurbyBaby furbyBaby = CreateNewBaby(type.TypeID);
				furbyBaby.NameLeft = TEST_MakeBabyNameLeft();
				furbyBaby.NameRight = TEST_MakeBabyNameRight();
				furbyBaby.Progress = TEST_MakeProgression(num++);
				Logging.Log(string.Format("Ooops, I just gave birth to {0}", furbyBaby.Name));
			}
			TEST_CreatePlayroomFurby();
			UnityEngine.Random.seed = seed;
		}

		public int GetNumberOfHatchedFurblings()
		{
			int num = 0;
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.P || furbyBabyByIndex.Progress == FurbyBabyProgresss.N)
				{
					num++;
				}
			}
			return num;
		}

		public void TEST_PickARandomCurrentBaby(FurbyBabyProgresss progressType)
		{
			IEnumerable<FurbyBaby> babiesOfProgress = GetBabiesOfProgress(progressType);
			int num = babiesOfProgress.Count();
			if (num != 0)
			{
				int index = UnityEngine.Random.Range(0, num);
				FurbyBaby furbyBaby = babiesOfProgress.ElementAt(index);
				FurbyGlobals.Player.InProgressFurbyBaby = furbyBaby;
				Logging.Log(string.Format("Set {0} as the current baby", furbyBaby.Name));
			}
		}

		private void ClearAll()
		{
			Logging.Log("CLEARING, START");
			while (Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies() > 0)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(0);
				Singleton<GameDataStoreObject>.Instance.Data.RemoveFurbyBaby(furbyBabyByIndex);
			}
			Logging.Log("CLEARING, COMPLETE");
		}

		public FurbyBaby CreateNewBaby(FurbyBabyTypeID typeID)
		{
			return CreateNewBaby(typeID, false);
		}

		public FurbyBaby CreateNewBaby(FurbyBabyTypeID typeID, bool listAll)
		{
			FurbyBaby furbyBaby = FurbyBaby.Create(typeID, m_namingData, listAll);
			furbyBaby.m_persistantData.LayingTime = DateTime.Now.Ticks;
			Singleton<GameDataStoreObject>.Instance.Data.AddNewFurbyBaby(furbyBaby);
			return furbyBaby;
		}

		public IEnumerable<FurbyBaby> GetBabiesOfProgress(FurbyBabyProgresss progressType)
		{
			int numFurblings = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int index = 0; index < numFurblings; index++)
			{
				FurbyBaby furbyBaby = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(index);
				if (progressType == furbyBaby.Progress)
				{
					yield return furbyBaby;
				}
			}
		}

		public List<FurbyBaby> GetBabiesInHoodOfTribe(FurbyTribeType tribeType)
		{
			List<FurbyBaby> list = new List<FurbyBaby>();
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Tribe == tribeType)
				{
					list.Add(furbyBabyByIndex);
				}
			}
			return list;
		}

		public List<FurbyBaby> GetBabiesInHoodOfTribeSet(Tribeset tribeset)
		{
			List<FurbyBaby> list = new List<FurbyBaby>();
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Tribe.TribeSet == tribeset)
				{
					list.Add(furbyBabyByIndex);
				}
			}
			return list;
		}

		public List<FurbyBaby> GetBabiesInHoodOfTribeAndLevel(FurbyTribeType tribeType, int level)
		{
			List<FurbyBaby> list = new List<FurbyBaby>();
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Tribe == tribeType && furbyBabyByIndex.Level == level)
				{
					list.Add(furbyBabyByIndex);
				}
			}
			return list;
		}

		public List<FurbyBaby> GetBabiesInHoodOfTribeLevelAndPattern(FurbyTribeType tribeType, int level, int iteration)
		{
			List<FurbyBaby> list = new List<FurbyBaby>();
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Tribe == tribeType && furbyBabyByIndex.Level == level && furbyBabyByIndex.Iteration == iteration)
				{
					list.Add(furbyBabyByIndex);
				}
			}
			return list;
		}

		public List<FurbyBaby> GetBabiesInHoodOfTribeLevelButNotPattern(FurbyTribeType tribeType, int level, int iteration)
		{
			List<FurbyBaby> list = new List<FurbyBaby>();
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N && furbyBabyByIndex.Tribe == tribeType && furbyBabyByIndex.Level == level && furbyBabyByIndex.Iteration != iteration)
				{
					list.Add(furbyBabyByIndex);
				}
			}
			return list;
		}

		public bool IsEggCartonFull()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				return true;
			}
			return false;
		}

		private int NumberInNeighbourhood()
		{
			int num = 0;
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Progress == FurbyBabyProgresss.N)
				{
					num++;
				}
			}
			return num;
		}

		private bool OnBabyGUI(FurbyBaby baby, int index)
		{
			bool result = false;
			bool flag = FurbyGlobals.Player.InProgressFurbyBaby == baby;
			GUILayout.BeginHorizontal();
			bool flag2 = GUILayout.Toggle(flag, GUIContent.none);
			if (flag != flag2)
			{
				FurbyGlobals.Player.InProgressFurbyBaby = baby;
				Application.LoadLevel(Application.loadedLevelName);
			}
			GUILayout.BeginVertical();
			if (DebugPanel.StartSection(string.Format("{0}{1} ({2})", baby.Progress, index, baby.Name)))
			{
				OnBabyGUI_ShowTribe(baby);
				OnBabyGUI_ShowPersonality(baby);
				OnBabyGUI_ShowProgress(baby);
				OnBabyGUI_ShowStats(baby);
				OnBabyGUI_ShowXP(baby);
				OnBabyGUI_ShowPersistantStatus(baby);
				OnBabyGUI_ShowFlairs(baby);
			}
			DebugPanel.EndSection();
			GUILayout.EndVertical();
			GUI.backgroundColor = new Color(0.88f, 0.25f, 0.25f);
			if (GUILayout.Button("DEL"))
			{
				Singleton<GameDataStoreObject>.Instance.Data.RemoveFurbyBaby(baby);
				Singleton<GameDataStoreObject>.Instance.Save();
				result = true;
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			return result;
		}

		private void OnBabyGUI_ShowStats(FurbyBaby baby)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Attention");
			baby.NewAttention = GUILayout.HorizontalSlider(baby.NewAttention, 0f, 1f, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Cleanliness");
			baby.NewCleanliness = GUILayout.HorizontalSlider(baby.NewCleanliness, 0f, 1f, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Satiatedness");
			baby.NewSatiatedness = GUILayout.HorizontalSlider(baby.NewSatiatedness, 0f, 1f, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		private void OnBabyGUI_ShowTribe(FurbyBaby baby)
		{
			GUILayout.BeginVertical();
			if (DebugPanel.StartSection(baby.Type.ToString()))
			{
				GUILayout.Label("Type: ");
				FurbyTribeList tribeList = FurbyGlobals.BabyLibrary.TribeList;
				string[] names = tribeList.Names;
				FurbyTribeType[] array = tribeList.List.ToArray();
				int num = tribeList.IndexOf(baby.Tribe);
				int num2 = GUILayout.SelectionGrid(num, names, 1);
				OnBabyGui_ShowPatternType(baby);
				if (num != num2)
				{
					baby.Tribe = array[num2];
					Application.LoadLevel(Application.loadedLevelName);
				}
			}
			DebugPanel.EndSection();
			GUILayout.EndVertical();
		}

		private void OnBabyGUI_ShowPersonality(FurbyBaby baby)
		{
			GUILayout.BeginVertical();
			if (DebugPanel.StartSection("Personality: " + baby.Personality))
			{
				string[] names = Enum.GetNames(typeof(FurbyBabyPersonality));
				FurbyBabyPersonality[] array = (FurbyBabyPersonality[])Enum.GetValues(typeof(FurbyBabyPersonality));
				int num = 0;
				FurbyBabyPersonality[] array2 = array;
				foreach (FurbyBabyPersonality furbyBabyPersonality in array2)
				{
					if (furbyBabyPersonality != baby.Personality)
					{
						num++;
						continue;
					}
					break;
				}
				int num2 = GUILayout.SelectionGrid(num, names, 1);
				if (num != num2)
				{
					baby.SetPersonality(array[num2], baby.Level);
					Application.LoadLevel(Application.loadedLevelName);
				}
			}
			DebugPanel.EndSection();
			GUILayout.EndVertical();
		}

		private void OnBabyGUI_ShowProgress(FurbyBaby baby)
		{
			FurbyBabyProgresss[] array = new FurbyBabyProgresss[2]
			{
				FurbyBabyProgresss.E,
				FurbyBabyProgresss.P
			};
			GUILayout.BeginVertical();
			if (DebugPanel.StartSection("Progress : " + baby.Progress))
			{
				FurbyBabyProgresss[] array2 = array;
				foreach (FurbyBabyProgresss furbyBabyProgresss in array2)
				{
					bool flag = furbyBabyProgresss == baby.Progress;
					bool flag2 = GUILayout.Toggle(flag, furbyBabyProgresss.ToString());
					if (flag != flag2)
					{
						baby.Progress = furbyBabyProgresss;
					}
				}
				if (baby.Progress == FurbyBabyProgresss.N)
				{
					GUILayout.Toggle(true, baby.Progress.ToString());
				}
			}
			DebugPanel.EndSection();
			GUILayout.EndVertical();
		}

		private void OnBabyGUI_ShowXP(FurbyBaby baby)
		{
			GUILayout.BeginVertical();
			if (DebugPanel.StartSection("XP : " + baby.XP))
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Current XP : ", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(baby.XP.ToString(), RelentlessGUIStyles.Style_Column);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("'Earnt' XP : ", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(baby.EarnedXP.ToString(), RelentlessGUIStyles.Style_Column);
				GUILayout.EndHorizontal();
				if (baby.XP < FurbyGlobals.BabyLibrary.GetBabyFurby(baby.Type).XpToLevelUp)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Req'd To Level Up: ", RelentlessGUIStyles.Style_Header);
					GUILayout.Label(FurbyGlobals.BabyLibrary.GetBabyFurby(baby.Type).XpToLevelUp.ToString(), RelentlessGUIStyles.Style_Column);
					GUILayout.EndHorizontal();
					if (GUILayout.Button("Level Up"))
					{
						baby.m_persistantData.XP = FurbyGlobals.BabyLibrary.GetBabyFurby(baby.Type).XpToLevelUp;
					}
				}
			}
			DebugPanel.EndSection();
			GUILayout.EndVertical();
		}

		private void OnBabyGui_ShowPatternType(FurbyBaby baby)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Pattern: ");
			GUILayout.BeginHorizontal();
			int[] array = new int[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
			string[] array2 = new string[8] { "1", "2", "3", "4", "5", "6", "7", "8" };
			int num = baby.Iteration - 1;
			int num2 = num;
			num2 = GUILayout.SelectionGrid(num, array2, array2.Length);
			if (num2 != num)
			{
				int iteration = array[num2];
				baby.Iteration = iteration;
				Application.LoadLevel(Application.loadedLevelName);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		private void OnBabyGUI_ShowFlairs(FurbyBaby baby)
		{
			if (DebugPanel.StartSection("Flair(s)"))
			{
				string[] flairs = baby.m_persistantData.flairs;
				foreach (string text in flairs)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Flair: ", RelentlessGUIStyles.Style_Header);
					GUILayout.Label(text, RelentlessGUIStyles.Style_Column);
					GUILayout.EndHorizontal();
				}
			}
			DebugPanel.EndSection();
		}

		private void OnBabyGUI_ShowPersistantStatus(FurbyBaby baby)
		{
			if (DebugPanel.StartSection("Persistent State"))
			{
				baby.m_persistantData.cameFromFriendsBook = GUILayout.Toggle(baby.m_persistantData.cameFromFriendsBook, "Came From Friendsbook");
				baby.m_persistantData.CanBeGifted = GUILayout.Toggle(baby.m_persistantData.CanBeGifted, "Can Be Gifted");
				baby.m_persistantData.FixedIncubationTime = GUILayout.Toggle(baby.m_persistantData.FixedIncubationTime, "Fixed Incubation Time");
				baby.m_persistantData.PreAllocatedPersonality = GUILayout.Toggle(baby.m_persistantData.PreAllocatedPersonality, "Has Pre-Allocated Personality");
			}
			DebugPanel.EndSection();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			bool flag = false;
			int num = 0;
			if (DebugPanel.StartSection("Eggs"))
			{
				int num2 = FurbyGlobals.BabyRepositoryHelpers.GetBabiesOfProgress(FurbyBabyProgresss.E).Count();
				if (num2 > 0)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Number of Eggs:", RelentlessGUIStyles.Style_Header);
					GUILayout.Label(num2.ToString(), RelentlessGUIStyles.Style_Column);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.Space(10f);
				}
				else
				{
					GUILayout.Label("No eggs!", RelentlessGUIStyles.Style_Header);
				}
				if (FurbyGlobals.Player.InProgressFurbyBaby != null && FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.E)
				{
					GUILayout.Label("Current:", RelentlessGUIStyles.Style_Header);
					OnBabyGUI(FurbyGlobals.Player.InProgressFurbyBaby, Singleton<GameDataStoreObject>.Instance.Data.InProgressBabyIndex);
				}
				if (num2 > 0)
				{
					GUILayout.Space(10f);
					GUILayout.Label("Egg List:", RelentlessGUIStyles.Style_Header);
					foreach (FurbyBaby item in FurbyGlobals.BabyRepositoryHelpers.EggCarton)
					{
						flag = OnBabyGUI(item, num++);
						if (flag)
						{
							break;
						}
					}
				}
			}
			DebugPanel.EndSection();
			if (DebugPanel.StartSection("Furblings"))
			{
				int num3 = FurbyGlobals.BabyRepositoryHelpers.GetBabiesOfProgress(FurbyBabyProgresss.N).Count();
				int num4 = FurbyGlobals.BabyRepositoryHelpers.GetBabiesOfProgress(FurbyBabyProgresss.P).Count();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Number in Playroom: ", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(num4.ToString(), RelentlessGUIStyles.Style_ColumnCentred);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Number in the Neighbourhood:", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(num3.ToString(), RelentlessGUIStyles.Style_ColumnCentred);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				if (num4 > 0)
				{
					GUILayout.Label("Playroom:", RelentlessGUIStyles.Style_Header);
					IEnumerable<FurbyBaby> playroom = FurbyGlobals.BabyRepositoryHelpers.Playroom;
					foreach (FurbyBaby item2 in playroom)
					{
						flag = OnBabyGUI(item2, num++);
						if (flag)
						{
							break;
						}
					}
				}
				GUILayout.Space(10f);
				if (num3 > 0)
				{
					GUILayout.Label("Neighbourhood:", RelentlessGUIStyles.Style_Header);
					IEnumerable<FurbyBaby> neighbourhood = FurbyGlobals.BabyRepositoryHelpers.Neighbourhood;
					foreach (FurbyBaby item3 in neighbourhood)
					{
						flag = OnBabyGUI(item3, num++);
						if (flag)
						{
							break;
						}
					}
				}
			}
			DebugPanel.EndSection();
			if (flag)
			{
				Singleton<GameDataStoreObject>.Instance.Save();
				Application.LoadLevel(Application.loadedLevelName);
			}
		}

		public void ProgressToNeighbourhood(FurbyBaby targetBaby)
		{
			targetBaby.Progress = FurbyBabyProgresss.N;
			targetBaby.NewSatiatedness = 1f;
			targetBaby.NewAttention = 1f;
			targetBaby.NewCleanliness = 1f;
			targetBaby.m_persistantData.Satiatedness = 1f;
			targetBaby.m_persistantData.Cleanliness = 1f;
			targetBaby.m_persistantData.Attention = 1f;
			targetBaby.m_persistantData.GraduationTime = DateTime.Now.Ticks;
			GameEventRouter.SendEvent(BabyLifecycleEvent.BabyGraduated, null, targetBaby);
		}

		public bool AssignRoomIndexPromo(FurbyBaby targetBaby)
		{
			if (targetBaby.Progress == FurbyBabyProgresss.N)
			{
				Logging.Log("ALREADY IN THE NEIGHBOURHOOD");
				return false;
			}
			foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
			{
				if (allFurbling.Type.Equals(targetBaby.Type))
				{
					targetBaby.NeighbourhoodIndex = allFurbling.NeighbourhoodIndex;
					return true;
				}
			}
			return false;
		}

		public bool AssignRoomIndex(FurbyBaby targetBaby)
		{
			if (targetBaby.Progress == FurbyBabyProgresss.N)
			{
				Logging.Log("ALREADY IN THE NEIGHBOURHOOD");
				return false;
			}
			int[][] array = new int[4][]
			{
				new int[2] { 0, 1 },
				new int[2] { 2, 3 },
				new int[3] { 4, 5, 6 },
				new int[1] { 7 }
			};
			bool flag = false;
			FurbyTribeType tribe = targetBaby.Tribe;
			int iteration = targetBaby.Iteration;
			int levelForIteration = FurbyBaby.GetLevelForIteration(iteration);
			if (levelForIteration >= array.Length)
			{
				Logging.LogError("AssignRoomIndexPromo should be used for Golden/Promo eggs");
				return false;
			}
			if (flag)
			{
				Logging.Log(string.Concat("AssignRoomIndex() Tribe:", tribe, ", Pattern:", iteration, ", Iteration:", levelForIteration));
			}
			int[] array2 = array[levelForIteration];
			List<FurbyBaby> babiesInHoodOfTribeAndLevel = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeAndLevel(tribe, levelForIteration);
			if (flag)
			{
				Logging.Log("AssignRoomIndex() Found " + babiesInHoodOfTribeAndLevel.Count + " matching this iteration...");
			}
			if (babiesInHoodOfTribeAndLevel.Count == 0)
			{
				if (flag)
				{
					Logging.Log("AssignRoomIndex() #1 RESOLVED! Chose RoomIndex:" + array2[0]);
				}
				if (flag)
				{
					Logging.Log(string.Concat("AssignRoomIndex() Tribe:", tribe, ", iteration_ob:", iteration, ", Level:", levelForIteration));
				}
				targetBaby.NeighbourhoodIndex = array2[0];
				return true;
			}
			if (flag)
			{
				Logging.Log("AssignRoomIndex() #1 UNRESOLVED! Looking closer.");
			}
			List<FurbyBaby> babiesInHoodOfTribeLevelAndPattern = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeLevelAndPattern(tribe, levelForIteration, iteration);
			if (flag)
			{
				Logging.Log("AssignRoomIndex() Found " + babiesInHoodOfTribeLevelAndPattern.Count + " at this iteration, matching our pattern...");
			}
			if (babiesInHoodOfTribeLevelAndPattern.Count > 0)
			{
				if (flag)
				{
					Logging.Log("AssignRoomIndex() #2 RESOLVED! Chose RoomIndex:" + babiesInHoodOfTribeLevelAndPattern[0].NeighbourhoodIndex);
				}
				if (flag)
				{
					Logging.Log(string.Concat("AssignRoomIndex() Tribe:", tribe, ", iteration_ob:", iteration, ", Level:", levelForIteration));
				}
				targetBaby.NeighbourhoodIndex = babiesInHoodOfTribeLevelAndPattern[0].NeighbourhoodIndex;
				return true;
			}
			if (flag)
			{
				Logging.Log("AssignRoomIndex() #2 UNRESOLVED! Looking closer.");
			}
			List<FurbyBaby> babiesInHoodOfTribeLevelButNotPattern = FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeLevelButNotPattern(tribe, levelForIteration, iteration);
			if (flag)
			{
				Logging.Log("AssignRoomIndex() Found " + babiesInHoodOfTribeLevelButNotPattern.Count + " at this iteration, NOT matching our pattern...");
			}
			List<int> list = new List<int>(array2);
			foreach (FurbyBaby item in babiesInHoodOfTribeLevelButNotPattern)
			{
				list.Remove(item.NeighbourhoodIndex);
			}
			if (list.Count <= 0)
			{
				if (flag)
				{
					Logging.Log("DAMN. Some exclusion didn't work...");
				}
				return false;
			}
			if (flag)
			{
				Logging.Log("AssignRoomIndex() #2 RESOLVED! Chose RoomIndex:" + list[0]);
			}
			if (flag)
			{
				Logging.Log(string.Concat("AssignRoomIndex() Tribe:", tribe, ", Pattern:", iteration, ", Iteration:", levelForIteration));
			}
			targetBaby.NeighbourhoodIndex = list[0];
			return true;
		}

		public bool ShouldGiveGoldEgg()
		{
			bool flag = false;
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Tribe.TribeSet == Tribeset.Golden)
				{
					flag = true;
					break;
				}
			}
			return !flag && HaveCompletedNTowers(6, Tribeset.MainTribes, Tribeset.Spring, Tribeset.CrystalGem);
		}

		public bool ShouldGiveCrystalGoldenEgg()
		{
			bool flag = false;
			int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
			for (int i = 0; i < numFurbyBabies; i++)
			{
				FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(i);
				if (furbyBabyByIndex.Tribe.TribeSet == Tribeset.CrystalGolden)
				{
					flag = true;
					break;
				}
			}
			return !flag && HaveCompletedNTowers(3, Tribeset.CrystalGem);
		}

		public bool HaveCompletedTheGame()
		{
			return HaveCompletedTheGame(false);
		}

		public bool HaveCompletedTheGame(bool crystal)
		{
			return (!crystal) ? HaveCompletedNTowers(9) : HaveCompletedNTowers(3, Tribeset.CrystalGem);
		}

		public bool HaveHatchedGoldenEgg()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeSet(Tribeset.Golden).Count > 0)
			{
				return true;
			}
			return false;
		}

		public bool HaveHatchedCrystalGoldenEgg()
		{
			return FurbyGlobals.BabyRepositoryHelpers.GetBabiesInHoodOfTribeSet(Tribeset.CrystalGolden).Count > 0;
		}

		private bool HaveCompletedNTowers(int numberOfTowersRequired)
		{
			return HaveCompletedNTowers(numberOfTowersRequired, Tribeset.MainTribes, Tribeset.Spring);
		}

		private bool HaveCompletedNTowers(int numberOfTowersRequired, params Tribeset[] validTribesets)
		{
			int num = 0;
			bool flag = false;
			string text = "Deciding if we have completed [" + numberOfTowersRequired + "] towers...";
			List<Tribeset> list = validTribesets.ToList();
			foreach (FurbyTribeType item in FurbyGlobals.BabyLibrary.TribeList.List)
			{
				if (list.Contains(item.TribeSet))
				{
					if (flag)
					{
						text = text + "\n\n-Looking at tribe " + item.ToString();
					}
					int num2 = item.UnlockLevels.Count();
					int num3 = 0;
					foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in item.UnlockLevels)
					{
						int num4 = unlockLevel.BabyTypes.Count();
						int num5 = 0;
						if (flag)
						{
							text = text + "\n--" + item.ToString() + " evaluating...";
						}
						FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
						foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
						{
							foreach (FurbyBaby allFurbling in FurbyGlobals.BabyRepositoryHelpers.AllFurblings)
							{
								if (allFurbling.Type.Equals(furbyBabyTypeInfo.TypeID))
								{
									num5++;
									if (flag)
									{
										string text2 = text;
										text = text2 + "\n--+ Iteration " + furbyBabyTypeInfo.TypeID.Iteration + " has been hatched!";
									}
									break;
								}
							}
						}
						if (flag)
						{
							string text2 = text;
							text = text2 + "\n-- " + item.ToString() + " " + num5 + " of " + num4 + " unlocks -> " + ((num5 < num4) ? "Incomplete" : "Completed");
						}
						if (num5 >= num4)
						{
							num3++;
						}
					}
					if (flag)
					{
						string text2 = text;
						text = text2 + "\n-" + item.ToString() + ": " + num3 + " of " + num2 + " levels -> " + ((num3 < num2) ? "Incomplete" : "Complete");
					}
					if (num3 >= num2)
					{
						num++;
					}
				}
				else if (flag)
				{
					text = text + "\n\n*** Ignoring Tribe " + item.TribeSet;
				}
			}
			if (flag)
			{
				string text2 = text;
				text = text2 + "\nCounted " + num + " where we needed " + numberOfTowersRequired;
			}
			if (flag)
			{
				Logging.Log(text);
			}
			return num >= numberOfTowersRequired;
		}
	}
}
