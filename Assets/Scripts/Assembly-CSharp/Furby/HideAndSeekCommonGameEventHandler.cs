using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class HideAndSeekCommonGameEventHandler : GameEventReceiver
	{
		public override Type EventType
		{
			get
			{
				return typeof(SharedGuiEvents);
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((SharedGuiEvents)(object)enumValue)
			{
			case SharedGuiEvents.Restart:
				Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = 0;
				break;
			case SharedGuiEvents.Quit:
				Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel = 0;
				break;
			}
		}
	}
}
