using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.EggCarton
{
	public class SelectEgg : MonoBehaviour
	{
		private CapsuleCollider m_Collider;

		private void Awake()
		{
			m_Collider = base.gameObject.GetComponentInChildren<CapsuleCollider>();
			AddEntrancingDelegate();
		}

		private void OnDisable()
		{
			RemoveEntrancingDelegate();
		}

		private void AddEntrancingDelegate()
		{
			GameEventRouter.AddDelegateForEnums(OnFinishedEntrancing, CartonGameEvent.EggsFinishedEntrancing);
		}

		private void RemoveEntrancingDelegate()
		{
			GameEventRouter.RemoveDelegateForEnums(OnFinishedEntrancing, CartonGameEvent.EggsFinishedEntrancing);
		}

		private void AddIncubatingDelegate()
		{
			GameEventRouter.AddDelegateForEnums(OnIncubateOrCancel, CartonGameEvent.EggSelectedToIncubate, CartonGameEvent.EggDialogGenericDecline);
		}

		private void RemoveIncubatingDelegate()
		{
			GameEventRouter.RemoveDelegateForEnums(OnIncubateOrCancel, CartonGameEvent.EggSelectedToIncubate, CartonGameEvent.EggDialogGenericDecline);
		}

		private void OnFinishedEntrancing(Enum enumType, GameObject gObj, params object[] parameters)
		{
			CartonGameEvent cartonGameEvent = (CartonGameEvent)(object)enumType;
			if (cartonGameEvent == CartonGameEvent.EggsFinishedEntrancing)
			{
				EnableCollision();
				RemoveEntrancingDelegate();
			}
		}

		private void OnIncubateOrCancel(Enum enumType, GameObject gObj, params object[] parameters)
		{
			switch ((CartonGameEvent)(object)enumType)
			{
			case CartonGameEvent.EggSelectedToIncubate:
				IncubateEgg();
				RemoveIncubatingDelegate();
				break;
			case CartonGameEvent.EggDialogGenericDecline:
				RemoveIncubatingDelegate();
				break;
			}
		}

		public void DisableCollision()
		{
			m_Collider.enabled = false;
		}

		private void EnableCollision()
		{
			m_Collider.enabled = true;
		}

		private void IncubateEgg()
		{
			BabyInstance componentInChildren = GetComponentInChildren<BabyInstance>();
			FurbyGlobals.Player.InProgressFurbyBaby = componentInChildren.GetTargetFurbyBaby();
		}

		public void OnClick()
		{
			AddIncubatingDelegate();
			BabyInstance componentInChildren = GetComponentInChildren<BabyInstance>();
			FurbyBaby targetFurbyBaby = componentInChildren.GetTargetFurbyBaby();
			base.gameObject.SendGameEvent(CartonGameEvent.EggClickedUpon, targetFurbyBaby);
		}
	}
}
