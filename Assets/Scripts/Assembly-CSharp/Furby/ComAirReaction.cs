using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class ComAirReaction : GameEventReaction
	{
		public enum ComAirCodeType
		{
			Command = 0,
			Action = 1
		}

		public ComAirCodeType CodeType;

		[SplitEnumDropdown("_")]
		public FurbyCommand[] Command;

		[SplitEnumDropdown("_")]
		public FurbyAction[] Action;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			FurbyReply furbyReply = null;
			if (paramlist != null && paramlist.Length == 1)
			{
				furbyReply = paramlist[0] as FurbyReply;
			}
			if (CodeType == ComAirCodeType.Command)
			{
				int num = UnityEngine.Random.Range(0, Command.Length);
				Singleton<FurbyDataChannel>.Instance.PostCommand(Command[num], furbyReply);
			}
			else
			{
				int num2 = UnityEngine.Random.Range(0, Action.Length);
				Singleton<FurbyDataChannel>.Instance.PostAction(Action[num2], furbyReply);
			}
		}
	}
}
