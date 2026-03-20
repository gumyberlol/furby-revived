using UnityEngine;

namespace Furby
{
	public class BounceItem : MonoBehaviour
	{
		public delegate void BounceItemDelegate();

		private float DragSpeed = 1024f;

		private float TimeOutDelay = 15f;

		private float HalfHeight = 512f;

		private float HalfWidth = 384f;

		private float BounceBorder = 50f;

		private Vector2 m_spriteSize;

		private Bounds m_bounceBounds;

		private bool m_returningItem;

		private float m_playTimer;

		private UISprite m_sprite;

		private Vector3 m_velocity;

		private Vector3 m_lastPosition;

		[SerializeField]
		private float m_ItemEnlargeScale = 1.5f;

		private float m_ItemFlickedSpeed = 15f;

		private Vector3 m_TouchPosition = Vector3.zero;

		private Vector3 m_OriginalScale = Vector3.one;

		private bool m_IsDraggingItem;

		[SerializeField]
		private Transform m_HoseDroppedItem;

		[SerializeField]
		private float m_HoseDroppedItemPositionScale = 0.8f;

		[SerializeField]
		private float m_HoseDroppedItemPositionOffset = 100f;

		private bool m_shouldTimeout = true;

		public bool ReturningItem
		{
			get
			{
				return m_returningItem;
			}
		}

		public bool ShouldTimeout
		{
			get
			{
				return m_shouldTimeout;
			}
			set
			{
				m_shouldTimeout = value;
			}
		}

		public event BounceItemDelegate OnGiven;

		public event BounceItemDelegate OnBounce;

		public event BounceItemDelegate OnTimeOut;

		public event BounceItemDelegate OnFlicked;

		public event BounceItemDelegate OnGrabbed;

		public void Return()
		{
			m_returningItem = true;
			m_velocity = new Vector2(Random.Range(-50f, 50f), -30f);
			m_lastPosition = new Vector3(0f, 1280f, 0f);
			SetLocalPosition(m_lastPosition);
		}

		private void ResetPlayTimer()
		{
			m_playTimer = 0f;
		}

		public void Activate(UIAtlas spriteAtlas, string spriteName, Vector3 initialPosition)
		{
			ResetPlayTimer();
			m_returningItem = false;
			m_velocity = Vector3.zero;
			base.transform.position = initialPosition;
			m_sprite.atlas = spriteAtlas;
			m_sprite.spriteName = spriteName;
			m_sprite.MakePixelPerfect();
			m_OriginalScale = m_sprite.transform.localScale;
			m_sprite.transform.localScale = m_OriginalScale * m_ItemEnlargeScale;
			base.gameObject.SetActive(true);
		}

		public void InitializeManually(float timeOutSecs, UISprite targetSprite, Vector3 initialPosition, float scaler)
		{
			TimeOutDelay = timeOutSecs;
			m_sprite = targetSprite;
			m_ItemEnlargeScale = scaler;
			Activate(m_sprite.atlas, m_sprite.spriteName, initialPosition);
			ResetPosition(initialPosition);
			m_bounceBounds.center = initialPosition;
			m_bounceBounds.min = new Vector2(0f - HalfWidth + BounceBorder, 0f - HalfHeight + BounceBorder);
			m_bounceBounds.max = new Vector2(HalfWidth - BounceBorder, HalfHeight - BounceBorder);
		}

		public void Deactivate()
		{
			base.gameObject.SetActive(false);
		}

		private void ItemGiven()
		{
			if (this.OnGiven != null)
			{
				this.OnGiven();
			}
			if (m_HoseDroppedItem != null)
			{
				base.gameObject.SetActive(false);
				m_HoseDroppedItem.localScale = Vector3.zero;
			}
		}

		private void ItemBounced(bool wasTouched)
		{
			if (!wasTouched && this.OnBounce != null)
			{
				this.OnBounce();
			}
		}

		private void Timeout()
		{
			if (this.OnTimeOut != null)
			{
				this.OnTimeOut();
			}
		}

		private void ItemFlicked()
		{
			if (this.OnFlicked != null)
			{
				this.OnFlicked();
			}
		}

		private void ItemGrabbed()
		{
			if (this.OnGrabbed != null)
			{
				this.OnGrabbed();
			}
		}

