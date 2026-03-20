using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FingerGesturesSwipe : RelentlessMonoBehaviour
	{
		public string m_leftAction;

		public string m_rightAction;

		public float m_screenWidth = 1024f;

		public int m_max;

		public int m_min = -4;

		public bool m_noBounds;

		public float m_maxSpeed = 4096f;

		public float m_maxAcceleration = 2048f;

		public float m_dampingPerSec = 0.5f;

		public bool m_rememberLocation;

		private static Dictionary<string, float> s_savedScrollOffsets = new Dictionary<string, float>();

		private float m_currentSpeed;

		public float m_fractionToScroll = 0.25f;

		public AnimationCurve m_movementCurve;

		public AnimationCurve m_stoppingCurve;

		public AnimationCurve m_distanceToMaxSpeedCurve;

		private Vector2 m_startDragPos;

		private Vector3 m_beginPos;

		private bool m_isDragging;

		private int m_currentScreen;

		private GUIScreen m_parentGUIScreen;

		private void Start()
		{
			FingerGestures.OnSwipe += FingerGestures_OnSwipe;
			FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
			FingerGestures.OnDragMove += FingerGestures_OnDragMove;
			FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
			m_parentGUIScreen = HierarchyHelper.FindParent<GUIScreen>(base.transform);
			if (m_rememberLocation)
			{
				float value = 0f;
				if (s_savedScrollOffsets.TryGetValue(base.name, out value))
				{
					base.transform.localPosition = new Vector3(value, 0f, 0f);
					m_currentScreen = (int)(value / m_screenWidth);
				}
			}
		}

		public void InstantScrollTo(int screen)
		{
			base.transform.localPosition = new Vector3((float)(-screen) * m_screenWidth, 0f, 0f);
			m_currentScreen = -screen;
		}

		private void OnDestroy()
		{
			FingerGestures.OnSwipe -= FingerGestures_OnSwipe;
			FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
			FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
			FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
			if (m_rememberLocation)
			{
				s_savedScrollOffsets[base.name] = base.transform.localPosition.x;
			}
		}

		private void FingerGestures_OnDragBegin(Vector2 fingerPos, Vector2 startPos)
		{
			m_startDragPos = startPos;
			m_beginPos = base.transform.localPosition;
			m_isDragging = true;
		}

		private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
		{
			float deltaTime = SingletonInstance<SystemFrameTimer>.Instance.GetDeltaTime();
			float num = fingerPos.x - m_startDragPos.x;
			num /= (float)Screen.width;
			num *= m_screenWidth;
			float f = num / m_screenWidth;
			m_currentSpeed = delta.x / deltaTime;
			if (Mathf.Abs(num) > 0f)
			{
				int num2 = m_currentScreen + (int)Mathf.Sign(num);
				if (!m_noBounds && (num2 > m_max || num2 < m_min))
				{
					num = m_screenWidth * m_stoppingCurve.Evaluate(Mathf.Abs(f)) * Mathf.Sign(f);
					m_currentSpeed = 0f;
				}
				else
				{
					num = m_screenWidth * m_movementCurve.Evaluate(Mathf.Abs(f)) * Mathf.Sign(f);
				}
			}
			base.transform.localPosition = m_beginPos + new Vector3(num, 0f, 0f);
		}

		private void FingerGestures_OnDragEnd(Vector2 fingerPos)
		{
			float num = fingerPos.x - m_startDragPos.x;
			num /= (float)Screen.width;
			num *= m_screenWidth;
			float f = num / m_screenWidth;
			if (Mathf.Abs(f) > m_fractionToScroll)
			{
				int currentScreen = m_currentScreen;
				m_currentScreen += (int)Mathf.Sign(num);
				if (!m_noBounds)
				{
					m_currentScreen = Mathf.Clamp(m_currentScreen, m_min, m_max);
				}
				BroadcastMessage(FurbyGUIMessages.s_onDragMotionEnd, (int)Mathf.Sign(m_currentScreen - currentScreen), SendMessageOptions.DontRequireReceiver);
				if (m_parentGUIScreen != null)
				{
					m_parentGUIScreen.gameObject.BroadcastMessage(FurbyGUIMessages.s_onCreatureChange, SendMessageOptions.DontRequireReceiver);
				}
			}
			m_isDragging = false;
		}

		private void FingerGestures_OnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
		{
		}

		private void Update()
		{
			float deltaTime = SingletonInstance<SystemFrameTimer>.Instance.GetDeltaTime();
			if (!m_isDragging)
			{
				float num = (float)m_currentScreen * m_screenWidth;
				float num2 = num - base.transform.localPosition.x;
				if (num2 != 0f)
				{
					float num3 = m_distanceToMaxSpeedCurve.Evaluate(Mathf.Abs(num2) / m_screenWidth) * m_maxSpeed * Mathf.Sign(num2);
					float f = num3 - m_currentSpeed;
					m_currentSpeed += Mathf.Sign(f) * deltaTime * m_maxAcceleration;
					m_currentSpeed = Mathf.Min(Mathf.Abs(m_currentSpeed), m_maxSpeed) * Mathf.Sign(m_currentSpeed);
					m_currentSpeed -= m_currentSpeed * m_dampingPerSec * deltaTime;
					float x = Mathf.Min(m_currentSpeed * deltaTime, Mathf.Abs(num2));
					base.transform.localPosition += new Vector3(x, 0f, 0f);
				}
			}
		}

		private void OnScrollLeft()
		{
			float num = (float)m_currentScreen * m_screenWidth;
			float num2 = num - base.transform.localPosition.x;
			if (num2 == 0f)
			{
				int currentScreen = m_currentScreen;
				m_currentScreen++;
				if (!m_noBounds && m_currentScreen > m_max)
				{
					m_currentScreen = m_max;
				}
				else
				{
					m_currentSpeed = m_maxSpeed;
				}
				BroadcastMessage(FurbyGUIMessages.s_onDragMotionEnd, (int)Mathf.Sign(m_currentScreen - currentScreen), SendMessageOptions.DontRequireReceiver);
				m_parentGUIScreen.gameObject.BroadcastMessage(FurbyGUIMessages.s_onCreatureChange, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnScrollRight()
		{
			float num = (float)m_currentScreen * m_screenWidth;
			float num2 = num - base.transform.localPosition.x;
			if (num2 == 0f)
			{
				int currentScreen = m_currentScreen;
				m_currentScreen--;
				if (!m_noBounds && m_currentScreen < m_min)
				{
					m_currentScreen = m_min;
				}
				else
				{
					m_currentSpeed = 0f - m_maxSpeed;
				}
				BroadcastMessage("OnDragMotionEnd", (int)Mathf.Sign(m_currentScreen - currentScreen), SendMessageOptions.DontRequireReceiver);
				m_parentGUIScreen.gameObject.BroadcastMessage(FurbyGUIMessages.s_onCreatureChange, SendMessageOptions.DontRequireReceiver);
			}
		}

		public bool CanScrollRight()
		{
			if (m_noBounds)
			{
				return true;
			}
			if (m_currentScreen > m_min)
			{
				return true;
			}
			return false;
		}

		public bool CanScrollLeft()
		{
			if (m_noBounds)
			{
				return true;
			}
			if (m_currentScreen < m_max)
			{
				return true;
			}
			return false;
		}
	}
}
