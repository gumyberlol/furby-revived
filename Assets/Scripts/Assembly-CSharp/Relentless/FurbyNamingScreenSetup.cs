using System.Linq;
using Furby;
using UnityEngine;

namespace Relentless
{
	public class FurbyNamingScreenSetup : RelentlessMonoBehaviour
	{
		private enum ScrollBarr
		{
			LeftScrollBar = 0,
			RightScrollBar = 1
		}

		[SerializeField]
		private FurbyNamingData m_namingData;

		[SerializeField]
		private GameObject m_gridItemPrefab;

		[SerializeField]
		private WhichName WhichName;

		[SerializeField]
		private UIGrid m_leftscrollViewGrid;

		[SerializeField]
		private UIGrid m_rightscrollViewGrid;

		[SerializeField]
		private UIDraggablePanel m_leftPanel;

		[SerializeField]
		private UIDraggablePanel m_rightPanel;

		private float m_LastCacheTime;

		private GameObject m_leftSelected;

		private GameObject m_rightSelected;

		private string m_leftSelectedName;

		private string m_rightSelectedName;

		private bool m_lastMovedHandWasLeft;

		private float m_lastMovementDirection;

		[SerializeField]
		private UILabel m_meaningLabel;

		public void Start()
		{
			if (WhichName == WhichName.BabyName)
			{
				FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
				if (inProgressFurbyBaby != null)
				{
					m_leftSelectedName = inProgressFurbyBaby.NameLeft;
					m_rightSelectedName = inProgressFurbyBaby.NameRight;
				}
				else
				{
					Logging.LogError("Trying to get furby baby name even though there is no furby baby");
				}
			}
			else
			{
				m_leftSelectedName = Singleton<GameDataStoreObject>.Instance.Data.FurbyNameLeft;
				m_rightSelectedName = Singleton<GameDataStoreObject>.Instance.Data.FurbyNameRight;
			}
			PopulateGrid(m_leftscrollViewGrid, false, "Item" + m_leftSelectedName);
			PopulateGrid(m_rightscrollViewGrid, true, "Item" + m_rightSelectedName);
		}