		private void Awake()
		{
			m_sprite = GetComponent<UISprite>();
			m_spriteSize = new Vector2(base.transform.localScale.x, base.transform.localScale.y);
			m_bounceBounds = default(Bounds);
			m_bounceBounds.min = new Vector2(0f - HalfWidth + BounceBorder, 0f - HalfHeight + BounceBorder);
			m_bounceBounds.max = new Vector2(HalfWidth - BounceBorder, HalfHeight - BounceBorder);
		}

		private Vector3 GetNormalisedMousePosition()
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.x /= Screen.width;
			mousePosition.y /= Screen.height;
			return mousePosition;
		}

		private void Update()
		{
			bool flag = false;
			Vector3 localPosition = base.transform.localPosition;
			if (Input.GetMouseButton(0) && !m_IsDraggingItem)
			{
				flag = true;
				m_returningItem = false;
				m_velocity = Vector3.zero;
				bool flag2 = false;
				Vector2 vector = Input.mousePosition;
				Ray ray = UICamera.currentCamera.ScreenPointToRay(vector);
				RaycastHit hitInfo;
				if (base.collider.Raycast(ray, out hitInfo, 1000f))
				{
					m_IsDraggingItem = true;
					m_TouchPosition = GetNormalisedMousePosition();
				}
			}
			else if (Input.GetMouseButton(0) && m_IsDraggingItem)
			{
				flag = true;
				m_returningItem = false;
				Vector3 normalisedMousePosition = GetNormalisedMousePosition();
				Vector3 vector2 = normalisedMousePosition - m_TouchPosition;
				m_TouchPosition = normalisedMousePosition;
				vector2.z = 0f;
				if ((vector2.x != 0f || vector2.y != 0f) && Time.deltaTime != 0f)
				{
					m_velocity = vector2 * DragSpeed;
				}
			}
			else if (Input.GetMouseButtonUp(0) && m_IsDraggingItem)
			{
				if (m_velocity.magnitude > m_ItemFlickedSpeed)
				{
					ItemFlicked();
				}
				m_IsDraggingItem = false;
			}
			float num = m_velocity.magnitude;
			if (Time.deltaTime == 0f)
			{
				num = 0f;
			}
			if (num > 0f)
			{
				num -= Time.deltaTime * 3f;
				m_velocity = m_velocity.normalized * num;
				if (localPosition.x <= m_bounceBounds.min.x || localPosition.x >= m_bounceBounds.max.x)
				{
					m_velocity.x *= -0.75f;
					ItemBounced(flag);
				}
				if (localPosition.y <= m_bounceBounds.min.y)
				{
					m_velocity.y *= -0.75f;
					ItemBounced(flag);
				}
				if (m_returningItem && localPosition.y >= m_bounceBounds.max.y && m_lastPosition.y <= m_bounceBounds.max.y)
				{
					m_velocity.y *= -0.75f;
					ItemBounced(flag);
				}
				if (!m_returningItem && localPosition.y >= m_bounceBounds.max.y + m_spriteSize.y)
				{
					m_velocity = Vector2.zero;
					ItemGiven();
				}
				localPosition += m_velocity;
			}
			m_lastPosition = base.transform.localPosition;
			localPosition.x = Mathf.Clamp(localPosition.x, m_bounceBounds.min.x, m_bounceBounds.max.x);
			localPosition.y = Mathf.Clamp(localPosition.y, m_bounceBounds.min.y, m_bounceBounds.max.y + m_spriteSize.y * m_ItemEnlargeScale);
			SetLocalPosition(localPosition);
			if (flag || num > 5f)
			{
				ResetPlayTimer();
			}
			else if (m_shouldTimeout)
			{
				m_playTimer += Time.deltaTime;
				if (m_playTimer >= TimeOutDelay)
				{
					Timeout();
				}
			}
		}

		private void SetLocalPosition(Vector3 pos)
		{
			base.transform.localPosition = pos;
			if (m_HoseDroppedItem != null)
			{
				Vector3 localPosition = m_HoseDroppedItem.localPosition;
				localPosition.x = pos.x * m_HoseDroppedItemPositionScale;
				localPosition.z = pos.y * m_HoseDroppedItemPositionScale + m_HoseDroppedItemPositionOffset;
				m_HoseDroppedItem.localPosition = localPosition;
			}
		}

		public void ResetPosition(Vector3 pos)
		{
			SetLocalPosition(pos);
		}
	}
}
