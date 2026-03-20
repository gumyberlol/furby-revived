using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PauseMenuPanel : CommonPanel
	{
		public delegate void CancellationRequest(HashSet<string> reasons);

		public delegate void Action();

		public override Type EventType
		{
			get
			{
				return typeof(SharedGuiEvents);
			}
		}

		public event CancellationRequest ResumeRequested;

		public event CancellationRequest RestartRequested;

		public event CancellationRequest QuitRequested;

		public event Action Resuming;

		public event Action Restarting;

		public event Action Quitting;

		public void OnClickResume()
		{
			ActionIfNoObjections(this.ResumeRequested, this.Resuming, delegate
			{
				GameEventRouter.SendEvent(SharedGuiEvents.Resume);
				SetDisabled(0.25f);
			});
		}

		public void OnClickRestart()
		{
			ActionIfNoObjections(this.RestartRequested, this.Restarting, delegate
			{
				GameEventRouter.SendEvent(SharedGuiEvents.Restart);
				SetDisabled(0.25f);
			});
		}

		public void OnClickQuit()
		{
			ActionIfNoObjections(this.QuitRequested, this.Quitting, delegate
			{
				GameEventRouter.SendEvent(SharedGuiEvents.Quit);
				SetDisabled(0.25f);
			});
		}

		private void ActionIfNoObjections(CancellationRequest cancellations, Action listeners, Action action)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (cancellations != null)
			{
				cancellations(hashSet);
			}
			if (hashSet.Count <= 0)
			{
				if (listeners != null)
				{
					listeners();
				}
				action();
			}
		}

		protected override void OnToggleWidgets(bool enabled)
		{
			base.OnToggleWidgets(enabled);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.enabled = enabled;
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if ((SharedGuiEvents)(object)enumValue == SharedGuiEvents.Pause)
			{
				SetEnabled(true);
			}
		}
	}
}
