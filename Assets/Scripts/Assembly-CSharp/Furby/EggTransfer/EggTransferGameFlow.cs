using System;
using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby.EggTransfer
{
	public class EggTransferGameFlow : Singleton<EggTransferGameFlow>
	{
		public FurbyCommandCoroutine m_CommandCoroutine = new FurbyCommandCoroutine();

		public BabyInstance m_BabyInstance;

		public Animation m_LayingFx;

		public GameObject m_congratulationBanner;

		public GameObject m_CartonButton;

		[SerializeField]
		private float m_furbySearchTimeout = 20f;

		private bool m_lookingForFriend;

		private FurbyStatus? m_foundStatus;

		private float m_waitRetryTimer;

		[SerializeField]
		private long m_secondsUntilNewFriendTransfer = 18000L;

		[SerializeField]
		private UIPanel m_backButtonPanel;

		[SerializeField]
		private DialogPanel m_lookingForFriendAdviceDialog;

		public void Start()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
			GameEventRouter.AddDelegateForType(typeof(FurbyDataEvent), RecieveDataEvent);
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				base.gameObject.SendGameEvent(EggTransferGameEvent.EggCartonFull);
				return;
			}
			EggTransferSettings eggTransferSettings = (EggTransferSettings)UnityEngine.Object.FindObjectOfType(typeof(EggTransferSettings));
			if (eggTransferSettings != null)
			{
				m_lookingForFriend = eggTransferSettings.m_transferringFriendsEgg;
				UnityEngine.Object.Destroy(eggTransferSettings.gameObject);
			}
			if (m_lookingForFriend)
			{
				GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferLookingForFriendEgg);
			}
			else
			{
				if (FurbyGlobals.Player.NumEggsAvailable == 0)
				{
					base.gameObject.SendGameEvent(EggTransferGameEvent.EggTransferNoEggsReady);
					return;
				}
				GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferLookingForMyFurbyEgg);
			}
			StartCoroutine(Execute());
		}

		public new void OnDestroy()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = FurbyGlobals.Player.NoFurbyOnSaveGame();
			GameEventRouter.RemoveDelegateForType(typeof(FurbyDataEvent), RecieveDataEvent);
			StopAllCoroutines();
			base.OnDestroy();
		}

		private IEnumerator Execute()
		{
			AdultFurbyLibrary furbyLibrary = FurbyGlobals.AdultLibrary;
			BabyRepositoryHelpers babyRepository = FurbyGlobals.BabyRepositoryHelpers;
			FurbyBaby furbyBaby = null;
			FurbyData furbyAdult = null;
			if (m_lookingForFriend)
			{
				if (DateTime.UtcNow.Ticks < Singleton<GameDataStoreObject>.Instance.Data.m_timeNextFriendEggTransferAllowed)
				{
					GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferFriendEggSentTooRecently);
					yield break;
				}
				m_backButtonPanel.enabled = false;
				m_lookingForFriendAdviceDialog.SetEnabled(true);
				WaitForGameEvent waiter = new WaitForGameEvent();
				yield return StartCoroutine(waiter.WaitForEvent(EggTransferGameEvent.EggTransferLookingForFriendAdviceClosed));
				m_lookingForFriendAdviceDialog.SetEnabled(false);
				m_backButtonPanel.enabled = true;
			}
			m_foundStatus = null;
			float timeoutTime = m_furbySearchTimeout;
			while (!m_foundStatus.HasValue)
			{
				if (m_waitRetryTimer < 0f)
				{
					Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
					Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
					Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Status);
					m_waitRetryTimer = 8f;
				}
				m_waitRetryTimer -= Time.deltaTime;
				timeoutTime -= Time.deltaTime;
				if (timeoutTime < 0f)
				{
					yield return StartCoroutine(NoFurbyDialogState());
					timeoutTime = m_furbySearchTimeout;
				}
				yield return null;
			}
			Logging.Log(string.Concat("#### ", m_foundStatus.Value.Pattern, " ", m_lookingForFriend, " ", FurbyGlobals.Player.IsPlayersFurby(m_foundStatus.Value)));
			if (FurbyGlobals.Player.IsPlayersFurby(m_foundStatus.Value) == m_lookingForFriend)
			{
				string furbyName = FurbyGlobals.Player.FullName;
				if (m_lookingForFriend)
				{
					GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferIncorrectFurbyInFriendMode, null, furbyName, furbyName);
				}
				else
				{
					GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferIncorrectFurbyInMyFurbyMode, null, furbyName, furbyName);
				}
				if (FurbyGlobals.Player.FlowStage != FlowStage.Normal)
				{
					FurbyGlobals.Player.FlowStage = FlowStage.Dashboard_SelectEgg;
				}
				yield break;
			}
			UnityEngine.Object.Destroy(m_backButtonPanel.gameObject);
			while (!m_CommandCoroutine.ReplyResult)
			{
				GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferStartSync, base.gameObject);
				yield return this.ConnectAndWaitOnReply(FurbyCommand.Application, 8f);
				GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferFinishSync, base.gameObject);
				if (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
				{
					yield return StartCoroutine(NoFurbyDialogState());
					continue;
				}
				yield return m_CommandCoroutine.PostAwaitReply(this, FurbyCommand.EggTransfer, 3);
				yield return this.HeartBeatAndWaitOnSend();
				if (!m_CommandCoroutine.ReplyResult)
				{
					yield return StartCoroutine(NoFurbyDialogState());
					continue;
				}
				yield return m_CommandCoroutine.PostAwaitReply(this, FurbyCommand.Nesting, 3);
				yield return this.HeartBeatAndWaitOnSend();
				if (!m_CommandCoroutine.ReplyResult)
				{
					yield return StartCoroutine(NoFurbyDialogState());
				}
			}
			AdultFurbyType aft = AdultFurbyLibrary.GetAdultFurbyTypeFromToy();
			bool adultFurbyIsCrystal = false;
			switch (aft)
			{
			case AdultFurbyType.CrystalPinkToPurple:
			case AdultFurbyType.CrystalYellowToOrange:
			case AdultFurbyType.CrystalGreenToBlue:
			case AdultFurbyType.CrystalPinkToBlue:
			case AdultFurbyType.CrystalOrangeToPink:
			case AdultFurbyType.CrystalRainbow:
				adultFurbyIsCrystal = true;
				break;
			}
			if (adultFurbyIsCrystal && !Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
			{
				Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
				GameEventRouter.SendEvent(CrystalUnlockTelemetryEvents.CrystalUnlocked_ReceivedEggFromFriendsFurbyToy);
			}
			furbyAdult = furbyLibrary.GetAdultFurby(aft);
			FurbyBabyTypeInfo furbyBabyTypeInfo = furbyAdult.GetNextBabyTypeFromToyFurby();
			if (furbyBabyTypeInfo == null)
			{
				babyRepository.TEST_MakeBabies(0);
			}
			FurbyBabyTypeID nextFurblingType = furbyBabyTypeInfo.TypeID;
			furbyBaby = babyRepository.CreateNewBaby(nextFurblingType, true);
			GameEventRouter.SendEvent(m_lookingForFriend ? BabyLifecycleEvent.FromFriendsFurbyToy : BabyLifecycleEvent.FromOwnFurbyToy, null, furbyBaby);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			m_BabyInstance.SetTargetFurbyBaby(furbyBaby);
			if (!m_lookingForFriend)
			{
				FurbyGlobals.Player.NumEggsAvailable = 0;
			}
			GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferStartTransfer, base.gameObject);
			m_BabyInstance.InstantiateObject();
			m_LayingFx.Play();
			yield return new WaitForSeconds(6.5f);
			yield return this.HeartBeatAndWaitOnSend();
			if (m_lookingForFriend)
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_timeNextFriendEggTransferAllowed = DateTime.UtcNow.AddSeconds(m_secondsUntilNewFriendTransfer).Ticks;
			}
			yield return new WaitForSeconds(3.9f);
			GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferFinishTransfer, base.gameObject);
			m_BabyInstance.Show();
			yield return new WaitForSeconds(2f);
			GameEventRouter.SendEvent(XpAwardEvent.LayEgg);
			m_congratulationBanner.SetActive(true);
			yield return new WaitForSeconds(3f);
			m_CartonButton.SetActive(true);
			if (!Singleton<GameDataStoreObject>.Instance.Data.MetFurbies.Contains(furbyAdult.AdultType))
			{
				Singleton<GameDataStoreObject>.Instance.Data.MetFurbies.Add(furbyAdult.AdultType);
			}
		}

		private IEnumerator NoFurbyDialogState()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			GameEventRouter.SendEvent(EggTransferGameEvent.EggTransferNoFurbyFound);
			yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
			if (waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
			{
				if (Singleton<GameDataStoreObject>.Instance.Data.FlowStage != FlowStage.Normal)
				{
					bool rememberBack = false;
					string destination = "SaveSlotSelect";
					FurbyGlobals.ScreenSwitcher.SwitchScreen(destination, rememberBack);
				}
				else
				{
					FurbyGlobals.ScreenSwitcher.BackScreen();
				}
			}
		}

		private void RecieveDataEvent(Enum eventType, GameObject gObj, params object[] parameters)
		{
			FurbyDataEvent furbyDataEvent = (FurbyDataEvent)(object)eventType;
			if (furbyDataEvent == FurbyDataEvent.FurbyDataReceived)
			{
				m_foundStatus = (FurbyStatus)parameters[0];
			}
		}

		private void OnDebugReply()
		{
			m_CommandCoroutine.ForceReply();
		}
	}
}
