using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public struct FurbyBabyTypeID
	{
		[SerializeField]
		private FurbyTribeType m_tribe;

		[SerializeField]
		private int m_iteration;

		public FurbyTribeType Tribe
		{
			get
			{
				return m_tribe;
			}
			set
			{
				m_tribe = value;
			}
		}

		public int Iteration
		{
			get
			{
				return m_iteration;
			}
			set
			{
				m_iteration = value;
			}
		}

		public FurbyBabyTypeID(FurbyTribeType tribe, int iteration)
		{
			m_tribe = tribe;
			m_iteration = iteration;
		}

		public override string ToString()
		{
			AdultFurbyType adultFurbyType = AdultFurbyType.Checkerboard;
			AdultFurbyType adultFurbyType2 = AdultFurbyType.Checkerboard;
			string text = "?";
			if (m_tribe != null)
			{
				text = m_tribe.Name;
				adultFurbyType = m_tribe.Mamma;
				adultFurbyType2 = m_tribe.Pappa;
			}
			return string.Format("{0}({1}-{2})#{3}", text, adultFurbyType, adultFurbyType2, m_iteration);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			FurbyBabyTypeID furbyBabyTypeID = (FurbyBabyTypeID)obj;
			return GetHashCode() == furbyBabyTypeID.GetHashCode();
		}

		public override int GetHashCode()
		{
			return m_tribe.GetHashCode() + m_iteration * 57;
		}
	}
}
