using System.Collections.Generic;

namespace Relentless
{
	public class ProviderManager<T, U> : SingletonInstance<T> where T : ProviderManager<T, U> where U : ProviderBase
	{
		private bool m_shouldInitialiseOnAwake = true;

		protected readonly List<U> m_providers = new List<U>();

		protected bool ShouldInitialiseOnAwake
		{
			get
			{
				return m_shouldInitialiseOnAwake;
			}
			set
			{
				m_shouldInitialiseOnAwake = value;
			}
		}

		public override void Awake()
		{
			base.Awake();
			if (ShouldInitialiseOnAwake)
			{
				Initialise();
			}
		}

		protected virtual void Initialise()
		{
			Logging.Log("Initialising providers for " + base.gameObject.name);
			m_providers.Clear();
			U[] components = base.gameObject.GetComponents<U>();
			for (int i = 0; i < components.Length; i++)
			{
				U item = components[i];
				if (item.IsValid())
				{
					m_providers.Add(item);
					item.Initialise();
				}
			}
			Logging.Log(string.Format("{0} ProviderManager: Initialised {1} providers", base.gameObject.name, m_providers.Count), this);
		}

		protected U GetHighestPriorityProvider()
		{
			U val = (U)null;
			foreach (U provider in m_providers)
			{
				if (val == null)
				{
					val = provider;
				}
				else if (val.ProviderPriority < provider.ProviderPriority)
				{
					val = provider;
				}
			}
			return val;
		}
	}
}
