using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomGameData : RelentlessMonoBehaviour
	{
		public InteractionReactions m_InteractionReactions;

		public InteractionIncident GetInteractionFromColliderAndAction(Collider collider, ActionType actionType)
		{
			foreach (InteractionIncident allInteraction in m_InteractionReactions.m_AllInteractions)
			{
				if (allInteraction.m_ActionTypeRequired == actionType && allInteraction.m_Collider == collider)
				{
					return allInteraction;
				}
			}
			return null;
		}

		private void Start()
		{
			InteractionCollisionHandler interactionCollisionHandler = (InteractionCollisionHandler)GetComponent("InteractionCollisionHandler");
			foreach (InteractionIncident allInteraction in m_InteractionReactions.m_AllInteractions)
			{
				if (allInteraction.m_ActionTypeRequired == ActionType.Tickle)
				{
					interactionCollisionHandler.m_Thresholds.Add(new ThresholdTrigger(allInteraction.m_DistanceRequired, allInteraction.m_Collider));
				}
			}
		}
	}
}
