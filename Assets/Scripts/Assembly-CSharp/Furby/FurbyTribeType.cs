using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyTribeType : ScriptableObject
	{
		[Serializable]
		public class BabyUnlockLevel
		{
			[EasyEditArray]
			public FurbyBabyTypeInfo[] BabyTypes;
		}

		[SerializeField]
		private string m_name;

		[SerializeField]
		private AdultFurbyType m_parentA;

		[SerializeField]
		private AdultFurbyType m_parentB;

		[SerializeField]
		private Tribeset m_tribeSet;

		[EasyEditArray]
		[SerializeField]
		private BabyUnlockLevel[] m_babyUnlockLevels;

		[SerializeField]
		private Cubemap m_tribeCubemap;

		[SerializeField]
		private Shader m_bitsShader;

		[SerializeField]
		private Shader m_faceShader;

		[SerializeField]
		private Shader m_eggShader;

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public AdultFurbyType Mamma
		{
			get
			{
				return m_parentA;
			}
			set
			{
				m_parentA = value;
			}
		}

		public AdultFurbyType Pappa
		{
			get
			{
				return m_parentB;
			}
			set
			{
				m_parentB = value;
			}
		}

		public Tribeset TribeSet
		{
			get
			{
				return m_tribeSet;
			}
		}

		public ICollection<BabyUnlockLevel> UnlockLevels
		{
			get
			{
				return m_babyUnlockLevels;
			}
		}

		public Cubemap Cubemap
		{
			get
			{
				return m_tribeCubemap;
			}
		}

		public Shader BitsShader
		{
			get
			{
				return m_bitsShader;
			}
		}

		public Shader FaceShader
		{
			get
			{
				return m_faceShader;
			}
		}

		public Shader EggShader
		{
			get
			{
				return m_eggShader;
			}
		}

		public bool IsBabyLevelUnlocked(int babyLevel)
		{
			for (int i = 0; i < babyLevel - 1; i++)
			{
				FurbyBabyTypeInfo[] babyTypes = m_babyUnlockLevels[i].BabyTypes;
				FurbyBabyTypeInfo babyType;
				for (int j = 0; j < babyTypes.Length; j++)
				{
					babyType = babyTypes[j];
					if (!FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Any((FurbyBaby x) => x.Type.Equals(babyType.TypeID)))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			FurbyTribeType furbyTribeType = (FurbyTribeType)obj;
			return m_name == furbyTribeType.m_name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
