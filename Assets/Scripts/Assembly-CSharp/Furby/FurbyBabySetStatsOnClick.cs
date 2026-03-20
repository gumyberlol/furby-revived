using UnityEngine;

namespace Furby
{
	public class FurbyBabySetStatsOnClick : MonoBehaviour
	{
		[SerializeField]
		private float m_attention = 1f;

		[SerializeField]
		private float m_satiatedness = 1f;

		[SerializeField]
		private float m_cleanliness = 1f;

		private void OnClick()
		{
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			if (inProgressFurbyBaby != null)
			{
				inProgressFurbyBaby.m_persistantData.Satiatedness = m_satiatedness;
				inProgressFurbyBaby.m_persistantData.Cleanliness = m_cleanliness;
				inProgressFurbyBaby.m_persistantData.Attention = m_attention;
				inProgressFurbyBaby.m_persistantData.NewSatiatedness = m_satiatedness;
				inProgressFurbyBaby.m_persistantData.NewCleanliness = m_cleanliness;
				inProgressFurbyBaby.m_persistantData.NewAttention = m_attention;
			}
		}
	}
}
