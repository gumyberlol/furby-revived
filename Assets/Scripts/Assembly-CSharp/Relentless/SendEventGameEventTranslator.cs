namespace Relentless
{
	public class SendEventGameEventTranslator : GameEventTranslator<SendEventReaction>
	{
		public SendEventReaction[] m_eventTable;

		protected override SendEventReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}
	}
}
