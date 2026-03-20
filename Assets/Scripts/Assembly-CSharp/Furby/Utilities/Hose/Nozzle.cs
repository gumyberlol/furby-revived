using System;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class Nozzle : MonoBehaviour
	{
		[SerializeField]
		private Animation m_asset;

		[SerializeField]
		private Hose m_hose;

		public AnimationClip m_animClipPressure;

		public AnimationClip m_animClipTemperature;

		private AnimationState m_pressureAnimState;

		private AnimationState m_temperatureAnimState;

		public void Start()
		{
			if (m_asset == null)
			{
				throw new ApplicationException(string.Format("{0} needs m_asset set.", base.gameObject.name));
			}
			foreach (AnimationState item in m_asset)
			{
				if (item.name == m_animClipTemperature.name)
				{
					m_temperatureAnimState = item;
				}
				if (item.name == m_animClipPressure.name)
				{
					m_pressureAnimState = item;
				}
			}
			m_temperatureAnimState.layer = 1;
			m_pressureAnimState.layer = 2;
			m_temperatureAnimState.speed = 0f;
			m_pressureAnimState.speed = 0f;
			m_asset.Play(m_pressureAnimState.name);
			m_asset.Play(m_temperatureAnimState.name);
			if (m_hose == null)
			{
				throw new ApplicationException(string.Format("No Hose for Nozzle {0}", base.gameObject.name));
			}
			m_hose.Changed += SetPressureAndTemp;
		}

		public void SetPressureAndTemp(float p, float t)
		{
			SetValue(m_pressureAnimState, p);
			SetValue(m_temperatureAnimState, t);
			m_asset.Sample();
		}

		private static void SetValue(AnimationState state, float f)
		{
			state.normalizedTime = f;
		}
	}
}
