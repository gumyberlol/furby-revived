namespace Furby.FurMail
{
	public class Message
	{
		private string m_TitleText;

		private string m_ContentsText;

		private bool m_IsImportant;

		private bool m_HasBeenRead;

		public Message(string titleText, string contentsText, bool isImportant)
		{
			m_TitleText = titleText;
			m_ContentsText = contentsText;
			m_IsImportant = isImportant;
			m_HasBeenRead = false;
		}

		public bool IsImportant()
		{
			return m_IsImportant;
		}

		public bool IsUnread()
		{
			return !m_HasBeenRead;
		}

		public void SetRead()
		{
			m_HasBeenRead = true;
		}

		public string GetTitleText()
		{
			return m_TitleText;
		}

		public string GetContentsText()
		{
			return m_ContentsText;
		}
	}
}
