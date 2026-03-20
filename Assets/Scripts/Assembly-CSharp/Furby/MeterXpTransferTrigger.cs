using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class MeterXpTransferTrigger : RelentlessMonoBehaviour
	{
		public int m_particleEmissionRate = 8;

		public ParticleSystem[] m_particleSystems;

		public bool m_disableEmissionAtStart = true;

		[SerializeField]
		private SerialisableEnum[] m_startEvent;

		private float[] m_previousValues;

		private void Start()
		{
			if (m_disableEmissionAtStart)
			{
				ParticleSystem[] particleSystems = m_particleSystems;
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					particleSystem.emissionRate = 0f;
				}
			}
			HashSet<SerialisableEnum> hashSet = new HashSet<SerialisableEnum>();
			SerialisableEnum[] startEvent = m_startEvent;
			foreach (SerialisableEnum item in startEvent)
			{
				hashSet.Add(item);
			}
			foreach (SerialisableEnum item2 in hashSet)
			{
				GameEventRouter.AddDelegateForEnums(OnFurbyStatEvent, item2);
			}
			m_previousValues = new float[m_startEvent.Length];
		}

		private void OnDestroy()
		{
			HashSet<SerialisableEnum> hashSet = new HashSet<SerialisableEnum>();
			SerialisableEnum[] startEvent = m_startEvent;
			foreach (SerialisableEnum item in startEvent)
			{
				hashSet.Add(item);
			}
			foreach (SerialisableEnum item2 in hashSet)
			{
				GameEventRouter.RemoveDelegateForEnums(OnFurbyStatEvent, item2);
			}
		}

		private void OnFurbyStatEvent(Enum evt, GameObject originator, params object[] parameters)
		{
			int num = 0;
			SerialisableEnum[] startEvent = m_startEvent;
			foreach (SerialisableEnum serialisableEnum in startEvent)
			{
				if (serialisableEnum.Value.Equals(evt))
				{
					float num2 = (float)parameters[1];
					if (num2 > 0f && (float)parameters[0] > m_previousValues[num])
					{
						StartCoroutine(EmitParticles(num, num2));
					}
					m_previousValues[num] = (float)parameters[0];
				}
				num++;
			}
		}

		private IEnumerator EmitParticles(int index, float timer)
		{
			TriggerXpTransferParticles(true, index);
			yield return new WaitForSeconds(timer);
			TriggerXpTransferParticles(false, index);
		}

		public void TriggerXpTransferParticles(bool emit, int meterIndex)
		{
			if (emit)
			{
				GameEventRouter.SendEvent(DashboardGameEvent.Meter_Bubbles);
				m_particleSystems[meterIndex].emissionRate = m_particleEmissionRate;
			}
			else
			{
				m_particleSystems[meterIndex].emissionRate = 0f;
			}
		}
	}
}
