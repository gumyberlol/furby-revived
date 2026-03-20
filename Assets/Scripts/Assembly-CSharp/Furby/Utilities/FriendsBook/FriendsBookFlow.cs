using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.FriendsBook
{
	public class FriendsBookFlow : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_boughtMessage;

		[SerializeField]
		private float m_messageTime = 3f;

		[SerializeField]
		private EggGiftingDialogBox m_EggConfirmDialogBox;

		private IEnumerator Start()
		{
			m_EggConfirmDialogBox.Hide();
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(FriendsBookEvent.ClickOnFurby, FriendsBookEvent.ClickOnLockedFurby, FriendsBookEvent.ClickOnEmptyFurby));
				int furbyIndex = (int)waiter.ReturnedParameters[0];
				FurbyData furbyData = (FurbyData)waiter.ReturnedParameters[1];
				FriendsBookEvent fbEvent = (FriendsBookEvent)(object)waiter.ReturnedEvent;
				if (fbEvent == FriendsBookEvent.ClickOnFurby)
				{
					yield return StartCoroutine(BuyEgg(furbyIndex, furbyData));
				}
			}
		}

		private IEnumerator BuyEgg(int furbyIndex, FurbyData furbyData)
		{
			int cost = FurbyGlobals.AdultLibrary.EggCost[furbyIndex];
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				base.gameObject.SendGameEvent(FriendsBookEvent.EggCartonFull);
			}
			else if (cost <= FurbyGlobals.Wallet.Balance)
			{
				WaitForGameEvent waiter = new WaitForGameEvent();
				FurbyBabyTypeInfo newBabyType = furbyData.GetNextBabyTypeFromVirtualFurby();
				if (m_EggConfirmDialogBox.gameObject.activeSelf)
				{
					FurbyBaby tempBaby = FurbyBaby.Create(newBabyType.TypeID, null);
					m_EggConfirmDialogBox.InitConfirmGiftEgg(tempBaby);
					while (!m_EggConfirmDialogBox.IsEggReadyToBeRendered())
					{
						yield return null;
					}
					m_EggConfirmDialogBox.SetConfirmGiftState("VIRTUALFRIENDS_CONFIRMPURCHASE", "MENU_OPTION_OK", SharedGuiEvents.DialogAccept, "MENU_OPTION_CANCEL", SharedGuiEvents.DialogCancel);
					m_EggConfirmDialogBox.SetVirtualFurbyLocalisedTextWithSubstitution("VIRTUALFRIENDS_CONFIRMPURCHASE", cost.ToString(), "MENU_OPTION_OK", "MENU_OPTION_CANCEL");
					m_EggConfirmDialogBox.Show(true);
				}
				else
				{
					base.gameObject.SendGameEvent(SharedGuiEvents.DialogShow, "Confirm", cost);
				}
				yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
				m_EggConfirmDialogBox.Hide();
				if (!waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
				{
					base.gameObject.SendGameEvent(FriendsBookEvent.EggBought);
					FurbyGlobals.Wallet.Balance -= cost;
					if (FurbyGlobals.Player.FlowStage == FlowStage.Friendsbook)
					{
						FurbyGlobals.Player.FlowStage = FlowStage.EggCarton_HatchSecond;
						Singleton<GameDataStoreObject>.Instance.Save();
					}
					if (newBabyType != null)
					{
						FurbyBaby baby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(newBabyType.TypeID);
						baby.m_persistantData.cameFromFriendsBook = true;
						Singleton<GameDataStoreObject>.Instance.Save();
						GameEventRouter.SendEvent(BabyLifecycleEvent.FromVirtualFurby, null, baby);
						GameEventRouter.SendEvent(XpAwardEvent.PurchaseEgg);
						Singleton<GameDataStoreObject>.Instance.Save();
						yield return new WaitForSeconds(0.5f);
						base.gameObject.SendGameEvent(SharedGuiEvents.DialogShow, "Congratulations");
						yield return new WaitForSeconds(m_messageTime);
						base.gameObject.SendGameEvent(SharedGuiEvents.DialogHide, "Congratulations");
						FurbyGlobals.ScreenSwitcher.SwitchScreen("EggCarton");
					}
				}
			}
			else
			{
				base.gameObject.SendGameEvent(FriendsBookEvent.NotEnoughMoney);
			}
		}
	}
}
