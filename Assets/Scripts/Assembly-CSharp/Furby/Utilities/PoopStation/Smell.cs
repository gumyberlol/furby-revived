using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class Smell : MonoBehaviour
	{
		public delegate void DeathHandler();

		[SerializeField]
		private float surpressionPause;

		[SerializeField]
		private float surpressionRequired;

		private float surpressionCountdown = -1f;

		private float totalSurpression;

		private bool m_dying;

		public float m_IntervalBeforeOfferingHints = 4f;

		private bool m_EnableHints;

		public event DeathHandler m_died;

		public void MultiplyEmissionRate(float multiplier)
		{
			float emissionRate = base.particleSystem.emissionRate;
			float emissionRate2 = emissionRate * multiplier;
			base.particleSystem.emissionRate = emissionRate2;
		}

		public void Surpress()
		{
			base.particleSystem.Stop();
			surpressionCountdown = surpressionPause;
		}

		public void Start()
		{
			m_EnableHints = true;
			FurbyGlobals.InputInactivity.ResetInactivity();
		}

		public void Update()
		{
			if (!m_dying)
			{
				if (m_EnableHints && FurbyGlobals.InputInactivity.HasIntervalPassed(m_IntervalBeforeOfferingHints))
				{
					FurbyGlobals.InputInactivity.ResetInactivity();
					GameEventRouter.SendEvent(HintEvents.PoopStation_SuggestRemovingSmell);
				}
				UpdateTimers();
			}
		}

		private void OnDisable()
		{
			m_EnableHints = false;
		}

		private void UpdateTimers()
		{
			if (surpressionCountdown > 0f)
			{
				surpressionCountdown -= Time.deltaTime;
			}
			if (surpressionCountdown > 0f)
			{
				totalSurpression += Time.deltaTime;
				if (totalSurpression > surpressionRequired)
				{
					StartCoroutine(WaitForParticlesAndDestroy());
				}
			}
			else
			{
				if (!base.particleSystem.isPlaying)
				{
					base.particleSystem.Play();
				}
				totalSurpression = 0f;
			}
		}

		private IEnumerator WaitForParticlesAndDestroy()
		{
			m_dying = true;
			while (base.particleSystem.IsAlive())
			{
				yield return null;
			}
			base.gameObject.SendGameEvent(PoopStationEvent.SmellDefeated, this);
			if (this.m_died != null)
			{
				this.m_died();
			}
			Object.Destroy(base.gameObject);
		}
	}
}
