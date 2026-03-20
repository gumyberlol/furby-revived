using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyModeBouncerController : RelentlessMonoBehaviour
	{
		[SerializeField]
		private Camera m_activeCamera;

		[SerializeField]
		private BoxCollider m_bouncerBounds;

		[SerializeField]
		private Transform m_goalCentre;

		private FurbyStrikerController m_strikerController;

		private void Start()
		{
			m_strikerController = (FurbyStrikerController)Object.FindObjectOfType(typeof(FurbyStrikerController));
		}

		private void Update()
		{
			bool flag = false;
			if (Input.GetMouseButton(0) && Input.mousePosition.y < (float)Screen.height * 0.8f && m_strikerController.AreAnyBallsReturning())
			{
				Ray ray = m_activeCamera.ScreenPointToRay(Input.mousePosition);
				float num = 1f / Vector3.Dot(ray.direction, Vector3.up) * (0f - ray.origin.y);
				Vector3 position = ray.origin + ray.direction * num;
				position.x = Mathf.Clamp(position.x, m_bouncerBounds.bounds.min.x, m_bouncerBounds.bounds.max.x);
				position.y = 0f;
				position.z = Mathf.Clamp(position.z, m_bouncerBounds.bounds.min.z, m_bouncerBounds.bounds.max.z);
				base.transform.position = position;
				flag = true;
			}
			base.collider.enabled = flag;
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (string.Compare(collisionInfo.collider.tag, "Furball_Football") == 0)
			{
				GameEventRouter.SendEvent(FurBallGameEvent.FurballBouncerHitsBall);
				Vector3 force = 0.2f * (m_goalCentre.position - base.transform.position).normalized;
				collisionInfo.rigidbody.velocity = Vector3.zero;
				collisionInfo.rigidbody.AddForce(force, ForceMode.Impulse);
				collisionInfo.gameObject.GetComponent<FurballController>().OnKicked();
			}
		}
	}
}
