using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Relentless
{
	public abstract class ActivateGUIScreen : FsmStateAction
	{
		private List<GUIScreenManager> m_managers;

		public override void OnEnter()
		{
			if (m_managers == null)
			{
				GuiActionFsmActionReceiver component = base.Owner.GetComponent<GuiActionFsmActionReceiver>();
				if (component != null)
				{
					m_managers = component.ScreenManagers;
				}
			}
			base.OnEnter();
		}

		protected void ActivateNow(bool isActive)
		{
			string screenName = base.State.Name;
			if (m_managers != null && m_managers.Count > 0)
			{
				foreach (GUIScreenManager manager in m_managers)
				{
					if (isActive)
					{
						manager.SwitchScreen(screenName);
					}
					else
					{
						manager.HideScreen(screenName);
					}
				}
				return;
			}
			Logging.LogError("Could not find any screen managers", base.Fsm.GameObject);
		}
	}
}
