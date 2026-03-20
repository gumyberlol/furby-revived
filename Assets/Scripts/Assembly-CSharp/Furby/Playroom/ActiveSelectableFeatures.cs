using System;

namespace Furby.Playroom
{
	[Serializable]
	public class ActiveSelectableFeatures : AllSelectableFeatures
	{
		public int m_ActiveIndex = -1;

		public void Select(int index)
		{
			RemoveCurrentCustomizable();
			AddCustomizableToScene(index);
		}

		private void RemoveCurrentCustomizable()
		{
			if (m_ActiveIndex != -1)
			{
				base.Features[m_ActiveIndex].RemoveFromScene();
			}
		}

		private void AddCustomizableToScene(int index)
		{
			if (base.Features.Count > 0)
			{
				base.Features[index].AddToScene();
				m_ActiveIndex = index;
			}
		}
	}
}
