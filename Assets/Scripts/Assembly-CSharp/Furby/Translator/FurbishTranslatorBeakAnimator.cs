using System;
using Relentless;
using UnityEngine;

namespace Furby.Translator
{
	public class FurbishTranslatorBeakAnimator : GameEventReceiver
	{
		[SerializeField]
		private string m_SpriteName;

		private UISprite m_TargetSprite;

		private float m_AnimationLength = 1f;

		private float m_AnimationTime = -1f;

		public override Type EventType
		{
			get
			{
				return typeof(FurbishTranslatorEvent);
			}
		}

		public void Start()
		{
			m_TargetSprite = GetComponent<UISprite>();
		}

		protected override void OnEvent(Enum type, GameObject sender, object[] arguments)
		{
			if ((FurbishTranslatorEvent)(object)type == FurbishTranslatorEvent.Synchronized)
			{
				m_AnimationLength = Mathf.Max(0f, m_AnimationLength);
				m_AnimationTime = 0f;
			}
		}

		public void Update()
		{
			int num = 0;
			int count = m_TargetSprite.atlas.spriteList.Count;
			if (m_AnimationTime < 0f)
			{
				num = 0;
			}
			else if (m_AnimationTime < m_AnimationLength)
			{
				float num2 = m_AnimationTime / m_AnimationLength;
				num = Mathf.RoundToInt(num2 * (float)count);
				num = Mathf.Clamp(num, 0, count - 1);
				m_AnimationTime += Time.deltaTime;
			}
			else
			{
				num = count - 1;
			}
			m_TargetSprite.spriteName = m_SpriteName + num;
		}
	}
}
