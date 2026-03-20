using Relentless;

namespace Furby.Utilities.VoiceChanger
{
	public class VoiceChangerResetButton : RelentlessMonoBehaviour
	{
		public delegate void VoiceChangerResetButtonDelegate();

		public event VoiceChangerResetButtonDelegate OnReset;

		private void OnClick()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.ResetButtonClicked, null, null);
			if (this.OnReset != null)
			{
				this.OnReset();
			}
		}
	}
}
