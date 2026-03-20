using Relentless;
using UnityEngine;

namespace Furby
{
	public class InGameGUIInterface : Singleton<InGameGUIInterface>
	{
		public UISlider m_playerHealthSlider;

		public UIJoystick m_playerLeftJoystick;

		public UIJoystick m_playerRightJoystick;

		public ButtonStateTracker m_defendButton;

		public UISlider m_defendChargeSlider;

		public UILabel m_timerLabel;

		public ButtonStateTracker m_supporterButton;

		public UISlider m_supporterChargeSlider;

		public Transform m_enemyIndicatorRoot;

		public Transform m_enemyHealthBarRoot;

		public Transform m_doorIndicatorRoot;
	}
}
