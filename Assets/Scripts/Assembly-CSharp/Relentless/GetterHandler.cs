using System;
using System.Reflection;

namespace Relentless
{
	public class GetterHandler : IGetterHandler
	{
		private readonly MemberInfo m_memberInfo;

		public GetterHandler(MemberInfo memberInfo)
		{
			m_memberInfo = memberInfo;
		}

		public object Call(object target)
		{
			if (m_memberInfo is PropertyInfo)
			{
				return CallPropertyGetter((PropertyInfo)m_memberInfo, target);
			}
			if (m_memberInfo is FieldInfo)
			{
				return CallFieldGetter((FieldInfo)m_memberInfo, target);
			}
			return null;
		}

		private static object CallPropertyGetter(PropertyInfo propertyInfo, object target)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			if (!propertyInfo.CanRead)
			{
				return null;
			}
			return propertyInfo.GetValue(target, null);
		}

		private static object CallFieldGetter(FieldInfo fieldInfo, object target)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			return fieldInfo.GetValue(target);
		}
	}
}
