using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorFastForwarder : GameEventConsumer<IncubatorGameEvent>
	{
		[SerializeField]
		private GameObject m_WidgetLocation;

		[SerializeField]
		private UILabel m_CounterLabel;

		[SerializeField]
		private UITexture m_CounterBG;

		[SerializeField]
		private GameObject m_ButtonBG_Root;

		[SerializeField]
		private IncubatorLogic m_GameLogic;

		[SerializeField]
		private Animation m_AnimationTarget;

		[SerializeField]
		private BoxCollider m_BoxCollider;

		public void Start()
		{
			m_WidgetLocation.SetActive(m_GameLogic.FastForwardVisible);
			m_AnimationTarget.enabled = IncubatorLogic.FurbyBaby.IncubationFastForwarded;
			m_BoxCollider.enabled = !m_AnimationTarget.enabled;
		}

		public void Update()
		{
			if (!m_GameLogic.FastForwardsUsable.HasValue)
			{
				m_CounterLabel.enabled = false;
				m_AnimationTarget.enabled = true;
				m_BoxCollider.enabled = false;
			}
			else if (m_GameLogic.AreFastForwardsViable())
			{
				m_CounterLabel.enabled = m_GameLogic.FastForwardThresholdReached;
			}
			else
			{
				m_BoxCollider.enabled = false;
				m_CounterLabel.enabled = false;
				m_ButtonBG_Root.SetActive(false);
			}
			m_CounterBG.enabled = m_CounterLabel.enabled;
			m_CounterLabel.text = FurbyGlobals.Player.IncubatorFastForwards.ToString();
		}

		private void OnClick()
		{
			if (m_GameLogic.FastForwardThresholdReached)
			{
				bool? fastForwardsUsable = m_GameLogic.FastForwardsUsable;
				if (!fastForwardsUsable.HasValue)
				{
					return;
				}
				switch (fastForwardsUsable.Value)
				{
				default:
					return;
				case true:
					GameEventRouter.SendEvent(IncubatorDialogEvent.Interruption_FastForward_Activated);
					return;
				case false:
					break;
				}
				if (m_GameLogic.FastForwardConsumables.Count() > 0 || SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable())
				{
					GameEventRouter.SendEvent(IncubatorDialogEvent.Interruption_FastForward_Purchase);
					return;
				}
			}
			GameEventRouter.SendEvent(IncubatorDialogEvent.Interruption_FastForward_Upsell);
		}
	}
}
