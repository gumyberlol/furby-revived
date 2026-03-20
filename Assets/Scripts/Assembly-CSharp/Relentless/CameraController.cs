using UnityEngine;

namespace Relentless
{
	public abstract class CameraController : RelentlessMonoBehaviour
	{
		public Camera m_TheCamera;

		protected CameraType m_CameraType;

		private bool m_IsActive;

		protected bool IsActive
		{
			get
			{
				return m_IsActive;
			}
			set
			{
				m_IsActive = value;
			}
		}

		private void Start()
		{
			Logging.Log("CameraController - START");
			m_IsActive = ValidateConfiguration();
			if (m_IsActive)
			{
				Initialise();
				if ((bool)m_TheCamera)
				{
					m_CameraType = (m_TheCamera.isOrthoGraphic ? CameraType.Orthographic : CameraType.Perspective);
				}
			}
		}

		private void Update()
		{
			if (m_IsActive)
			{
				UpdateCameraControls();
			}
		}

		protected abstract bool ValidateConfiguration();

		protected abstract void UpdateCameraControls();

		protected abstract void Initialise();
	}
}
