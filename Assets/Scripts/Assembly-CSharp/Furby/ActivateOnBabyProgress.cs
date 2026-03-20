using UnityEngine;

namespace Furby
{
	public class ActivateOnBabyProgress : MonoBehaviour
	{
		public enum EnableType
		{
			ActivateGameObject = 0,
			EnableNGUIWidgets = 1
		}

		[SerializeField]
		private bool m_activeWithNoFurby = true;

		[SerializeField]
		private bool m_showEgg = true;

		[SerializeField]
		private bool m_showPlayroom = true;

		[SerializeField]
		private bool m_showNeighbourhood = true;

		[SerializeField]
		private bool m_useInProgressFurbling;

		[SerializeField]
		private EnableType m_enableType;

		[SerializeField]
		private Color m_disableColor;

		private void Start()
		{
			FurbyBaby furbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			if (m_useInProgressFurbling)
			{
				furbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			}
			bool flag = false;
			flag = ((furbyBaby == null) ? (!m_showEgg && !m_showPlayroom && !m_showNeighbourhood) : ((furbyBaby.Progress == FurbyBabyProgresss.E && m_showEgg) || (furbyBaby.Progress == FurbyBabyProgresss.P && m_showPlayroom) || (furbyBaby.Progress == FurbyBabyProgresss.N && m_showNeighbourhood)));
			flag &= m_activeWithNoFurby || !FurbyGlobals.Player.NoFurbyOnSaveGame();
			switch (m_enableType)
			{
			case EnableType.ActivateGameObject:
				base.gameObject.SetActive(flag);
				break;
			case EnableType.EnableNGUIWidgets:
				SetNGUIEnabled(flag);
				break;
			}
		}

		private void SetNGUIEnabled(bool enable)
		{
			if (!enable)
			{
				UIWidget[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIWidget>();
				foreach (UIWidget uIWidget in componentsInChildren)
				{
					Color color = uIWidget.color * m_disableColor;
					uIWidget.color = color;
				}
				BoxCollider[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<BoxCollider>();
				foreach (BoxCollider boxCollider in componentsInChildren2)
				{
					boxCollider.enabled = false;
				}
				SphereCollider[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<SphereCollider>();
				foreach (SphereCollider sphereCollider in componentsInChildren3)
				{
					sphereCollider.enabled = false;
				}
			}
		}
	}
}
