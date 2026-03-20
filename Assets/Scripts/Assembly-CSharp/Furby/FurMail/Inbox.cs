using System.Collections.Generic;

namespace Furby.FurMail
{
	public class Inbox
	{
		private const int MaximumMessageCount = 20;

		private List<Message> m_Messages = new List<Message>();

		public void AddMessage(Message newMessage)
		{
			int index = FindInsertIndex(newMessage);
			m_Messages.Insert(index, newMessage);
			int count = m_Messages.Count;
			if (count > 20)
			{
				m_Messages.RemoveAt(count - 1);
			}
		}

		private int FindInsertIndex(Message newMessage)
		{
			int count = m_Messages.Count;
			if (count == 0)
			{
				return 0;
			}
			if (newMessage.IsImportant())
			{
				return 0;
			}
			for (int i = 0; i < count; i++)
			{
				if (!m_Messages[i].IsImportant())
				{
					return i;
				}
			}
			return count;
		}

		public int GetNumberOfMessages()
		{
			return m_Messages.Count;
		}

		public int GetNumberOfUnreadMessages()
		{
			int num = 0;
			foreach (Message message in m_Messages)
			{
				if (message.IsUnread())
				{
					num++;
				}
			}
			return num;
		}

		public string GetMessageTitle(int index)
		{
			return m_Messages[index].GetTitleText();
		}

		public string GetMessageContents(int index)
		{
			return m_Messages[index].GetContentsText();
		}

		public bool IsMessageUnread(int index)
		{
			return m_Messages[index].IsUnread();
		}

		public void SetMessageRead(int index)
		{
			m_Messages[index].SetRead();
		}
	}
}
