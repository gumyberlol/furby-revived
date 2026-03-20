using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class FurbyPuddleReactions : MonoBehaviour
	{
		public delegate void DropItemHandler(DroppedItem i);

		[SerializeField]
		private DroppedItemList m_droppedItemList;

		[SerializeField]
		private DroppedItemReactionList m_itemReactions;

		public event DropItemHandler ItemDropped;

		public void Start()
		{
			if (m_itemReactions == null)
			{
				throw new ApplicationException("Need m_itemReactions");
			}
		}

		public void DropItem()
		{
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			Logging.Log(string.Format("Furby of type {0} dropping item.", personality));
			DroppedItem item = m_droppedItemList.InstantiateRandom(personality);
			ArriveAndDisperse arriveAndDisperse = item.ArriveAndDisperse;
			arriveAndDisperse.Arrive();
			arriveAndDisperse.ArrivalCompleted += delegate
			{
				ReactToItem(item);
			};
			if (this.ItemDropped != null)
			{
				this.ItemDropped(item);
			}
		}

		public void ReactToItem(DroppedItem item)
		{
			Logging.Log(string.Format("Ooh look, I dropped \"{0}\"", item.gameObject.name));
			FurbyDataChannel instance = Singleton<FurbyDataChannel>.Instance;
			FurbyPersonality personality = instance.FurbyStatus.Personality;
			DroppedItem.ItemType itemType = item.GetItemType();
			FurbyAction actionFor = m_itemReactions.GetActionFor(personality, itemType);
			Logging.Log(string.Format("{0} reacting to {1} with {2}", personality, itemType, actionFor));
			instance.PostAction(actionFor, null);
		}
	}
}
