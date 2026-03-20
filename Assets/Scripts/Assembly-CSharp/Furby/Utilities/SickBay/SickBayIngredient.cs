using Relentless;
using UnityEngine;

namespace Furby.Utilities.SickBay
{
	public class SickBayIngredient : RelentlessMonoBehaviour
	{
		public enum IngredientType
		{
			Cola = 0,
			Honey = 1,
			HotWater = 2,
			IceCube = 3,
			Mints = 4
		}

		[SerializeField]
		private IngredientType m_IngredientType;

		private bool m_IsCureIngredient;

		public void SetAsCureIngredient()
		{
			m_IsCureIngredient = true;
		}

		public bool IsCureIngredient()
		{
			return m_IsCureIngredient;
		}

		public IngredientType GetIngredientType()
		{
			return m_IngredientType;
		}

		public void OnClick()
		{
			GameEventRouter.SendEvent(SickBayEvent.IngredientClicked, base.gameObject, this);
		}
	}
}
