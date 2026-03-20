using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorEffects : MonoBehaviour
	{
		[SerializeField]
		private Material m_PersonalityMaterial;

		[SerializeField]
		private Texture m_GobblerTexture;

		[SerializeField]
		private Texture m_SweetBelleTexture;

		[SerializeField]
		private Texture m_KookyTexture;

		[SerializeField]
		private Texture m_ToughGirlTexture;

		[SerializeField]
		private Texture m_RockStarTexture;

		private FurbyPersonality? personalityApplied;

		private void Update()
		{
			FurbyPersonality? latestPersonality = IncubatorLogic.FurbyBaby.GetLatestPersonality();
			FurbyPersonality? furbyPersonality = personalityApplied;
			if (furbyPersonality.GetValueOrDefault() == latestPersonality.GetValueOrDefault() && !(furbyPersonality.HasValue ^ latestPersonality.HasValue))
			{
				return;
			}
			if (latestPersonality.HasValue)
			{
				switch (latestPersonality.Value)
				{
				case FurbyPersonality.Gobbler:
					m_PersonalityMaterial.mainTexture = m_GobblerTexture;
					break;
				case FurbyPersonality.SweetBelle:
					m_PersonalityMaterial.mainTexture = m_SweetBelleTexture;
					break;
				case FurbyPersonality.Kooky:
					m_PersonalityMaterial.mainTexture = m_KookyTexture;
					break;
				case FurbyPersonality.ToughGirl:
					m_PersonalityMaterial.mainTexture = m_ToughGirlTexture;
					break;
				case FurbyPersonality.Base:
					m_PersonalityMaterial.mainTexture = m_RockStarTexture;
					break;
				}
			}
			personalityApplied = latestPersonality;
		}
	}
}
