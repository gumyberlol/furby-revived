using System;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorPersonalityButton : MonoBehaviour
	{
		[SerializeField]
		private int m_FurbuckValue;

		[SerializeField]
		private UISprite m_CostIcon;

		[SerializeField]
		private UILabel m_CostText;

		[SerializeField]
		private UISprite m_OwnedIcon;

		[SerializeField]
		private UISprite m_PersonalityIcon;

		[SerializeField]
		private NGUILocaliser m_GenreLocaliser;

		[NonSerialized]
		private FurbyPersonality m_FurbyType;

		[NonSerialized]
		private bool m_PersonalityChange;

		[SerializeField]
		private IncubatorLogic m_GameLogic;

		public void SetPersonality(FurbyPersonality personalityType)
		{
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			m_PersonalityChange = personality != personalityType;
			m_FurbyType = personalityType;
			if (m_PersonalityChange)
			{
				switch (personalityType)
				{
				case FurbyPersonality.Gobbler:
					m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_GOBBLER";
					break;
				case FurbyPersonality.Kooky:
					m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_KOOKY";
					break;
				case FurbyPersonality.Base:
					m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_ROCKSTAR";
					break;
				case FurbyPersonality.SweetBelle:
					m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_SWEETBELLE";
					break;
				case FurbyPersonality.ToughGirl:
					m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_TOUGHGIRL";
					break;
				}
			}
			else
			{
				m_GenreLocaliser.LocalisedStringKey = "INCUBATOR_IMPRINT_FURBY";
				SetOwnershipStatus(true);
			}
			m_GenreLocaliser.UpdateUI();
			m_PersonalityIcon.spriteName = GetPersonalitySprite(personalityType.ToString());
		}

		private string GetPersonalitySprite(string personalityName)
		{
			personalityName = personalityName.ToLower();
			foreach (UIAtlas.Sprite sprite in m_PersonalityIcon.atlas.spriteList)
			{
				string text = sprite.name.ToLower();
				if (text.Contains(personalityName))
				{
					return sprite.name;
				}
			}
			return personalityName;
		}

		private void SetOwnershipStatus(bool alreadyOwned)
		{
			m_CostIcon.gameObject.SetActive(!alreadyOwned);
			m_OwnedIcon.gameObject.SetActive(alreadyOwned);
			m_CostText.gameObject.SetActive(!alreadyOwned);
		}

		private void OnClick()
		{
			int num = m_FurbuckValue;
			if (!m_PersonalityChange)
			{
				num = 0;
			}
			if (Singleton<FurbucksWallet>.Instance.Balance >= num)
			{
				Singleton<FurbucksWallet>.Instance.Balance -= num;
				GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Personality);
				m_GameLogic.ImprintFinished(m_FurbyType, m_PersonalityChange);
			}
		}
	}
}
