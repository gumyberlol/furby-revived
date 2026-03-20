using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyData : ScriptableObject
	{
		[SerializeField]
		private AdultFurbyType m_adultType;

		[SerializeField]
		private FurbyColouring m_colouring;

		[SerializeField]
		private FurbyTribeType m_tribe;

		[SerializeField]
		public int m_LastIndex_MainTribes;

		[SerializeField]
		public int m_LastIndex_Spring;

		[SerializeField]
		public int m_LastIndex_Crystal;

		[EasyEditArray]
		[SerializeField]
		private AdultFurbyType[] m_unlockOrder = new AdultFurbyType[12]
		{
			AdultFurbyType.Checkerboard,
			AdultFurbyType.Christmas,
			AdultFurbyType.Cubes,
			AdultFurbyType.Diagonals,
			AdultFurbyType.Diamonds,
			AdultFurbyType.Hearts,
			AdultFurbyType.Lightning,
			AdultFurbyType.Peacock,
			AdultFurbyType.Stripes,
			AdultFurbyType.Triangles,
			AdultFurbyType.Waves,
			AdultFurbyType.Zigzags
		};

		[SerializeField]
		[EasyEditArray]
		private FurbyBabyTypeInfo[] m_babyTypes;

		[SerializeField]
		private int m_maxFurbyBabiesOfType = 5;

		public AdultFurbyType AdultType
		{
			get
			{
				return m_adultType;
			}
		}

		public FurbyColouring Colouring
		{
			get
			{
				return m_colouring;
			}
		}

		public FurbyTribeType Tribe
		{
			get
			{
				return m_tribe;
			}
		}

		public IEnumerable<AdultFurbyType> UnlocksInOrder
		{
			get
			{
				return m_unlockOrder;
			}
		}

		public FurbyBabyTypeInfo[] BabyTypes
		{
			get
			{
				return m_babyTypes;
			}
		}

		public void CreateRandomUnlockOrder()
		{
			List<AdultFurbyType> list = new List<AdultFurbyType>((AdultFurbyType[])Enum.GetValues(typeof(AdultFurbyType)));
			list.Remove(AdultFurbyType.NoFurby);
			list.Remove(AdultFurbyType.PROMO);
			list.Remove(AdultFurbyType.Unknown);
			List<AdultFurbyType> list2 = new List<AdultFurbyType>();
			while (list.Count != 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count - 1);
				list2.Add(list[index]);
				list.RemoveAt(index);
			}
			m_unlockOrder = list2.ToArray();
		}

		private int GetNextBabyTypeIndex()
		{
			IEnumerable<FurbyBaby> allFurblings = FurbyGlobals.BabyRepositoryHelpers.AllFurblings;
			List<int> list = new List<int>();
			for (int i = 0; i < m_maxFurbyBabiesOfType; i++)
			{
				for (int j = 0; j < m_babyTypes.Length; j++)
				{
					int num = 0;
					foreach (FurbyBaby item in allFurblings)
					{
						if (item.Type.Equals(m_babyTypes[j].TypeID))
						{
							num++;
						}
					}
					if (num == i)
					{
						if (num == 0)
						{
							return j;
						}
						list.Add(j);
					}
				}
				if (list.Count != 0)
				{
					return list[UnityEngine.Random.Range(0, list.Count)];
				}
			}
			return -1;
		}

		private FurbyBabyTypeInfo GetGoldEgg()
		{
			foreach (FurbyTribeType item in FurbyGlobals.BabyLibrary.TribeList.List)
			{
				if (item.TribeSet != Tribeset.Golden)
				{
					continue;
				}
				foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in item.UnlockLevels)
				{
					FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
					int num = 0;
					if (num < babyTypes.Length)
					{
						return babyTypes[num];
					}
				}
			}
			return null;
		}

		private FurbyBabyTypeInfo GetCrystalGoldEgg()
		{
			foreach (FurbyTribeType item in FurbyGlobals.BabyLibrary.TribeList.List)
			{
				if (item.TribeSet != Tribeset.CrystalGolden)
				{
					continue;
				}
				using (IEnumerator<FurbyBabyTypeInfo> enumerator2 = item.UnlockLevels.SelectMany((FurbyTribeType.BabyUnlockLevel level) => level.BabyTypes).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						return enumerator2.Current;
					}
				}
			}
			return null;
		}

		public FurbyBabyTypeInfo GetNextBabyTypeFromVirtualFurby()
		{
			int nextBabyTypeIndex = GetNextBabyTypeIndex();
			if (FurbyGlobals.BabyRepositoryHelpers.ShouldGiveCrystalGoldenEgg())
			{
				return GetCrystalGoldEgg();
			}
			if (FurbyGlobals.BabyRepositoryHelpers.ShouldGiveGoldEgg())
			{
				return GetGoldEgg();
			}
			if (nextBabyTypeIndex >= 0)
			{
				return m_babyTypes[nextBabyTypeIndex];
			}
			return null;
		}

		public FurbyBabyTypeInfo GetNextBabyTypeFromToyFurby()
		{
			int nextBabyTypeIndex = GetNextBabyTypeIndex();
			if (FurbyGlobals.BabyRepositoryHelpers.ShouldGiveCrystalGoldenEgg())
			{
				return GetCrystalGoldEgg();
			}
			if (FurbyGlobals.BabyRepositoryHelpers.ShouldGiveGoldEgg())
			{
				return GetGoldEgg();
			}
			if (nextBabyTypeIndex >= 0)
			{
				return m_babyTypes[nextBabyTypeIndex];
			}
			return null;
		}
	}
}
