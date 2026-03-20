using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class SerialisableEnum
	{
		private Enum m_enum;

		private Type m_type;

		[SerializeField]
		private string m_fullTypeName = string.Empty;

		[SerializeField]
		private int m_enumIntValue;

		[SerializeField]
		private string m_enumStringValue;

		public Enum Value
		{
			get
			{
				if (m_enum == null)
				{
					RecreateSystemEnum();
				}
				return m_enum;
			}
			set
			{
				if (m_type != null && value.GetType() != m_type)
				{
					throw new Exception(string.Format("Cannot change enum type from {0} to {1}", m_type.Name, value.GetType().Name));
				}
				if (m_type == null)
				{
					m_type = value.GetType();
					m_fullTypeName = m_type.FullName;
					m_enumIntValue = Convert.ToInt32(value);
					m_enumStringValue = value.ToString();
				}
				m_enum = value;
			}
		}

		public Type Type
		{
			get
			{
				if (m_type == null)
				{
					RecreateSystemEnum();
				}
				return m_type;
			}
		}

		public SerialisableEnum(Enum enumValue)
		{
			Value = enumValue;
		}

		public bool IsTypeSet()
		{
			return m_fullTypeName != string.Empty;
		}

		private void RecreateSystemEnum()
		{
			m_type = SerialisableType.FindType(m_fullTypeName);
			if (string.IsNullOrEmpty(m_enumStringValue))
			{
				Logging.LogWarning("Error recreating SerialisableEnum - no string value set");
				m_enum = (Enum)Enum.ToObject(m_type, m_enumIntValue);
				return;
			}
			try
			{
				m_enum = (Enum)Enum.Parse(m_type, m_enumStringValue);
			}
			catch
			{
				if (m_type != null)
				{
					Logging.LogError("SerialisableEnum of type " + m_type.ToString() + " doesn't exist: " + m_enumStringValue + ". IntValue is " + m_enumIntValue + " (" + Enum.ToObject(m_type, m_enumIntValue).ToString() + ")");
					m_enum = (Enum)Enum.ToObject(m_type, m_enumIntValue);
				}
			}
			if (!m_enum.Equals(Enum.ToObject(m_type, m_enumIntValue)))
			{
				Logging.LogWarning("SerialisableEnum of type " + m_type.ToString() + " doesn't match: " + m_enumIntValue + " (" + Enum.ToObject(m_type, m_enumIntValue).ToString() + ") and " + m_enumStringValue);
			}
		}

		public static implicit operator SerialisableEnum(Enum enumValue)
		{
			return new SerialisableEnum(enumValue);
		}

		public static implicit operator Enum(SerialisableEnum serialisableEnum)
		{
			return serialisableEnum.Value;
		}
	}
}
