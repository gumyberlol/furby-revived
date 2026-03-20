using UnityEngine;

namespace Relentless
{
	public class GameObjectSendEvent : MonoBehaviour
	{
		public enum GameObjectEventType
		{
			Enable = 1,
			Start = 2,
			Disable = 4,
			Destroy = 8
		}

		[DisplayAsMaskDropdown(typeof(GameObjectEventType))]
		public int GameObjectEvents;

		public SerialisableEnum GameEvent;

		private void OnEnable()
		{
			if ((GameObjectEvents & 1) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, GameObjectEventType.Enable);
			}
		}

		private void Start()
		{
			if ((GameObjectEvents & 2) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, GameObjectEventType.Start);
			}
		}

		private void OnDisable()
		{
			if ((GameObjectEvents & 4) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, GameObjectEventType.Disable);
			}
		}

		private void OnDestroy()
		{
			if ((GameObjectEvents & 8) != 0)
			{
				base.gameObject.SendGameEvent(GameEvent.Value, GameObjectEventType.Destroy);
			}
		}
	}
}
