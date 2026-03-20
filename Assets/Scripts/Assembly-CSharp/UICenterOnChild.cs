using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center On Child")]
public class UICenterOnChild : MonoBehaviour
{
	public SpringPanel.OnFinished onFinished;

	protected UIDraggablePanel mDrag;

	protected GameObject mCenteredObject;

	public GameObject centeredObject
	{
		get
		{
			return mCenteredObject;
		}
	}

	private void OnEnable()
	{
		Recenter(null);
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Recenter(null);
		}
	}

	public virtual void Recenter(Transform closest)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 vector = localPosition - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		if (closest == null)
		{
			float num = float.MaxValue;
			Transform transform = base.transform;
			int i = 0;
			for (int childCount = transform.childCount; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				float num2 = Vector3.SqrMagnitude(child.position - vector);
				if (num2 < num)
				{
					num = num2;
					closest = child;
				}
			}
		}
		if (closest != null)
		{
			mCenteredObject = closest.gameObject;
			Vector3 vector2 = cachedTransform.InverseTransformPoint(closest.position);
			Vector3 vector3 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector4 = vector2 - vector3;
			if (mDrag.scale.x == 0f)
			{
				vector4.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector4.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector4.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector4, 8f).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}
}
