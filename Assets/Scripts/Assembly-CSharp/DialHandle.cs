using UnityEngine;

public class DialHandle : MonoBehaviour
{
	public delegate void ChangeHandler(float f);

	public delegate void TouchHandler();

	private float m_correctDistance;

	private Plane m_dragPlane;

	[SerializeField]
	private float m_range = 180f;

	public event ChangeHandler ValueChanged;

	public event TouchHandler Touched;

	public void Start()
	{
		m_correctDistance = (base.transform.position - base.transform.parent.position).magnitude;
	}

	public void OnPress(bool down)
	{
		if (down)
		{
			m_dragPlane = new Plane(base.transform.parent.up, base.transform.parent.position);
			if (this.Touched != null)
			{
				this.Touched();
			}
		}
	}

	public void OnDrag(Vector2 delta)
	{
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		float enter = 0f;
		if (m_dragPlane.Raycast(ray, out enter))
		{
			Vector3 point = ray.GetPoint(enter);
			base.transform.position = point;
		}
		base.transform.LookAt(base.transform.parent.position);
		base.transform.Rotate(base.transform.up, 180f);
		float magnitude = (base.transform.position - base.transform.parent.position).magnitude;
		float num = (magnitude - m_correctDistance) * -1f;
		Vector3 normalized = base.transform.forward.normalized;
		base.transform.position += normalized * num;
		Vector3 lhs = Vector3.Cross(base.transform.parent.forward.normalized, base.transform.forward.normalized);
		float f = Vector3.Dot(lhs, base.transform.parent.up.normalized);
		float num2 = Mathf.Asin(f);
		num2 = 57.29578f * num2;
		if (Vector3.Dot(base.transform.forward, base.transform.parent.forward) < 0f)
		{
			num2 = 180f - num2;
		}
		float num3 = Mathf.Clamp(num2, 0f, m_range);
		float f2 = num3 / m_range;
		RotateForValue(f2);
		if (this.ValueChanged != null)
		{
			this.ValueChanged(f2);
		}
	}

	public void SetValue(float f)
	{
		RotateForValue(f);
	}

	private void RotateForValue(float f)
	{
		Transform transform = base.transform;
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;
		float angle = f * m_range;
		transform.Rotate(transform.up, angle, Space.World);
		transform.position += transform.forward.normalized * m_correctDistance;
	}
}
