using UnityEngine;

namespace Relentless
{
	public class NamedTextAttribute : PropertyAttribute
	{
		private string m_defaultDatabaseName = "USA";

		public string DatabaseName
		{
			get
			{
				return m_defaultDatabaseName;
			}
		}

		public NamedTextAttribute()
		{
		}

		public NamedTextAttribute(string databaseName)
		{
			m_defaultDatabaseName = databaseName;
		}
	}
}
