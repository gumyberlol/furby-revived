using UnityEngine;

namespace Relentless
{
	public class GameEventOnNGUIEvent : MonoBehaviour
	{
		public enum NGUIEventType
		{
			Hover = 1,
			Press = 2,
			Click = 4,
			DoubleClick = 8,
			Select = 0x10,
			Drag = 0x20,
			Drop = 0x40,
			Input = 0x80,
			Tooltip = 0x100,
			Scroll = 0x200,
			Key = 0x400,
			Release = 0x800
		}

		[DisplayAsMaskDropdown(typeof(NGUIEventType))]
		public int NGUIEvents = 4;

		public SerialisableEnum GameEvent;

		private void OnHover()
		{
			if ((NGUIEvents & 1) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Hover);
			}
		}

		private void OnPress(bool pressed)
		{
			if (pressed)
			{
				if ((NGUIEvents & 2) != 0)
				{
					base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Press);
				}
			}
			else if ((NGUIEvents & 0x800) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Release);
			}
		}

		private void OnClick()
		{
			if ((NGUIEvents & 4) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Click);
			}
		}

		private void OnDoubleClick()
		{
			if ((NGUIEvents & 8) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.DoubleClick);
			}
		}

		private void OnSelect()
		{
			if ((NGUIEvents & 0x10) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Select);
			}
		}

		private void OnDrag()
		{
			if ((NGUIEvents & 0x20) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Drag);
			}
		}

		private void OnDrop()
		{
			if ((NGUIEvents & 0x40) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Drop);
			}
		}

		private void OnInput()
		{
			if ((NGUIEvents & 0x80) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Input);
			}
		}

		private void OnTooltip()
		{
			if ((NGUIEvents & 0x100) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Tooltip);
			}
		}

		private void OnScroll()
		{
			if ((NGUIEvents & 0x200) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Scroll);
			}
		}

		private void OnKey()
		{
			if ((NGUIEvents & 0x400) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, NGUIEventType.Key);
			}
		}
	}
}
