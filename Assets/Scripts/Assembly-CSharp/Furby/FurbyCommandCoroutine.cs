using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyCommandCoroutine
	{
		private bool m_ReplyResult;

		private bool m_ForceAdvance;

		public bool ReplyResult
		{
			get
			{
				return m_ReplyResult;
			}
		}

		public Coroutine PostAwaitReply(MonoBehaviour gameScript, FurbyCommand furbyCommand, int retryCount)
		{
			return gameScript.StartCoroutine(PostAwaitReply(furbyCommand, retryCount));
		}

		public IEnumerator PostAwaitReply(FurbyCommand furbyCommand, int retryCount)
		{
			FurbyDataChannel furbyComms = Singleton<FurbyDataChannel>.Instance;
			m_ReplyResult = false;
			for (int i = 0; i < retryCount; i++)
			{
				if (m_ReplyResult)
				{
					break;
				}
				bool replyReceived = false;
				if (furbyComms.PostCommand(furbyCommand, delegate(bool flag)
				{
					replyReceived = true;
					m_ReplyResult |= flag;
				}))
				{
					while (!replyReceived && !m_ReplyResult)
					{
						yield return null;
					}
				}
				for (int j = 0; j < 20; j++)
				{
					if (m_ReplyResult)
					{
						break;
					}
					yield return new WaitForSeconds(0.1f);
				}
				m_ReplyResult |= m_ForceAdvance;
			}
		}

		public void ForceReply()
		{
			m_ForceAdvance = true;
			m_ReplyResult = true;
		}
	}
}
