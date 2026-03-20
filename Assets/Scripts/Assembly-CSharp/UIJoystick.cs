using System.Collections;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/UI Joystick")]
public class UIJoystick : MonoBehaviour
{
	public Transform target;

	public Vector3 scale = Vector3.one;

	public float radius = 100f;

	public bool centerOnPress = true;

	private Vector3 userInitTouchPos;

	public bool normalize;

	public Vector2 position;

	public float deadZone = 2f;

	public float fadeOutAlpha = 0.2f;

	public float fadeOutDelay = 1f;

	public UIWidget[] widgetsToFade;

	public Transform[] widgetsToCenter;

	public GameObject[] m_autoAimObjects;

	public GameObject[] m_manualObjects;

	private bool m_autoAim;

	private bool m_initialised;

	public void SetAutoAim(bool autoAim)
	{
		if (m_autoAim != autoAim || !m_initialised)
		{
			m_autoAim = autoAim;
			GameObject[] autoAimObjects = m_autoAimObjects;
			foreach (GameObject gameObject in autoAimObjects)
			{
				gameObject.SetActive(m_autoAim);
			}
			GameObject[] manualObjects = m_manualObjects;
			foreach (GameObject gameObject2 in manualObjects)
			{
				gameObject2.SetActive(!m_autoAim);
			}
			m_initialised = true;
		}
	}

	private void Awake()
	{
		userInitTouchPos = Vector3.zero;
	}

	private void Start()
	{
		if (centerOnPress)
		{
			StartCoroutine(fadeOutJoystick());
		}
	}

	private void OnDisable()
	{
		target.localPosition = Vector3.zero;
		position = Vector3.zero;
	}

	private IEnumerator fadeOutJoystick()
	{
		yield return new WaitForSeconds(fadeOutDelay);
		UIWidget[] array = widgetsToFade;
		foreach (UIWidget widget in array)
		{
			Color lastColor = widget.color;
			Color newColor = lastColor;
			newColor.a = fadeOutAlpha;
			TweenColor.Begin(widget.gameObject, 0.5f, newColor).method = UITweener.Method.EaseOut;
		}
	}

	public void OnPress(bool pressed)
	{
		if (!(target != null))
		{
			return;
		}
		if (pressed)
		{
			if (m_autoAim)
			{
				userInitTouchPos = target.position;
				target.localPosition = Vector3.zero;
				position = Vector3.one;
				return;
			}
			StopAllCoroutines();
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
			float distance = 0f;
			Vector3 point = ray.GetPoint(distance);
			point.z = 0f;
			if (centerOnPress)
			{
				userInitTouchPos = point;
				UIWidget[] array = widgetsToFade;
				foreach (UIWidget uIWidget in array)
				{
					TweenColor.Begin(uIWidget.gameObject, 0.1f, Color.white).method = UITweener.Method.EaseIn;
				}
				Transform[] array2 = widgetsToCenter;
				foreach (Transform transform in array2)
				{
					transform.position = userInitTouchPos;
				}
			}
			else
			{
				userInitTouchPos = target.position;
				OnDrag(Vector2.zero);
			}
		}
		else
		{
			ResetJoystick();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (m_autoAim)
		{
			userInitTouchPos = target.position;
			target.localPosition = Vector3.zero;
			position = Vector3.one;
			return;
		}
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float distance = 0f;
		Vector3 point = ray.GetPoint(distance);
		Vector3 vector = point - userInitTouchPos;
		if (vector.x != 0f || vector.y != 0f)
		{
			vector = target.InverseTransformDirection(vector);
			vector.Scale(scale);
			vector = target.TransformDirection(vector);
			vector.z = 0f;
		}
		target.position = userInitTouchPos + vector;
		Vector3 vector2 = target.position;
		vector2.z = 0f;
		target.position = vector2;
		float sqrMagnitude = target.localPosition.sqrMagnitude;
		if (sqrMagnitude < deadZone * deadZone)
		{
			position = Vector2.zero;
			target.localPosition = position;
		}
		else
		{
			if (sqrMagnitude > radius * radius)
			{
				target.localPosition = Vector3.ClampMagnitude(target.localPosition, radius);
			}
			position = target.localPosition;
		}
		if (normalize)
		{
			position = position / radius * Mathf.InverseLerp(radius, deadZone, 1f);
		}
	}

	private void ResetJoystick()
	{
		position = Vector2.zero;
		target.position = userInitTouchPos;
		if (centerOnPress)
		{
			StartCoroutine(fadeOutJoystick());
		}
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
	}

	public Vector3 GetDirectionPressed()
	{
		return new Vector3(position.x, 0f, position.y);
	}
}
