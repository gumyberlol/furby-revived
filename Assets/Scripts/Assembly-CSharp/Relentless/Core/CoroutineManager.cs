using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Relentless.Core
{
	public class CoroutineManager : RelentlessMonoBehaviour
	{
		private static readonly object m_lock = new object();

		private static readonly Dictionary<CoroutineMethod, bool> m_coroutines = new Dictionary<CoroutineMethod, bool>();

		private static bool m_shouldWaitForCoroutines = true;

		private static bool ShouldWaitForCoroutines
		{
			get
			{
				lock (m_lock)
				{
					return m_shouldWaitForCoroutines;
				}
			}
		}

		public static void BlockingCall(CoroutineMethod coroutineMethod)
		{
			Add(coroutineMethod);
			while (StillWaitingFor(coroutineMethod))
			{
				Thread.Sleep(100);
			}
		}

		public static void Add(CoroutineMethod coroutine)
		{
			lock (m_lock)
			{
				m_coroutines.Add(coroutine, false);
			}
		}

		public static bool StillWaitingFor(CoroutineMethod coroutine)
		{
			lock (m_lock)
			{
				return m_coroutines.ContainsKey(coroutine);
			}
		}

		public void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			StartCoroutine(ExecuteCoroutines());
		}

		private CoroutineMethod GetNextCoroutine()
		{
			lock (m_lock)
			{
				foreach (KeyValuePair<CoroutineMethod, bool> coroutine in m_coroutines)
				{
					if (!coroutine.Value)
					{
						CoroutineMethod key = coroutine.Key;
						m_coroutines[coroutine.Key] = true;
						return key;
					}
				}
				return null;
			}
		}

		private void MarkCoroutineAsFinished(CoroutineMethod coroutine)
		{
			lock (m_lock)
			{
				if (m_coroutines.ContainsKey(coroutine))
				{
					m_coroutines.Remove(coroutine);
				}
			}
		}

		private IEnumerator ExecuteCoroutines()
		{
			while (ShouldWaitForCoroutines)
			{
				CoroutineMethod coroutineMethod = GetNextCoroutine();
				if (coroutineMethod != null)
				{
					StartCoroutine("CoroutineRunner", coroutineMethod);
				}
				else
				{
					yield return new WaitForEndOfFrame();
				}
			}
		}

		private IEnumerator CoroutineRunner(object parameter)
		{
			CoroutineMethod coroutineMethod = (CoroutineMethod)parameter;
			if (coroutineMethod != null)
			{
				yield return StartCoroutine(coroutineMethod());
				MarkCoroutineAsFinished(coroutineMethod);
			}
		}
	}
}
