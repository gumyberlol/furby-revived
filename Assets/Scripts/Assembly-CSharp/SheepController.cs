using Relentless;
using UnityEngine;

public class SheepController : MonoBehaviour
{
	private static Collider s_CurrentCollider;

	[SerializeField]
	private LayerMask m_LayerMask;

	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private float m_MovementLimitX = 0.6f;

	[SerializeField]
	private float m_MovementLimitY = 0.9f;

	[SerializeField]
	private float m_DragCoefficient = 0.1f;

	[SerializeField]
	private float m_BounceCoefficient = 0.6f;

	[SerializeField]
	private float m_FencePositionY = 0.6f;

	[SerializeField]
	private float m_MinimumFenceJumpSpeed = 0.4f;

	private Camera m_Camera;

	private Transform m_MovementTransform;

	private Vector3 m_LastMouseWorldPosition = Vector3.zero;

	private Vector3 m_Velocity = Vector3.zero;

	private bool m_HasJumpedFence;

	private void Start()
	{
		Camera[] allCameras = Camera.allCameras;
		foreach (Camera camera in allCameras)
		{
			if (camera.gameObject.layer == base.gameObject.layer)
			{
				m_Camera = camera;
				break;
			}
		}
		m_MovementTransform = base.gameObject.transform.parent.transform;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && IsNoCurrentCollider())
		{
			if (IsUnderTouchPosition())
			{
				OnTouchStart();
			}
		}
		else if (IsCurrentCollider())
		{
			if (Input.GetMouseButton(0))
			{
				OnTouchDrag();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnTouchEnd();
			}
		}
		Vector3 position = m_MovementTransform.position + m_Velocity;
		if (!m_HasJumpedFence)
		{
			m_Velocity -= m_Velocity * m_DragCoefficient;
		}
		if (position.x <= 0f - m_MovementLimitX)
		{
			position.x = 0f - m_MovementLimitX;
			m_Velocity.x = 0f - m_Velocity.x;
			ApplyBounceCoefficient();
		}
		else if (position.x >= m_MovementLimitX)
		{
			position.x = m_MovementLimitX;
			m_Velocity.x = 0f - m_Velocity.x;
			m_Velocity *= m_BounceCoefficient;
		}
		if (position.y <= 0f - m_MovementLimitY)
		{
			position.y = 0f - m_MovementLimitY;
			m_Velocity.y = 0f - m_Velocity.y;
			m_Velocity *= m_BounceCoefficient;
		}
		else if (position.y >= m_FencePositionY && !m_HasJumpedFence)
		{
			float magnitude = m_Velocity.magnitude;
			if (magnitude < m_MinimumFenceJumpSpeed)
			{
				Logging.Log(string.Format("{0} - Speed: {1}", base.gameObject.transform.parent.name, magnitude));
				position.y = m_FencePositionY;
				m_Velocity.y = 0f - m_Velocity.y;
				m_Velocity *= m_BounceCoefficient;
				m_HasJumpedFence = true;
			}
		}
		m_MovementTransform.position = position;
	}

	private void ApplyBounceCoefficient()
	{
		if (!m_HasJumpedFence)
		{
			m_Velocity *= m_BounceCoefficient;
		}
	}

	private bool IsNoCurrentCollider()
	{
		return s_CurrentCollider == null;
	}

	private bool IsCurrentCollider()
	{
		return s_CurrentCollider == m_Collider;
	}

	private bool IsUnderTouchPosition()
	{
		Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, m_LayerMask) && hitInfo.collider == m_Collider)
		{
			return true;
		}
		return false;
	}

	private Vector3 GetMouseWorldPosition()
	{
		Vector3 result = m_Camera.ScreenToWorldPoint(Input.mousePosition);
		result.z = 0f;
		return result;
	}

	private void OnTouchStart()
	{
		m_LastMouseWorldPosition = GetMouseWorldPosition();
		s_CurrentCollider = m_Collider;
		m_Velocity = Vector3.zero;
	}

	private void OnTouchDrag()
	{
		Vector3 mouseWorldPosition = GetMouseWorldPosition();
		Vector3 vector = mouseWorldPosition - m_LastMouseWorldPosition;
		m_LastMouseWorldPosition = mouseWorldPosition;
		Vector3 position = m_MovementTransform.position + vector;
		m_MovementTransform.position = position;
	}

	private void OnTouchEnd()
	{
		s_CurrentCollider = null;
		Vector3 mouseWorldPosition = GetMouseWorldPosition();
		Vector3 velocity = mouseWorldPosition - m_LastMouseWorldPosition;
		m_Velocity = velocity;
	}
}
