using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class Bowl : MonoBehaviour
	{
		private class EnabledPeriod : IDisposable
		{
			private Bowl b;

			public EnabledPeriod(Bowl b)
			{
				this.b = b;
				b.EnableFlush();
			}

			public void Dispose()
			{
				b.DisableFlush();
			}
		}

		public GameObject m_asset;

		private HashSet<Poop> m_poops = new HashSet<Poop>();

		public float m_IntervalBeforeOfferingHints = 4f;

		private bool m_EnableHints;

		private bool m_flushing;

		private uint m_enableCount;

		private void EnableFlush()
		{
			m_enableCount++;
			m_EnableHints = true;
			FurbyGlobals.InputInactivity.ResetInactivity();
		}

		private void DisableFlush()
		{
			m_enableCount--;
			m_EnableHints = false;
		}

		private bool IsEnabled()
		{
			return m_enableCount != 0;
		}

		public IDisposable GetEnabledPeriod()
		{
			return new EnabledPeriod(this);
		}

		public void ReceivePoop(Poop p)
		{
			m_poops.Add(p);
		}

		public bool HasPoops()
		{
			return m_poops.Count > 0;
		}

		public IEnumerator<Poop> GetPoopEnumerator()
		{
			return m_poops.GetEnumerator();
		}

		public void Flush()
		{
			if (IsEnabled() && !m_flushing)
			{
				m_EnableHints = false;
				m_flushing = true;
				base.gameObject.SendGameEvent(PoopStationEvent.FlushPressed);
				StartCoroutine(FlushCoroutine());
			}
		}

		public bool IsFlushing()
		{
			return m_flushing;
		}

		private IEnumerator FlushCoroutine()
		{
			Animation anim = m_asset.animation;
			anim.Play("poopStation_flush");
			HashSet<Poop> poops = m_poops;
			m_poops = new HashSet<Poop>();
			foreach (Poop p in poops)
			{
				p.Leave(anim);
			}
			while (anim.isPlaying)
			{
				yield return null;
			}
			while (true)
			{
				bool allDone = true;
				foreach (Poop p2 in poops)
				{
					allDone &= p2.HasLeft();
				}
				if (allDone)
				{
					break;
				}
				yield return null;
			}
			foreach (Poop p3 in poops)
			{
				UnityEngine.Object.Destroy(p3.gameObject);
			}
			poops.Clear();
			anim.Rewind();
			anim.Play();
			anim.Sample();
			anim.Stop();
			m_flushing = false;
			base.gameObject.SendGameEvent(PoopStationEvent.FlushCompleted);
		}

		private void Start()
		{
			m_EnableHints = false;
		}

		private void Update()
		{
			if (m_EnableHints && FurbyGlobals.InputInactivity.HasIntervalPassed(m_IntervalBeforeOfferingHints))
			{
				FurbyGlobals.InputInactivity.ResetInactivity();
				GameEventRouter.SendEvent(HintEvents.PoopStation_SuggestFlushingToilet);
			}
		}
	}
}
