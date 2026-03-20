using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorDialogButton : MonoBehaviour
	{
		[SerializeField]
		public IncubatorConfirmDialog ParentPanel;

		[SerializeField]
		public object ObjectModel;

		private void OnClick()
		{
			ParentPanel.m_EventSender = this;
		}
	}
}
