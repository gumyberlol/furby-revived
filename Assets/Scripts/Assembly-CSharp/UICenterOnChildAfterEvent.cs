using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center On Child (after event)")]
[ExecuteInEditMode]
public class UICenterOnChildAfterEvent : UICenterOnChild
{
	public bool TriggeredByDragFinishedEvent = true;

	public bool AutoCenterAtStart = true;

	public float StrengthOfSpring = 8f;

	public string InitialSelectedObjectName;

	private UIWrappedGrid m_grid;

	private Stack<SpringPanel.OnFinished> m_callbacks = new Stack<SpringPanel.OnFinished>();

	private void Start()
	{
		m_grid = base.gameObject.GetComponent<UIWrappedGrid>();
	}

	private void OnEnable()
	{
		m_grid = base.gameObject.GetComponent<UIWrappedGrid>();
		if (AutoCenterAtStart)
		{
			Recenter();
		}
		else if (!string.IsNullOrEmpty(InitialSelectedObjectName))
		{
			RecenterOnInitialItem();
		}
	}

	public void RecenterOnInitialItem()
	{
		GameObject childGameObject = base.gameObject.GetChildGameObject(InitialSelectedObjectName);
		if (childGameObject != null)
		{
			Recenter(childGameObject.transform);
		}
	}

	private void OnDragFinished()
	{
		if (TriggeredByDragFinishedEvent && base.enabled)
		{
			Invoke("Recenter", 0.5f);
		}
	}

	private void Recenter()
	{
		Recenter(null);
	}

	public override void Recenter(Transform closest)
	{
		Recenter(closest, false, null);
	}

	public Transform FindClosest()
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Logging.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return null;
			}
			UIDraggablePanel uIDraggablePanel = mDrag;
			uIDraggablePanel.onDragFinished = (UIDraggablePanel.OnDragFinished)Delegate.Combine(uIDraggablePanel.onDragFinished, new UIDraggablePanel.OnDragFinished(OnDragFinished));
		}
		Vector3 center = GetCenter(mDrag.panel);
		Vector3 offsetCenter = center - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		return FindClosest(center, offsetCenter);
	}

	public Transform FindClosest(Vector3 center, Vector3 offsetCenter)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Logging.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return null;
			}
			UIDraggablePanel uIDraggablePanel = mDrag;
			uIDraggablePanel.onDragFinished = (UIDraggablePanel.OnDragFinished)Delegate.Combine(uIDraggablePanel.onDragFinished, new UIDraggablePanel.OnDragFinished(OnDragFinished));
		}
		if (mDrag.panel == null)
		{
			return null;
		}
		Transform result = null;
		float num = float.MaxValue;
		Transform transform = base.transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - offsetCenter);
			if (num2 < num)
			{
				num = num2;
				result = child;
			}
		}
		return result;
	}

	public static Vector3 GetCenter(UIPanel panel)
	{
		Vector4 clipRange = panel.clipRange;
		Transform cachedTransform = panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		return cachedTransform.parent.TransformPoint(localPosition);
	}

	public void Recenter(Transform closest, bool snap, SpringPanel.OnFinished callback)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Logging.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			UIDraggablePanel uIDraggablePanel = mDrag;
			uIDraggablePanel.onDragFinished = (UIDraggablePanel.OnDragFinished)Delegate.Combine(uIDraggablePanel.onDragFinished, new UIDraggablePanel.OnDragFinished(OnDragFinished));
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector3 center = GetCenter(mDrag.panel);
		Vector3 offsetCenter = center - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		if (closest == null)
		{
			closest = FindClosest(center, offsetCenter);
		}
		else
		{
			m_grid.SnapEnabled = false;
		}
		if (closest != null)
		{
			mCenteredObject = closest.gameObject;
			Transform cachedTransform = mDrag.panel.cachedTransform;
			Vector3 vector = cachedTransform.InverseTransformPoint(closest.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(center);
			Vector3 vector3 = vector - vector2;
			if (mDrag.scale.x == 0f)
			{
				vector3.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector3.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector3.z = 0f;
			}
			Vector3 localPosition = cachedTransform.localPosition;
			m_callbacks.Push(callback);
			if (!snap)
			{
				SpringPanel.Begin(mDrag.gameObject, localPosition - vector3, StrengthOfSpring).onFinished = OnFinished;
			}
			else
			{
				SpringPanel.Begin(mDrag.gameObject, localPosition - vector3, 0f).onFinished = OnFinished;
			}
		}
		else
		{
			Logging.LogWarning("Failed to find a center child", this);
			mCenteredObject = null;
		}
	}

	private void OnFinished()
	{
		SpringPanel.OnFinished onFinished = base.onFinished;
		if (onFinished != null)
		{
			onFinished();
		}
		while (m_callbacks.Count > 0)
		{
			onFinished = m_callbacks.Pop();
			if (onFinished != null)
			{
				onFinished();
			}
		}
		m_grid.SnapEnabled = true;
	}
}
