using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SpriteFurbyDisplay : RelentlessMonoBehaviour
	{
		public UILabel Cost;

		public int Index;

		public UISprite Sprite;

		[SerializeField]
		private GameObject m_unlockedObj;

		[SerializeField]
		private GameObject m_lockedObj;

		[SerializeField]
		private GameObject m_crystalLockedObj;

		[SerializeField]
		private UIGradientTiledSprite m_background;

		[SerializeField]
		private UIWidget[] m_tintedSections;

		[SerializeField]
		private UILabel m_speech;

		[SerializeField]
		private UILabel m_unlockSpeech;

		[SerializeField]
		private GameObject m_previouslyMetWidget;

		[NonSerialized]
		public Color GradientTop = Color.magenta;

		[NonSerialized]
		public Color GradientBottom = Color.blue;

		[NonSerialized]
		public Color EdgeTint = Color.yellow;

		[NonSerialized]
		public float Angle;

		[NonSerialized]
		public Vector3 Offset = Vector3.zero;

		[NonSerialized]
		public string Message = string.Empty;

		[NonSerialized]
		public string LockMessage = string.Empty;

		[SerializeField]
		private FurbyData m_furbyData;

		private Material m_furMaterial;

		private bool m_unlocked = true;

		public bool IsCrystal
		{
			get
			{
				return m_furbyData.Tribe.TribeSet == Tribeset.CrystalGem;
			}
		}

		public bool IsCrystalLocked
		{
			get
			{
				return !Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked && IsCrystal;
			}
		}

		public bool Unlocked
		{
			get
			{
				return m_unlocked;
			}
			set
			{
				m_unlocked = value;
				Refresh();
			}
		}

		public FurbyData Furby
		{
			get
			{
				return m_furbyData;
			}
			set
			{
				m_furbyData = value;
				Refresh();
			}
		}

		private void Start()
		{
			Refresh();
		}

		public void Refresh()
		{
			if ((bool)m_furbyData)
			{
				m_crystalLockedObj.SetActive(IsCrystalLocked);
				m_unlockedObj.SetActive(m_unlocked && !IsCrystalLocked);
				m_lockedObj.SetActive(!m_unlocked && !IsCrystalLocked);
				if (m_furbyData.Colouring.FurbySpriteName != string.Empty)
				{
					Sprite.spriteName = m_furbyData.Colouring.FurbySpriteName;
				}
				Cost.text = string.Format("{0}", FurbyGlobals.AdultLibrary.EggCost[Index]);
				m_background.mGradient.mGradient1 = GradientTop;
				m_background.mGradient.mGradient2 = GradientBottom;
				UIWidget[] tintedSections = m_tintedSections;
				foreach (UIWidget uIWidget in tintedSections)
				{
					uIWidget.color = EdgeTint;
				}
				Sprite.transform.localPosition = Offset;
				Sprite.transform.localRotation = Quaternion.Euler(0f, 0f, Angle);
				m_speech.text = Message;
				m_unlockSpeech.text = LockMessage;
				bool flag = Singleton<GameDataStoreObject>.Instance.Data.MetFurbies.Contains(m_furbyData.AdultType);
				if (m_previouslyMetWidget != null)
				{
					m_previouslyMetWidget.SetActive(flag);
				}
			}
		}
	}
}
