using System.Collections.Generic;
using System.ComponentModel;
using Relentless;
using UnityEngine;

[RequireComponent(typeof(UICenterOnChildAfterEvent))]
[AddComponentMenu("NGUI/Interaction/WrappedGrid")]
public class UIWrappedGrid : UIGrid
{
	private const string LeftItemTag = "LeftPanelItem";

	private const string RightItemTag = "RightPanelItem";

	private UIDraggablePanel m_dragPanel;

	private UICenterOnChildAfterEvent m_centerChild;

	private int m_minimumNumberOfItems;

	private int m_originalNumberOfItems;

	private bool m_initialised;

	private bool m_recenterAfter;

	private Vector3 m_cachedCurrentMomentum;

	private List<Transform> m_items = new List<Transform>();

	public GameObject SnapCollider;

	private bool m_snapEnabled;

	public bool SnapEnabled
	{
		get
		{
			return m_snapEnabled;
		}
		set
		{
			m_snapEnabled = value;
			if (m_snapEnabled)
			{
				SnapCollider.SetActive(false);
				SnapCollider.SetActive(true);
			}
			else
			{
				SnapCollider.SetActive(false);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		Initialise();
	}

	public override void OnEnable()
	{
		SnapEnabled = false;
		base.OnEnable();
	}

	public void OnDisable()
	{
		SnapEnabled = false;
	}

	private new void Update()
	{
		base.Update();
	}

	public void Snap(Snap command)
	{
		if (SnapEnabled)
		{
			SnapInternal(command, m_originalNumberOfItems, true);
		}
	}

	public void Snap(Snap command, bool recenterAfter)
	{
		if (SnapEnabled)
		{
			SnapInternal(command, m_originalNumberOfItems, recenterAfter);
		}
	}

	private void SnapInternal(Snap command, int numberOfItems, bool recenterAfter)
	{
		int num = 0;
		switch (command)
		{
		case Relentless.Snap.Left:
			num = 1;
			break;
		case Relentless.Snap.Right:
			num = -1;
			break;
		default:
			throw new InvalidEnumArgumentException("could not handle " + command);
		}
		Vector3 vector = ((arrangement != Arrangement.Horizontal) ? new Vector3(0f, (float)numberOfItems * cellHeight * (float)num, 0f) : new Vector3((float)numberOfItems * cellWidth * (float)num, 0f, 0f));
		if (!(m_dragPanel == null) && !(m_dragPanel.panel == null) && !(m_dragPanel.gameObject == null))
		{
			Transform cachedTransform = m_dragPanel.panel.cachedTransform;
			Vector3 pos = cachedTransform.localPosition + vector;
			m_recenterAfter = recenterAfter;
			if (recenterAfter)
			{
				m_cachedCurrentMomentum = Vector3.zero;
				m_dragPanel.currentMomentum = Vector3.zero;
			}
			else
			{
				m_cachedCurrentMomentum = m_dragPanel.currentMomentum;
			}
			if (m_recenterAfter)
			{
				SnapEnabled = false;
			}
			SpringPanel component = m_dragPanel.gameObject.GetComponent<SpringPanel>();
			if (!(component == null))
			{
				component.SnapTo(pos);
				OnFinished();
			}
		}
	}

	public static int SortByX(Transform a, Transform b)
	{
		return a.localPosition.x.CompareTo(b.localPosition.x);
	}

	public static int SortByY(Transform a, Transform b)
	{
		return a.localPosition.y.CompareTo(b.localPosition.y);
	}

	private void Initialise()
	{
		if (m_initialised)
		{
			return;
		}
		m_initialised = true;
		m_dragPanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		if (m_dragPanel == null)
		{
			Logging.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
			base.enabled = false;
			return;
		}
		m_centerChild = NGUITools.FindInParents<UICenterOnChildAfterEvent>(base.gameObject);
		if (m_centerChild == null)
		{
			Logging.LogWarning(string.Concat(GetType(), " requires ", typeof(UICenterOnChildAfterEvent), " on a parent object in order to work"), this);
			base.enabled = false;
		}
		else
		{
			m_originalNumberOfItems = base.transform.childCount;
			m_minimumNumberOfItems = base.transform.childCount * 3;
			UpdateChildren(false);
		}
	}

	private void UpdateChildren(bool recenter)
	{
		if (base.transform.childCount == 0)
		{
			return;
		}
		if (m_items.Count == 0)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				if ((bool)child && (!hideInactive || NGUITools.GetActive(child.gameObject)))
				{
					m_items.Add(child);
				}
			}
			EnsureMinimumNumberOfItems(m_items);
			if (arrangement == Arrangement.Horizontal)
			{
				m_items.Sort(SortByX);
			}
			else
			{
				m_items.Sort(SortByY);
			}
			int count = m_items.Count;
			float num = (float)count / 2f;
			float num2 = (float)m_originalNumberOfItems / 2f;
			int num3 = Mathf.RoundToInt(num + num2) - 1;
			int num4 = Mathf.RoundToInt((float)m_originalNumberOfItems / 4f);
			int num5 = m_originalNumberOfItems - num4;
			for (int j = 0; j <= num4; j++)
			{
				BoxCollider componentInChildren = m_items[j].gameObject.GetComponentInChildren<BoxCollider>();
				if ((bool)componentInChildren)
				{
					componentInChildren.gameObject.tag = "LeftPanelItem";
				}
			}
			for (int k = num3 + num5; k < count; k++)
			{
				BoxCollider componentInChildren2 = m_items[k].gameObject.GetComponentInChildren<BoxCollider>();
				if ((bool)componentInChildren2)
				{
					componentInChildren2.gameObject.tag = "RightPanelItem";
				}
			}
		}
		if (recenter)
		{
			m_centerChild.Recenter(null, false, EnableSnapping);
		}
	}

