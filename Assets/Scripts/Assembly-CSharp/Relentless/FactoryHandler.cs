using System;
using System.Reflection;

namespace Relentless
{
	public class FactoryHandler : IFactoryHandler
	{
		private ConstructorInfo m_ctorInfo;

		public FactoryHandler(ConstructorInfo ctorInfo)
		{
			m_ctorInfo = ctorInfo;
		}

		public object Call(params object[] args)
		{
			return CallTypeFactory(m_ctorInfo, args);
		}

		private static object CallTypeFactory(ConstructorInfo ctor, params object[] args)
		{
			if (ctor == null)
			{
				throw new ArgumentNullException("ctor");
			}
			Type declaringType = ctor.DeclaringType;
			ParameterInfo[] parameters = ctor.GetParameters();
			if (parameters.Length != args.Length)
			{
				throw new TypeLoadException("Missing constructor arguments");
			}
			return Activator.CreateInstance(declaringType, args);
		}
	}
}
