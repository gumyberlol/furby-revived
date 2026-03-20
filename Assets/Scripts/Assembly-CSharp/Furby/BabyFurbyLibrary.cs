using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyFurbyLibrary : RelentlessMonoBehaviour
	{
		private Dictionary<FurbyBabyTypeID, FurbyBabyTypeInfo> m_furbyDictionary = new Dictionary<FurbyBabyTypeID, FurbyBabyTypeInfo>();

		[SerializeField]
		private FurbyTribeList m_tribeList;

		[SerializeField]
		private int m_maxEggsInCarton = 12;

		private FurbyBabyTypeInfo[] m_furbiesBabies;

		[SerializeField]
		private FurbyBabyStatRates m_inGameRates;

		[SerializeField]
		private FurbyBabyStatRates m_outOfGameRates;

		public List<FurbyBabyTypeInfo> TypeList
		{
			get
			{
				return new List<FurbyBabyTypeInfo>(m_furbiesBabies);
			}
		}

		public FurbyTribeList TribeList
		{
			get
			{
				return m_tribeList;
			}
		}

		public FurbyBabyStatRates InGameStatRates
		{
			get
			{
				return m_inGameRates;
			}
		}

		public FurbyBabyStatRates OutOfGameStatRates
		{
			get
			{
				return m_outOfGameRates;
			}
		}

		private void Awake()
		{
			BuildDictionary();
		}

		public void ClearDictionary()
		{
			if (m_furbyDictionary.Count > 0)
			{
				Logging.Log("BabyFurbyLibrary [" + base.name + "] Contains [" + m_furbyDictionary.Count + "] Furblings -> DELETING!");
			}
			m_furbyDictionary.Clear();
		}

		public void BuildDictionary()
		{
			m_furbyDictionary.Clear();
			m_furbiesBabies = (from tribe in m_tribeList.List
				from unlockLevel in tribe.UnlockLevels
				from babyType in unlockLevel.BabyTypes
				select babyType).ToArray();
			FurbyBabyTypeInfo[] furbiesBabies = m_furbiesBabies;
			foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in furbiesBabies)
			{
				try
				{
					m_furbyDictionary.Add(furbyBabyTypeInfo.TypeID, furbyBabyTypeInfo);
				}
				catch (ArgumentException)
				{
				}
			}
			string text = "BabyFurbyLibrary [" + base.name + "] Contains [" + m_furbyDictionary.Count + "] Furblings\nThe list follows...\n";
			foreach (KeyValuePair<FurbyBabyTypeID, FurbyBabyTypeInfo> item in m_furbyDictionary)
			{
				text = text + item.ToString() + "\n";
			}
			Logging.Log(text);
		}

		public FurbyBabyTypeInfo GetBabyFurby(FurbyBabyTypeID type)
		{
			if (m_furbyDictionary.ContainsKey(type))
			{
				return m_furbyDictionary[type];
			}
			Logging.LogError("FurbyBabyLibrary::GetBabyFurby Furbling type: [" + type.ToString() + "] does NOT exist in the dictionary... Returning the first entry so the game can continue...");
			using (Dictionary<FurbyBabyTypeID, FurbyBabyTypeInfo>.Enumerator enumerator = m_furbyDictionary.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current.Value;
				}
			}
			return null;
		}

		public FurbyTribeType GetTribeByName(string name)
		{
			return GetTribeByName(name, false);
		}

		public FurbyTribeType GetTribeByName(string name, bool listAll)
		{
			if (!m_tribeList)
			{
				return null;
			}
			int num = m_tribeList.IndexOf(name, listAll);
			if (num < 0)
			{
				return null;
			}
			if (listAll)
			{
				return m_tribeList.ListAll[num];
			}
			return m_tribeList.List[num];
		}

		public int GetMaxEggsInCarton()
		{
			return m_maxEggsInCarton;
		}
	}
}
