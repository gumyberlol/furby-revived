using Relentless;

namespace Furby
{
	public class ButtonStateTracker : RelentlessMonoBehaviour
	{
		private bool m_pressed;

		private void OnPress(bool isDown)
		{
			m_pressed = isDown;
		}

		public bool IsPressed()
		{
			return m_pressed;
		}
	}
}
