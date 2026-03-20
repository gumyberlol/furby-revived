using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class SlotReference
	{
		public SlotHandler m_Handler;

		public UISprite m_Sprite;

		public SphereCollider m_Collider;

		public GameObject m_VFX;

		public SlotHandler Handler
		{
			get
			{
				return m_Handler;
			}
			set
			{
				m_Handler = value;
			}
		}

		public UISprite Sprite
		{
			get
			{
				return m_Sprite;
			}
			set
			{
				m_Sprite = value;
			}
		}

		public SphereCollider Collider
		{
			get
			{
				return m_Collider;
			}
			set
			{
				m_Collider = value;
			}
		}

		public GameObject VFX
		{
			get
			{
				return m_VFX;
			}
			set
			{
				m_VFX = value;
			}
		}
	}
}
