using Fabric;
using UnityEngine;

public class Listener : MonoBehaviour
{
	public float speed = 6f;

	public float jumpSpeed = 8f;

	public float moveSpeed = 8f;

	public float gravity = 20f;

	private Vector3 moveDirection = Vector3.zero;

	private void Update()
	{
		CharacterController component = GetComponent<CharacterController>();
		if (component.isGrounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			moveDirection = base.transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetButton("Jump"))
			{
				moveDirection.y = jumpSpeed;
				EventManager.Instance.PostEvent("Jump", base.gameObject);
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		component.Move(moveDirection * Time.deltaTime);
	}
}
