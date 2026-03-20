namespace Relentless
{
	public abstract class Achievement : RelentlessMonoBehaviour
	{
		public NamedTextReference m_namedTextTitle;

		public NamedTextReference m_namedTextBeforeDescription;

		public NamedTextReference m_namedTextAfterDescription;

		public abstract bool IsUnlocked();
	}
}
