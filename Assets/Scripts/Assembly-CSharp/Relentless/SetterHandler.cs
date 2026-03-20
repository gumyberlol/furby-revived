using System;
using System.Reflection;

namespace Relentless
{
	public class SetterHandler : ISetterHandler
	{
		private MemberInfo m_memberInfo;

		public SetterHandler(MemberInfo memberInfo)
		{
			m_memberInfo = memberInfo;
		}

		public void Call(object target, object value)
		{
			if (m_memberInfo is PropertyInfo)
			{
				CallPropertySetter((PropertyInfo)m_memberInfo, target, value);
			}
			else if (m_memberInfo is FieldInfo)
			{
				CallFieldSetter((FieldInfo)m_memberInfo, target, value);
			}
		}

		private static void CallPropertySetter(PropertyInfo propertyInfo, object target, object value)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(target, value, null);
			}
		}

		private static void CallFieldSetter(FieldInfo fieldInfo, object target, object value)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
			{
				fieldInfo.SetValue(target, value);
			}
		}
	}
}
