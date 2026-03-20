using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class SceneReferences
	{
		public GameObject m_VideoFeed;

		public GameObject m_BackButton;

		public GameObject m_BannerTitle;

		public GameObject m_VFXSpiral;

		public GameObject m_UnlockBurst;

		public GameObject m_VFXUnlock;

		public GameObject m_UnlockedItem;

		public GameObject m_ContinueButton_ToPantry;

		public GameObject m_ContinueButton_ToDashboard;

		public GameObject m_ContinueButton_ToEggCarton;

		public GameObject m_ContinueButton_ToPlayroomViaHood;

		public UISprite m_UnlockedItemTarget;

		public GameObject m_ScanBlanker;

		public GameObject m_ScanText;

		public GameObject m_ScanBar;

		public CodeReader m_CodeReader;

		public SlotPopulator m_SlotPopulator;
	}
}