	private void EnableSnapping()
	{
		SnapEnabled = true;
	}

	private void OnFinished()
	{
		if (m_recenterAfter)
		{
			m_centerChild.Recenter(null, false, EnableSnapping);
		}
		else
		{
			EnableSnapping();
		}
		m_dragPanel.currentMomentum = m_cachedCurrentMomentum;
		m_cachedCurrentMomentum = Vector3.zero;
	}

	private void EnsureMinimumNumberOfItems(List<Transform> list)
	{
		if (list.Count < m_minimumNumberOfItems)
		{
			int num = 0;
			List<Transform> list2 = new List<Transform>();
			while (base.transform.childCount < m_minimumNumberOfItems)
			{
				DuplicateChildren(list2, list, num++);
			}
			list.AddRange(list2);
			bool flag = hideInactive;
			hideInactive = false;
			Reposition();
			hideInactive = flag;
			if (!string.IsNullOrEmpty(m_centerChild.InitialSelectedObjectName))
			{
				m_centerChild.RecenterOnInitialItem();
			}
		}
	}

	private void DuplicateChildren(List<Transform> newList, List<Transform> existingChildren, int batch)
	{
		int num = existingChildren.Count;
		int num2 = 0;
		int i = 0;
		for (int count = existingChildren.Count; i < count; i++)
		{
			Transform transform = existingChildren[i];
			float z = transform.localPosition.z;
			Vector3 vector = ((arrangement != Arrangement.Horizontal) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z));
			GameObject gameObject = Object.Instantiate(transform.gameObject, vector, transform.rotation) as GameObject;
			gameObject.transform.parent = null;
			gameObject.AddComponent<TmpClone>();
			if (batch % 2 == 0)
			{
				gameObject.name = string.Format("Z{0}{1}", batch, transform.gameObject.name);
			}
			else
			{
				gameObject.name = string.Format("{0}{1}", batch, transform.gameObject.name);
			}
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = vector;
			newList.Add(gameObject.transform);
			if (++num >= maxPerLine && maxPerLine > 0)
			{
				num = 0;
				num2++;
			}
		}
	}
}
