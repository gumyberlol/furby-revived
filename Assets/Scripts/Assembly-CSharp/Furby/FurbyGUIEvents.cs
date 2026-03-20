using UnityEngine;

namespace Furby
{
	public class FurbyGUIEvents : MonoBehaviour
	{
		private FurbyGUIController target;

		public void UpdateTarget(FurbyGUIController newTarget)
		{
			target = newTarget;
		}

		public void NamedTextEvent(string message)
		{
			target.SendMessage(message);
		}
	}
}
