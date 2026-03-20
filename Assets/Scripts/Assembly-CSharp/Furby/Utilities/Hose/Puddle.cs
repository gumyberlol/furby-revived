using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class Puddle : MonoBehaviour
	{
		public delegate void Handler();

		[SerializeField]
		private float m_cleanliness = 1f;

		[SerializeField]
		private GameObject m_prefab;

		[SerializeField]
		private AnimationClip m_colourClip;

		[SerializeField]
		private float m_timeToClearFromDirty = 10f;

		[SerializeField]
		private Transform m_ColourAnimMixingTransform;

		private GameObject m_asset;

		private Animation m_anim;

		private AnimationState m_colourState;

		public ArriveAndDisperse ArriveAndDisperse
		{
			get
			{
				return base.gameObject.GetComponent<ArriveAndDisperse>();
			}
		}

		public event Handler Destroying;

		public void Start()
		{
			m_asset = Object.Instantiate(m_prefab) as GameObject;
			m_anim = null;
			foreach (Transform item in m_asset.transform)
			{
				m_anim = item.animation;
				if (m_anim != null)
				{
					break;
				}
			}
			ArriveAndDisperse arriveAndDisperse = ArriveAndDisperse;
			arriveAndDisperse.SetAnim(m_anim);
			foreach (AnimationState item2 in m_anim)
			{
				item2.layer = 1;
			}
			m_colourState = m_anim[m_colourClip.name];
			m_colourState.layer = 2;
			m_colourState.speed = 0f;
			m_colourState.wrapMode = WrapMode.Loop;
			SetAnimCleanliness(m_cleanliness);
			m_anim.Play(m_colourState.name);
			arriveAndDisperse.DispersalCompleted += delegate
			{
				Object.Destroy(base.gameObject);
			};
			arriveAndDisperse.ArrivalStarted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.PuddleFillStart);
			};
			arriveAndDisperse.ArrivalCompleted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.PuddleFillComplete);
			};
			arriveAndDisperse.DispersalStarted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.PuddleEmptyStart);
			};
			arriveAndDisperse.DispersalCompleted += delegate
			{
				base.gameObject.SendGameEvent(HoseGameEvent.PuddleEmptyComplete);
			};
			arriveAndDisperse.Arrive();
		}

		public void Update()
		{
			float animCleanliness = GetAnimCleanliness();
			float num = m_cleanliness - animCleanliness;
			bool flag = num < 0f;
			num *= ((!flag) ? 1f : (-1f));
			float b = Time.deltaTime / m_timeToClearFromDirty;
			num = Mathf.Min(num, b);
			num *= ((!flag) ? 1f : (-1f));
			float value = animCleanliness + num;
			value = Mathf.Clamp01(value);
			SetAnimCleanliness(value);
			m_anim.Sample();
		}

		public void SetImmediateCleanliness(float c)
		{
			c = Mathf.Clamp01(c);
			m_cleanliness = c;
			if (m_colourState != null)
			{
				SetAnimCleanliness(c);
			}
		}

		public void TendTowardsCleanliness(float c)
		{
			m_cleanliness = Mathf.Clamp01(c);
		}

		private void SetAnimCleanliness(float c)
		{
			m_colourState.normalizedTime = 1f - c;
		}

		private float GetAnimCleanliness()
		{
			return 1f - m_colourState.normalizedTime;
		}

		public void OnDestroy()
		{
			if (this.Destroying != null)
			{
				this.Destroying();
			}
			Object.Destroy(m_asset);
		}
	}
}
