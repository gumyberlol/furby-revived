using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomHintController : MonoBehaviour
	{
		[SerializeField]
		private HintState m_SelectItemTop = new HintState();

		[SerializeField]
		private HintState m_SelectItemBot = new HintState();

		[SerializeField]
		private HintState m_SelectArea = new HintState();

		[SerializeField]
		private HintState m_ConfirmChanges = new HintState();

		[SerializeField]
		private HintState m_ScrollTop = new HintState();

		[SerializeField]
		private HintState m_ScrollBot = new HintState();

		public HintState SelectItemTop
		{
			get
			{
				return m_SelectItemTop;
			}
		}

		public HintState SelectItemBot
		{
			get
			{
				return m_SelectItemBot;
			}
		}

		public HintState SelectArea
		{
			get
			{
				return m_SelectArea;
			}
		}

		public HintState ConfirmChanges
		{
			get
			{
				return m_ConfirmChanges;
			}
		}

		public HintState ScrollTop
		{
			get
			{
				return m_ScrollTop;
			}
		}

		public HintState ScrollBot
		{
			get
			{
				return m_ScrollBot;
			}
		}

		public void DisableAll()
		{
			SelectItemTop.Disable();
			SelectItemBot.Disable();
			SelectArea.Disable();
			ConfirmChanges.Disable();
			ScrollTop.Disable();
			ScrollBot.Disable();
		}

		public void Update()
		{
			if (SelectArea.IsEnabled())
			{
				SelectArea.TestAndBroadcastState();
			}
			else if (ScrollBot.IsEnabled() || ScrollTop.IsEnabled())
			{
				ScrollBot.TestAndBroadcastState();
				ScrollTop.TestAndBroadcastState();
			}
			else if (SelectItemTop.IsEnabled() || SelectItemBot.IsEnabled())
			{
				SelectItemTop.TestAndBroadcastState();
				SelectItemBot.TestAndBroadcastState();
			}
			else
			{
				ConfirmChanges.TestAndBroadcastState();
			}
		}
	}
}
