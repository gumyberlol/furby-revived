using Relentless;
using UnityEngine;

namespace Furby.Utilities.VoiceChanger
{
	public class VoiceChangerPotion : RelentlessMonoBehaviour
	{
		[SerializeField]
		private FurbyCommand m_PotionCommand;

		[SerializeField]
		private UIAtlas m_GraphicsAtlas;

		[SerializeField]
		private string m_GraphicsName;

		public FurbyCommand GetPotionCommand()
		{
			return m_PotionCommand;
		}

		public UIAtlas GetGraphicsAtlas()
		{
			return m_GraphicsAtlas;
		}

		public string GetGraphicsName()
		{
			return m_GraphicsName;
		}

		public void OnClick()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionItemClicked, base.gameObject, this);
		}
	}
}
