using System;
using System.Reflection;

namespace Relentless
{
	public class ProxyHandler : IProxyHandler
	{
		private MethodInfo m_methodInfo;

		public ProxyHandler(MethodInfo methodInfo)
		{
			m_methodInfo = methodInfo;
		}

		public object Call(object target, params object[] args)
		{
			return CallMethodProxy(m_methodInfo, target, args);
		}

		private static object CallMethodProxy(MethodInfo methodInfo, object target, params object[] args)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (args.Length != parameters.Length)
			{
				throw new ArgumentException("Missing method arguments");
			}
			foreach (ParameterInfo parameterInfo in parameters)
			{
				if (parameterInfo.IsOut)
				{
					throw new NotSupportedException("CallMethodProxy does not support out parameters.");
				}
			}
			object result = methodInfo.Invoke(target, args);
			if (methodInfo.ReturnType == typeof(void))
			{
				return null;
			}
			return result;
		}
	}
}
