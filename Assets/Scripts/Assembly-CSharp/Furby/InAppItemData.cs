using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class InAppItemData
	{
		[SerializeField]
		public string m_DisplayName = "YouNeedToDefineMe_Name";

		[SerializeField]
		public string m_SpriteName = "YouNeedToDefineMe_Name";

		[SerializeField]
		public UIAtlas m_Atlas;
	}
}
