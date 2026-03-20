using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities
{
	public class FurbyComAirWaiter
	{
		private const float m_InternalTimeOut = 4f;

		private bool m_ReceivedResponse;

		private float m_WaitedTime;

		private bool m_InterruptAndExit;

		public IEnumerator SendComAirEventAndWaitForResponse(FurbyAction action, FurbyCommand command, bool shouldWaitForResponse, float timeout = float.MaxValue)
		{
			m_InterruptAndExit = false;
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			float timeTaken = 0f;
			if (shouldWaitForResponse)
			{
				m_ReceivedResponse = false;
				bool postedMessage = false;
				bool waiting = true;
				m_WaitedTime = 0f;
				while (!postedMessage && timeTaken < timeout)
				{
					if (m_InterruptAndExit)
					{
						yield break;
					}
					postedMessage = ((action == (FurbyAction)0) ? dataChannel.PostCommand(command, delegate(bool b)
					{
						m_ReceivedResponse = b;
						waiting = false;
					}) : dataChannel.PostAction(action, delegate(bool b)
					{
						m_ReceivedResponse = b;
						waiting = false;
					}));
					m_WaitedTime += Time.deltaTime;
					if (m_WaitedTime >= 4f)
					{
						postedMessage = true;
						waiting = false;
					}
					yield return null;
					timeTaken += Time.deltaTime;
				}
				if (action != 0)
				{
					Logging.Log(string.Format("<-- Started waiting for response to {0}", action.ToString()));
				}
				else
				{
					Logging.Log(string.Format("<-- Started waiting for response to {0}", command.ToString()));
				}
				while (waiting && timeTaken < timeout)
				{
					if (m_InterruptAndExit)
					{
						yield break;
					}
					yield return null;
					timeTaken += Time.deltaTime;
				}
				Logging.Log(string.Format("<-- Stopped waiting for response. Received Response = {0}", m_ReceivedResponse));
				yield break;
			}
			bool postedMessage2 = false;
			while (!postedMessage2 && timeTaken < timeout && !m_InterruptAndExit)
			{
				postedMessage2 = ((action == (FurbyAction)0) ? dataChannel.PostCommand(command, null) : dataChannel.PostAction(action, null));
				m_WaitedTime += Time.deltaTime;
				if (m_WaitedTime >= 4f)
				{
					postedMessage2 = true;
				}
				yield return null;
				timeTaken += Time.deltaTime;
			}
		}

		public void Interrupt()
		{
			m_InterruptAndExit = true;
		}

		public bool ReceivedResponse()
		{
			return m_ReceivedResponse;
		}

		public void OverrideReceivedResponse(bool received)
		{
			m_ReceivedResponse = received;
		}
	}
}
