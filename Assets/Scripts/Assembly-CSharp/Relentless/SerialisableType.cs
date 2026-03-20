using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class SerialisableType
	{
		public class ValidTypes : PropertyAttribute
		{
			public enum StandardTypeSets
			{
				GameEventEnums = 0,
				AllGameTypes = 1
			}

			public delegate Type[] ValidTypesCallback();

			private Type[] m_types;

			public Type[] Types
			{
				get
				{
					return m_types;
				}
			}

			public ValidTypes(params Type[] types)
			{
				m_types = types;
			}

			public ValidTypes(StandardTypeSets types)
			{
				switch (types)
				{
				case StandardTypeSets.GameEventEnums:
					m_types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
						from type in assembly.GetTypes()
						where Attribute.IsDefined(type, typeof(GameEventEnum))
						orderby type.Name
						select type).ToArray();
					break;
				case StandardTypeSets.AllGameTypes:
					m_types = (from type in Assembly.GetExecutingAssembly().GetTypes()
						orderby type.Name
						select type).ToArray();
					break;
				}
			}
		}

		[SerializeField]
		private string m_qualifiedTypeName;

		private Type m_type;

		public Type Type
		{
			get
			{
				if (m_type == null)
				{
					m_type = FindType(m_qualifiedTypeName);
				}
				return m_type;
			}
			set
			{
				m_qualifiedTypeName = value.FullName;
				m_type = value;
			}
		}

		public SerialisableType(Type type)
		{
			Type = type;
		}

		public SerialisableType()
		{
		}

		public static Type FindType(string typename)
		{
			Type type = Type.GetType(typename);
			if (type == null)
			{
				IEnumerable<Type> source = from assembly in AppDomain.CurrentDomain.GetAssemblies()
					from assemblyType in assembly.GetTypes()
					where assemblyType.FullName == typename
					select assemblyType;
				type = source.FirstOrDefault();
			}
			return type;
		}

		public static implicit operator SerialisableType(Type type)
		{
			return new SerialisableType(type);
		}

		public static implicit operator Type(SerialisableType serializableType)
		{
			return serializableType.Type;
		}
	}
}
