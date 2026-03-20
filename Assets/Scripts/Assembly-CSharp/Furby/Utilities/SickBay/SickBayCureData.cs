using System;

namespace Furby.Utilities.SickBay
{
	[Serializable]
	public class SickBayCureData
	{
		private const int NUM_INGREDIENTS = 2;

		public string m_CureName;

		public UIAtlas m_GraphicAtlas;

		public string m_GraphicName;

		public SickBayDisease m_CuredDisease;

		public SickBayIngredient.IngredientType[] m_Ingredients = new SickBayIngredient.IngredientType[2];

		public FurbyAction m_ItemReactionAction;

		public FurbyAction m_CureEffectAction;
	}
}
