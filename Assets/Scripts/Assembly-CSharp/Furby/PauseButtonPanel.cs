using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PauseButtonPanel : CommonPanel
	{
		public delegate void CancellationRequest(HashSet<string> reasons);

		public delegate void Handler();

		public override Type EventType
		{
			get
			{
				return typeof(SharedGuiEvents);
			}
		}

		public event CancellationRequest Pausing;

		public event Handler Enabled;

		public void OnClickPause()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (this.Pausing != null)
			{
				this.Pausing(hashSet);
			}
			if (hashSet.Count <= 0)
			{
				GameEventRouter.SendEvent(SharedGuiEvents.Pause);
				SetEnabled(false);
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			SharedGuiEvents sharedGuiEvents = (SharedGuiEvents)(object)enumValue;
			if (sharedGuiEvents == SharedGuiEvents.Resume || sharedGuiEvents == SharedGuiEvents.Restart)
			{
				SetEnabled(true);
				if (this.Enabled != null)
				{
					this.Enabled();
				}
			}
		}
	}
}
