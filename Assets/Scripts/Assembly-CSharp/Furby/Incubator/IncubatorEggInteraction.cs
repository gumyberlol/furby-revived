using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorEggInteraction : MonoBehaviour
	{
		[SerializeField]
		private Camera m_EggCamera;

		[SerializeField]
		private Collider m_EggCollider;

		public bool Active { get; set; }

		public float Contribution { get; set; }

		public float Duration { get; set; }

		public void OnEnable()
		{
			FingerGestures.OnFingerDown += OnFingerDown;
			FingerGestures.OnFingerUp += OnFingerUp;
			Active = false;
			Contribution = 0f;
			Duration = 0f;
		}

		private void OnDisable()
		{
			FingerGestures.OnFingerDown -= OnFingerDown;
			FingerGestures.OnFingerUp -= OnFingerUp;
		}

		private void OnFingerDown(int finger, Vector2 origin)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(m_EggCamera.ScreenPointToRay(origin), out hitInfo))
			{
				Active = hitInfo.collider == m_EggCollider;
			}
		}

		private void OnFingerUp(int finger, Vector2 origin, float time)
		{
			if (Active)
			{
				Active = false;
				if (time < 0.125f)
				{
					Contribution = 1f;
					Duration = 0f;
				}
				else
				{
					Contribution = time;
					Duration = time;
				}
			}
		}
	}
}
