using UnityEngine;

namespace Furby
{
	public class FurbyBabyTypeInfo : ScriptableObject
	{
		[SerializeField]
		private FurbyTribeType m_tribe;

		[SerializeField]
		private int m_iteration;

		[SerializeField]
		private string m_code = "FURBY";

		[SerializeField]
		private int m_xpToLevelUp = 1000;

		public FurbyBabyTypeID TypeID
		{
			get
			{
				return new FurbyBabyTypeID(m_tribe, m_iteration);
			}
		}

		public FurbyTribeType Tribe
		{
			get
			{
				return m_tribe;
			}
		}

		public int Iteration
		{
			get
			{
				return m_iteration;
			}
		}

		public string Code
		{
			get
			{
				return m_code;
			}
		}

		public int XpToLevelUp
		{
			get
			{
				return m_xpToLevelUp;
			}
		}

		public string GetColoringAssetBundleName()
		{
			return "FurbyBabyColoring/" + base.name + "_coloring";
		}
	}
}
