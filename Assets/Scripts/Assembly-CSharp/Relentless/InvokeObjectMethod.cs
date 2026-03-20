using System;
using System.Reflection;
using HutongGames.PlayMaker;

namespace Relentless
{
	[Tooltip("Invokes a Method in a Object. Will currently ONLY work with methods with NO parameters")]
	[ActionCategory("Relentless")]
	public class InvokeObjectMethod : FsmStateAction
	{
		[RequiredField]
		public FsmObject targetObject;

		[RequiredField]
		public string methodName;

		public override void OnEnter()
		{
			Type type = targetObject.Value.GetType();
			MethodInfo method = type.GetMethod(methodName);
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length > 0)
			{
				Logging.LogError("InvokeObjectMethod Action can not execute a method with parameters!");
				Finish();
			}
			else
			{
				method.Invoke(targetObject.Value, null);
				Finish();
			}
		}
	}
}
