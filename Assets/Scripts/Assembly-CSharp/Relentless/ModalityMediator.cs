using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class ModalityMediator : SingletonInstance<ModalityMediator>
	{
		public delegate void DismissHandler();

		private MonoBehaviour m_ActiveClient;

		private DismissHandler m_DismissHandler;

		public bool IsAvailable()
		{
			bool result = true;
			if (m_ActiveClient != null)
			{
				result = m_DismissHandler != null;
			}
			return result;
		}

		public bool Acquire(MonoBehaviour clientObject, DismissHandler dismissHandler)
		{
			DebugUtils.AssertNotNull(clientObject);
			if (clientObject != m_ActiveClient)
			{
				DebugUtils.Assert(IsAvailable(), "ModalityMediator.AcquireModality : Modality not available!\n\nThis could indicate an issue with dialogs/panels that overlap\nwhen this is not the desired effect.  Please check.");
				if (m_ActiveClient != null)
				{
					if (m_DismissHandler == null)
					{
						return false;
					}
					m_DismissHandler();
				}
				m_DismissHandler = dismissHandler;
				m_ActiveClient = clientObject;
			}
			return true;
		}

		public void Release(MonoBehaviour clientObject)
		{
			DebugUtils.AssertNotNull(clientObject);
			if (m_ActiveClient == clientObject)
			{
				m_DismissHandler = null;
				m_ActiveClient = null;
			}
		}

		public IEnumerator WaitAndAcquire(MonoBehaviour clientObject, DismissHandler dismissHandler)
		{
			if (clientObject != m_ActiveClient)
			{
				while (!IsAvailable())
				{
					yield return null;
				}
				Acquire(clientObject, dismissHandler);
			}
		}
	}
}