		public void SaveName()
		{
			if (WhichName == WhichName.BabyName)
			{
				FurbyGlobals.Player.InProgressFurbyBaby.NameLeft = m_leftSelectedName;
				FurbyGlobals.Player.InProgressFurbyBaby.NameRight = m_rightSelectedName;
				FurbyGlobals.Player.InProgressFurbyBaby.HasBeenNamed = true;
				GameEventRouter.SendEvent(BabyLifecycleEvent.BabyNamed, null, FurbyGlobals.Player.InProgressFurbyBaby);
			}
			else
			{
				Singleton<GameDataStoreObject>.Instance.Data.FurbyNameLeft = m_leftSelectedName;
				Singleton<GameDataStoreObject>.Instance.Data.FurbyNameRight = m_rightSelectedName;
			}
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		private void PopulateGrid(UIGrid scrollViewGrid, bool rightName, string selectName)
		{
			string[] array = ((!rightName) ? m_namingData.m_leftNames : m_namingData.m_rightNames);
			Transform transform = null;
			string[] array2 = array;
			foreach (string text in array2)
			{
				GameObject gameObject = Object.Instantiate(m_gridItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>();
				UILabel[] array3 = componentsInChildren;
				foreach (UILabel uILabel in array3)
				{
					uILabel.text = text;
				}
				gameObject.name = string.Format("Item{0}", text);
				gameObject.transform.parent = scrollViewGrid.gameObject.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				gameObject.layer = scrollViewGrid.gameObject.layer;
				gameObject.SetActive(true);
				if (gameObject.name == selectName)
				{
					transform = gameObject.transform;
				}
			}
			scrollViewGrid.Reposition();
			if (transform != null)
			{
				UIPanel component = scrollViewGrid.transform.parent.GetComponent<UIPanel>();
				Vector4 clipRange = component.clipRange;
				clipRange.x = transform.localPosition.x;
				clipRange.y = transform.localPosition.y;
				component.clipRange = clipRange;
				Vector3 localPosition = component.transform.localPosition;
				localPosition.x = 0f - transform.localPosition.x;
				localPosition.y = 0f - transform.localPosition.y;
				component.transform.localPosition = localPosition;
			}
		}

		private void InitialiseGridOnNamedItem(UICenterOnChild uiCenterOnChild, UIGrid grid, string name)
		{
			GameObject childGameObject = uiCenterOnChild.gameObject.GetChildGameObject("Item" + name);
			UIPanel component = grid.transform.parent.GetComponent<UIPanel>();
			Vector4 clipRange = component.clipRange;
			clipRange.x = childGameObject.transform.localPosition.x;
			clipRange.y = childGameObject.transform.localPosition.y;
			component.clipRange = clipRange;
		}

		private GameObject CentredChild(UIGrid grid)
		{
			UIPanel component = grid.transform.parent.GetComponent<UIPanel>();
			Vector3 panelCentre = -component.transform.localPosition;
			panelCentre.z = 0f;
			Transform transform = (from Transform t in grid.transform
				orderby (t.localPosition - panelCentre).magnitude
				select t).First();
			return transform.gameObject;
		}

		private void Recenter(UICenterOnChild centering, float direction, GameObject selected, UIGrid scrollGrid)
		{
			bool flag = false;
			Transform transform = null;
			foreach (Transform item in scrollGrid.transform)
			{
				if (flag)
				{
					if (direction < 0f)
					{
						transform = item;
					}
					break;
				}
				if (item == selected.transform)
				{
					flag = true;
				}
				else if (direction >= 0f)
				{
					transform = item;
				}
			}
			if (transform != null)
			{
				centering.Recenter(transform);
			}
		}

		private void CacheCentredChildren()
		{
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			if (timeSinceLevelLoad != m_LastCacheTime)
			{
				m_LastCacheTime = timeSinceLevelLoad;
				m_leftSelected = CentredChild(m_leftscrollViewGrid);
				m_leftSelectedName = m_leftSelected.GetComponentInChildren<UILabel>().text;
				m_rightSelected = CentredChild(m_rightscrollViewGrid);
				m_rightSelectedName = m_rightSelected.GetComponentInChildren<UILabel>().text;
			}
		}

		private void Update()
		{
			CacheCentredChildren();
			bool flag = m_leftPanel.IsPressed();
			bool flag2 = m_rightPanel.IsPressed();
			if (flag)
			{
				m_lastMovedHandWasLeft = true;
				if (m_leftPanel.currentMomentum.magnitude > 0f)
				{
					m_lastMovementDirection = m_leftPanel.currentMomentum.y;
				}
			}
			else if (flag2)
			{
				m_lastMovedHandWasLeft = false;
				if (m_rightPanel.currentMomentum.magnitude > 0f)
				{
					m_lastMovementDirection = m_rightPanel.currentMomentum.y;
				}
			}
			if (!flag && !flag2)
			{
				FurbyNamingData.DisallowedName[] disallowedNames = m_namingData.m_disallowedNames;
				foreach (FurbyNamingData.DisallowedName disallowedName in disallowedNames)
				{
					if (disallowedName.m_disallowedLeft == m_leftSelectedName && disallowedName.m_disallowedRight == m_rightSelectedName)
					{
						if (!m_lastMovedHandWasLeft)
						{
							UICenterOnChild component = m_rightscrollViewGrid.GetComponent<UICenterOnChild>();
							Recenter(component, m_lastMovementDirection, m_rightSelected, m_rightscrollViewGrid);
						}
						else
						{
							UICenterOnChild component2 = m_leftscrollViewGrid.GetComponent<UICenterOnChild>();
							Recenter(component2, m_lastMovementDirection, m_leftSelected, m_leftscrollViewGrid);
						}
					}
				}
			}
			string key = string.Format("NAME_DEF_{0}_{1}", m_leftSelectedName.ToUpper(), m_rightSelectedName.ToUpper());
			if (IsNameAllowed() && m_meaningLabel != null)
			{
				m_meaningLabel.text = Singleton<Localisation>.Instance.GetText(key);
			}
		}

		public bool IsNameAllowed()
		{
			CacheCentredChildren();
			FurbyNamingData.DisallowedName[] disallowedNames = m_namingData.m_disallowedNames;
			foreach (FurbyNamingData.DisallowedName disallowedName in disallowedNames)
			{
				if (disallowedName.m_disallowedLeft == m_leftSelectedName && disallowedName.m_disallowedRight == m_rightSelectedName)
				{
					return false;
				}
			}
			return true;
		}

		public string GetCurrentName()
		{
			CacheCentredChildren();
			return m_leftSelectedName + "_" + m_rightSelectedName;
		}
	}
}
