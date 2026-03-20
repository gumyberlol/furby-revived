using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ScreenBounds
	{
		[SerializeField]
		public Vector3 m_BottomLeft;

		[SerializeField]
		public Vector3 m_TopRight;

		[SerializeField]
		public float m_CameraMaxY;

		[SerializeField]
		public float m_CameraMinY;

		[SerializeField]
		public float m_CameraMaxX;

		[SerializeField]
		public float m_CameraMinX;

		public void InitialiseScreenBounds(Transform trans, Camera cam)
		{
			m_TopRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 0f - trans.position.z));
			m_BottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f - trans.position.z));
			m_CameraMaxX = m_TopRight.x;
			m_CameraMaxY = m_TopRight.y;
			m_CameraMinX = m_BottomLeft.x;
			m_CameraMinY = m_BottomLeft.y;
		}

		public void ClampToBounds(ref Camera cam, ref Vector3 in_position)
		{
			m_TopRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 0f - in_position.z));
			m_BottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f - in_position.z));
			if (m_TopRight.x > m_CameraMaxX)
			{
				in_position = new Vector3(in_position.x - (m_TopRight.x - m_CameraMaxX), in_position.y, in_position.z);
			}
			if (m_TopRight.y > m_CameraMaxY)
			{
				in_position = new Vector3(in_position.x, in_position.y - (m_TopRight.y - m_CameraMaxY), in_position.z);
			}
			if (m_BottomLeft.x < m_CameraMinX)
			{
				in_position = new Vector3(in_position.x + (m_CameraMinX - m_BottomLeft.x), in_position.y, in_position.z);
			}
			if (m_BottomLeft.y < m_CameraMinY)
			{
				in_position = new Vector3(in_position.x, in_position.y + (m_CameraMinY - m_BottomLeft.y), in_position.z);
			}
		}
	}
}
