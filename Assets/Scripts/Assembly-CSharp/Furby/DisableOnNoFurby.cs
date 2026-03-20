using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DisableOnNoFurby : MonoBehaviour
	{
		public enum DisableFlags
		{
			GameObject = 1,
			Button = 2
		}

		public enum DisableType
		{
			NoFurbyForAnyReason = 0,
			NoFurbyOnSaveGame = 1,
			HasFurbyButNotAtTheMoment = 2,
			NoFurbyOnSaveGameOrInitial = 3
		}

		[DisplayAsMaskDropdown(typeof(DisableFlags))]
		public DisableFlags WhatToDisable;

		public DisableType DisableRule;

		private void OnEnable()
		{
			switch (DisableRule)
			{
			case DisableType.NoFurbyForAnyReason:
				SetEnabled(!FurbyGlobals.Player.NoFurbyForEitherReason());
				break;
			case DisableType.NoFurbyOnSaveGame:
				SetEnabled(!FurbyGlobals.Player.NoFurbyOnSaveGame());
				break;
			case DisableType.HasFurbyButNotAtTheMoment:
				SetEnabled(!FurbyGlobals.Player.HasFurbyButNotAtTheMoment());
				break;
			}
			GameEventRouter.AddDelegateForType(typeof(PlayerFurbyEvent), OnPlayerFurbyEvent);
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PlayerFurbyEvent), OnPlayerFurbyEvent);
			}
		}

		private void OnDestroy()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PlayerFurbyEvent), OnPlayerFurbyEvent);
			}
		}

		private void SetEnabled(bool buttonEnabled)
		{
			if ((WhatToDisable & DisableFlags.Button) != 0)
			{
				GetComponent<Collider>().enabled = buttonEnabled;
			}
			if ((WhatToDisable & DisableFlags.GameObject) != 0)
			{
				base.gameObject.SetActive(buttonEnabled);
			}
		}

		public void OnPlayerFurbyEvent(Enum eventType, GameObject origin, params object[] parameters)
		{
			switch (DisableRule)
			{
			case DisableType.NoFurbyForAnyReason:
				SetEnabled(!FurbyGlobals.Player.NoFurbyForEitherReason());
				break;
			case DisableType.NoFurbyOnSaveGame:
				SetEnabled(!FurbyGlobals.Player.NoFurbyOnSaveGame());
				break;
			case DisableType.HasFurbyButNotAtTheMoment:
				SetEnabled(!FurbyGlobals.Player.HasFurbyButNotAtTheMoment());
				break;
			case DisableType.NoFurbyOnSaveGameOrInitial:
				SetEnabled(!FurbyGlobals.Player.NoFurbyOnSaveGame() || !Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow);
				break;
			}
		}
	}
}
