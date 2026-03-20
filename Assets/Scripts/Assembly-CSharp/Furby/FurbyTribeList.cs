using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyTribeList : ScriptableObject
	{
		[EasyEditArray]
		[SerializeField]
		private List<FurbyTribeType> m_tribes = new List<FurbyTribeType>();

		[SerializeField]
		public int m_LastIndex_MainTribes;

		[SerializeField]
		public int m_LastIndex_Spring;

		[SerializeField]
		public int m_LastIndex_Crystal;

		public List<FurbyTribeType> List
		{
			get
			{
				int count = m_tribes.Count;
				count = (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal ? (m_LastIndex_Crystal + 1) : ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring) ? (m_LastIndex_MainTribes + 1) : (m_LastIndex_Spring + 1)));
				return m_tribes.GetRange(0, count);
			}
		}

		public List<FurbyTribeType> ListAll
		{
			get
			{
				return m_tribes;
			}
		}

		public string[] Names
		{
			get
			{
				string[] array = new string[m_tribes.Count];
				int num = 0;
				foreach (FurbyTribeType item in List)
				{
					array[num++] = item.Name;
				}
				return array;
			}
		}

		public int IndexOf(string tribeName)
		{
			return IndexOf(tribeName, false);
		}

		public int IndexOf(string tribeName, bool listAll)
		{
			int num = 0;
			foreach (FurbyTribeType item in (!listAll) ? List : ListAll)
			{
				if (item.Name.Equals(tribeName))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public int IndexOf(FurbyTribeType t)
		{
			return IndexOf(t.Name);
		}
	}
}
