using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Furby.Incubator;
using Furby.Playroom;
using Furby.SaveSlotSelect;
using Furby.Scanner;
using Furby.Utilities.Blender;
using Furby.Utilities.Salon;
using Furby.Utilities.Salon2;
using Relentless;
using Relentless.Core.Crypto;
using Relentless.Network.Analytics;
using UnityEngine;

namespace Furby
{
	public class FurbyTelemetry : MonoBehaviour
	{
		private enum ScanType
		{
			FirstTime = 0,
			Dashboard = 1
		}

		private enum ScanResult
		{
			NoResult = 0,
			Success = 1,
			IncorrectFurby = 2,
			NoFurbyFound = 3,
			Furby1Detected = 4
		}

		private class ScanInformation
		{
			public ScanType ScanType;

			public int NumAttempts;

			public ScanResult Result;

			public bool? NamingWasRequired;

			public bool? PlayedWithFurby;

			public FurbyStatus? FurbyStatus;
		}

		[SerializeField]
		private string[] m_scenesToExclude;

		[SerializeField]
		private string[] m_babyScenes;

		[SerializeField]
		private string[] m_adultScenes;

		private DateTime? m_sceneStart;

		private TelemetryParams m_sceneTelemetry;

		private DateTime m_sessionStart;

		private bool hasShutdown;

		private ScanInformation m_lastScan;

		private bool m_successWillBeValid = true;

		private Dictionary<AdultFurbyType, string> m_patternNames = new Dictionary<AdultFurbyType, string>
		{
			{
				AdultFurbyType.Triangles,
				"Triangles"
			},
			{
				AdultFurbyType.Stripes,
				"Stripes"
			},
			{
				AdultFurbyType.Peacock,
				"Peacock"
			},
			{
				AdultFurbyType.Checkerboard,
				"Polkadot"
			},
			{
				AdultFurbyType.Cubes,
				"Cubes"
			},
			{
				AdultFurbyType.Diamonds,
				"Teal"
			},
			{
				AdultFurbyType.Zigzags,
				"Chevron"
			},
			{
				AdultFurbyType.Hearts,
				"Hearts"
			},
			{
				AdultFurbyType.Waves,
				"Waves"
			},
			{
				AdultFurbyType.Christmas,
				"Christmas"
			},
			{
				AdultFurbyType.Diagonals,
				"Diagonals"
			},
			{
				AdultFurbyType.Lightning,
				"Bolt"
			},
			{
				AdultFurbyType.NoFurby,
				"NoFurby"
			},
			{
				AdultFurbyType.PROMO,
				"Promo"
			},
			{
				AdultFurbyType.Unknown,
				"Currently unscanned"
			}
		};

		public KeyValuePair<int, string>[] m_timeBuckets = new KeyValuePair<int, string>[40]
		{
			new KeyValuePair<int, string>(5, "0 - 5 seconds"),
			new KeyValuePair<int, string>(10, "6 - 10 seconds"),
			new KeyValuePair<int, string>(20, "11 - 20 seconds"),
			new KeyValuePair<int, string>(30, "21 - 30 seconds"),
			new KeyValuePair<int, string>(45, "31 - 45 seconds"),
			new KeyValuePair<int, string>(60, "46 - 60 seconds"),
			new KeyValuePair<int, string>(90, "61 - 90 seconds"),
			new KeyValuePair<int, string>(120, "1.5 - 2 minutes"),
			new KeyValuePair<int, string>(180, "2 - 3 minutes"),
			new KeyValuePair<int, string>(240, "3 - 4 minutes"),
			new KeyValuePair<int, string>(300, "4 - 5 minutes"),
			new KeyValuePair<int, string>(360, "5 - 6 minutes"),
			new KeyValuePair<int, string>(420, "6 - 7 minutes"),
			new KeyValuePair<int, string>(480, "7 - 8 minutes"),
			new KeyValuePair<int, string>(540, "8 - 9 minutes"),
			new KeyValuePair<int, string>(600, "9 - 10 minutes"),
			new KeyValuePair<int, string>(900, "11 - 15 minutes"),
			new KeyValuePair<int, string>(1200, "16 - 20 minutes"),
			new KeyValuePair<int, string>(1800, "21 - 30 minutes"),
			new KeyValuePair<int, string>(2700, "31 - 45 minutes"),
			new KeyValuePair<int, string>(3600, "46 - 60 minutes"),
			new KeyValuePair<int, string>(5400, "61 - 90 minutes"),
			new KeyValuePair<int, string>(7200, "1.5 - 2 hours"),
			new KeyValuePair<int, string>(14400, "2 - 4 hours"),
			new KeyValuePair<int, string>(28800, "4 - 8 hours"),
			new KeyValuePair<int, string>(43200, "8 - 12 hours"),
			new KeyValuePair<int, string>(86400, "0.5 - 1 day"),
			new KeyValuePair<int, string>(172800, "1 - 2 days"),
			new KeyValuePair<int, string>(345600, "2 - 4 days"),
			new KeyValuePair<int, string>(604800, "4 - 7 days"),
			new KeyValuePair<int, string>(1209600, "1 - 2 weeks"),
			new KeyValuePair<int, string>(2628000, "2 weeks - 1 month"),
			new KeyValuePair<int, string>(5256000, "1 - 2 months"),
			new KeyValuePair<int, string>(10512000, "2 - 4 months"),
			new KeyValuePair<int, string>(15768000, "4 - 6 months"),
			new KeyValuePair<int, string>(31557600, "6 - 12 months"),
			new KeyValuePair<int, string>(47336400, "12 - 18 months"),
			new KeyValuePair<int, string>(63115200, "1.5 - 2 years"),
			new KeyValuePair<int, string>(94672800, "2 - 3 years"),
			new KeyValuePair<int, string>(126230400, "3 - 4 years")
		};

		private GameEventSubscription m_DebugPanelSub;

		private IEnumerator Start()
		{
			if (m_sceneTelemetry == null)
			{
				m_sceneTelemetry = new TelemetryParams();
			}
			yield return null;
			m_sessionStart = DateTime.Now;
			SendTimeSinceLastSession();
			GameEventRouter.AddDelegateForEnums(SaveSlotSelectEvent, SaveSlotSelectGameEvent.SelectSlot0, SaveSlotSelectGameEvent.SelectSlot1, SaveSlotSelectGameEvent.SelectSlot2);
			GameEventRouter.AddDelegateForEnums(SaveSlotSelectEvent, SaveSlotSelectGameEvent.ConfirmDelete);
			GameEventRouter.AddDelegateForEnums(SendSceneActivity, SpsEvent.EnterScreen);
			GameEventRouter.AddDelegateForType(typeof(ScannerEvents), ScannerEvents);
			GameEventRouter.AddDelegateForType(typeof(FurbyDataEvent), OnFurbyDataEvent);
			GameEventRouter.AddDelegateForType(typeof(BabyLifecycleEvent), OnBabyLifecycleEvent);
			GameEventRouter.AddDelegateForType(typeof(VirtualItemPurchase), OnVirtualItemPurchase);
			GameEventRouter.AddDelegateForType(typeof(FurbyModeChoice), OnModeChoice);
			GameEventRouter.AddDelegateForEnums(OnConversion, FurbyModeChoice.DashboardArrival_Conversion);
			GameEventRouter.AddDelegateForType(typeof(LegacyFurbyEvents), OnProductLinkPage);
			GameEventRouter.AddDelegateForType(typeof(IAPPurchaseEvents), OnIAPPurchaseEvent);
			GameEventRouter.AddDelegateForType(typeof(IAPReceiptValidationEvents), OnIAPReceiptValidationEvent);
			GameEventRouter.AddDelegateForType(typeof(IncubatorPurchaseDialog.ConsumableTelemetry), OnConsumablePurchaseAttempt);
			GameEventRouter.AddDelegateForType(typeof(GeoCodeEvents), OnGeoCodeEvent);
			GameEventRouter.AddDelegateForType(typeof(InAppShopPurchaseState), OnNonConsumablePurchaseResult);
			GameEventRouter.AddDelegateForType(typeof(CrystalUnlockTelemetryEvents), OnCrystalUnlockEvent);
			GameEventRouter.AddDelegateForType(typeof(IAPWarningEvent), OnIAPWarningNavigation);
			foreach (object _t in Enum.GetValues(typeof(AdultFurbyType)))
			{
				AdultFurbyType t = (AdultFurbyType)(int)_t;
				if (!m_patternNames.ContainsKey(t))
				{
					m_patternNames.Add(t, t.ToString());
				}
			}
		}

		private void OnApplicationQuit()
		{
			OnSessionEnd();
		}

		private void OnDestroy()
		{
			OnSessionEnd();
			m_DebugPanelSub.Dispose();
		}

		private void OnSessionEnd()
		{
			if (!hasShutdown)
			{
				hasShutdown = true;
				TelemetryParams telemetryParams = new TelemetryParams();
				telemetryParams.Add("Session Length", GetBucketedTimePeriod(m_sessionStart, DateTime.Now));
				SingletonInstance<TelemetryManager>.Instance.LogEvent("SessionLength", telemetryParams, false);
			}
		}

		public void SendTimeSinceLastSession()
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.TimeOfLastSave == 0L)
			{
				telemetryParams.Add("Time", "First time on this device");
			}
			else
			{
				telemetryParams.Add("Time", GetBucketedTimePeriod(Singleton<GameDataStoreObject>.Instance.GlobalData.TimeOfLastSave, DateTime.Now.Ticks));
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("TimeSinceLastPlay", telemetryParams, false);
		}

		public void OnModeChoice(Enum evt, GameObject gObj, object[] parameters)
		{
			FurbyModeChoice furbyModeChoice = (FurbyModeChoice)(object)evt;
			string text = string.Empty;
			switch (furbyModeChoice)
			{
			case FurbyModeChoice.DashboardArrival_Furby:
				text = "Play With Furby";
				break;
			case FurbyModeChoice.DashboardArrival_NoFurby:
				text = "Play With No Furby";
				break;
			}
			if (text.Length == 0)
			{
				return;
			}
			TelemetryParams telemetryParams = new TelemetryParams();
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			telemetryParams.Add("Mode", text);
			string value = Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale.ToString();
			telemetryParams.Add("Language", value);
			string value2;
			string value3;
			string value4;
			if (!data.NoFurbyMode)
			{
				int stepPower = 2;
				value2 = BucketIntLog(data.m_numFurbySessions, stepPower);
				value3 = BucketIntLog(data.m_numNoFurbySessions, stepPower);
				value4 = data.m_numSessionsIsCountingFromStart.ToString();
			}
			else
			{
				value2 = (value3 = "N/A");
				value4 = "N/A";
			}
			telemetryParams.Add("Sessions with toy", value2);
			telemetryParams.Add("Sessions without toy", value3);
			if (data.m_numFurbySessions == 1 && data.m_numNoFurbySessions > 0)
			{
				bool flag = furbyModeChoice == FurbyModeChoice.DashboardArrival_Furby;
				bool flag2 = !data.m_HaveSentConversionTelemetry;
				if (flag && flag2)
				{
					TelemetryParams telemetryParams2 = new TelemetryParams();
					telemetryParams2.Add("Language", Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale.ToString());
					telemetryParams2.Add("Country", Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode);
					telemetryParams2.Add("Sessions before conversion", value3);
					foreach (GameData.IAPBundleStats bundleStat in data.m_BundleStats)
					{
						telemetryParams2.Add("Number of " + bundleStat.m_BundleID, bundleStat.m_PurchaseCount.ToString());
					}
					int num = 0;
					bool flag3 = false;
					foreach (GameData.IAPBundleStats bundleStat2 in data.m_BundleStats)
					{
						if (bundleStat2.m_BundleID.Equals("FurbyIAPBundle4"))
						{
							flag3 = true;
						}
						else
						{
							num += bundleStat2.m_PurchaseCount;
						}
					}
					telemetryParams2.Add("IAP, Consumables", BucketIntLog(num, 2));
					telemetryParams2.Add("IAP, Crystal Re-Skin", (!flag3) ? "Not Purchased Crystal" : "Have Purchased Crystal");
					SingletonInstance<TelemetryManager>.Instance.LogEvent("ConfirmedConversion", telemetryParams2, false);
					data.m_HaveSentConversionTelemetry = true;
				}
			}
			else
			{
				telemetryParams.Add("Session counts are true", value4);
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("ModeChoice", telemetryParams, false);
			if (SingletonInstance<GameConfiguration>.Instance.IsIAPAvailable())
			{
				telemetryParams.Add("Country", Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode);
				SingletonInstance<TelemetryManager>.Instance.LogEvent("ModeChoiceWithIAP", telemetryParams, false);
			}
		}

		private void BuildAndSendPlayerSplits()
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (AdultFurbyType metFurby in Singleton<GameDataStoreObject>.Instance.Data.MetFurbies)
			{
				switch (metFurby)
				{
				case AdultFurbyType.Checkerboard:
				case AdultFurbyType.Christmas:
				case AdultFurbyType.Cubes:
				case AdultFurbyType.Diagonals:
				case AdultFurbyType.Diamonds:
				case AdultFurbyType.Hearts:
				case AdultFurbyType.Lightning:
				case AdultFurbyType.Peacock:
				case AdultFurbyType.Stripes:
				case AdultFurbyType.Triangles:
				case AdultFurbyType.Waves:
				case AdultFurbyType.Zigzags:
					flag = true;
					break;
				case AdultFurbyType.SpringHearts:
				case AdultFurbyType.SpringDiamond:
				case AdultFurbyType.SpringStar:
				case AdultFurbyType.SpringRainbows:
				case AdultFurbyType.SpringHoundstooth:
				case AdultFurbyType.SpringZigZag:
					flag2 = true;
					break;
				case AdultFurbyType.CrystalPinkToPurple:
				case AdultFurbyType.CrystalYellowToOrange:
				case AdultFurbyType.CrystalGreenToBlue:
				case AdultFurbyType.CrystalPinkToBlue:
				case AdultFurbyType.CrystalOrangeToPink:
				case AdultFurbyType.CrystalRainbow:
					flag3 = true;
					break;
				}
			}
			bool flag4 = flag && flag2 && flag3;
			telemetryParams.Add("Toy Scan (Original)", (!flag) ? "Never Scanned Original" : "Have Scanned Original");
			telemetryParams.Add("Toy Scan (Spring)", (!flag2) ? "Never Scanned Spring" : "Have Scanned Spring");
			telemetryParams.Add("Toy Scan (Crystal)", (!flag3) ? "Never Scanned Crystal" : "Have Scanned Crystal");
			telemetryParams.Add("Toy Scan (All)", (!flag4) ? "Not Scanned All" : "Have Scanned All");
			string value = "Never Scanned Anything";
			if (flag && flag2 && flag3)
			{
				value = "Everything";
			}
			if (flag && !flag2 && !flag3)
			{
				value = "Original Only";
			}
			if (flag && flag2 && !flag3)
			{
				value = "Original and Spring";
			}
			if (flag && !flag2 && flag3)
			{
				value = "Original and Crystal";
			}
			if (!flag && flag2 && !flag3)
			{
				value = "Spring Only";
			}
			if (!flag && !flag2 && flag3)
			{
				value = "Crystal Only";
			}
			if (!flag && flag2 && flag3)
			{
				value = "Spring and Crystal";
			}
			telemetryParams.Add("Toy Scan (Overall)", value);
			int num = 0;
			bool flag5 = false;
			foreach (GameData.IAPBundleStats bundleStat in data.m_BundleStats)
			{
				if (bundleStat.m_BundleID.Equals("FurbyIAPBundle4"))
				{
					flag5 = true;
				}
				else
				{
					num += bundleStat.m_PurchaseCount;
				}
			}
			string value2 = num.ToString();
			if (num > 0)
			{
				value2 = BucketIntLog(num, 2);
			}
			telemetryParams.Add("Num FastForwards Purchased", value2);
			telemetryParams.Add("Crystal Re-Skin IAP", (!flag5) ? "Not Purchased Crystal" : "Have Purchased Crystal");
			SingletonInstance<TelemetryManager>.Instance.LogEvent("PlayerSplits", telemetryParams, false);
		}

		private void OnConversion(Enum evt, GameObject gObj, object[] parameters)
		{
			GameData gameData = parameters[0] as GameData;
			int numNoFurbySessions = gameData.m_numNoFurbySessions;
			TelemetryParams telemetryParams = new TelemetryParams();
			telemetryParams.Add("Sessions without toy", numNoFurbySessions.ToString());
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Conversion", telemetryParams, false);
		}

		public void OnProductLinkPage(Enum evt, GameObject gObj, object[] parameters)
		{
			LegacyFurbyEvents legacyFurbyEvents = (LegacyFurbyEvents)(object)evt;
			TelemetryParams telemetryParams = new TelemetryParams();
			switch (legacyFurbyEvents)
			{
			case LegacyFurbyEvents.ClickedBuyFurby:
				telemetryParams.Add("Clicked", "Buy Furby Boom");
				break;
			case LegacyFurbyEvents.ClickedDownloadLegacyApp:
				telemetryParams.Add("Clicked", "Download Legacy App");
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("ProductPage", telemetryParams, false);
		}

		public void OnConsumablePurchaseAttempt(Enum evt, GameObject gObj, object[] parameters)
		{
			string text = (string)parameters[0];
			IncubatorPurchaseDialog.ConsumableTelemetry consumableTelemetry = (IncubatorPurchaseDialog.ConsumableTelemetry)(object)evt;
			TelemetryParams telemetryParams = new TelemetryParams();
			switch (consumableTelemetry)
			{
			case IncubatorPurchaseDialog.ConsumableTelemetry.ConsumableNotInterestedDeclined:
				telemetryParams.Add("Result", "Declined");
				break;
			case IncubatorPurchaseDialog.ConsumableTelemetry.ConsumableInterestedButDeclined:
				telemetryParams.Add("Result", "Cancelled");
				telemetryParams.Add(text, "Cancelled");
				break;
			case IncubatorPurchaseDialog.ConsumableTelemetry.ConsumableTakenAndSucceeded:
				telemetryParams.Add("Result", "Purchased");
				telemetryParams.Add(text, "Purchased");
				telemetryParams.Add("Purchased Item", text);
				break;
			case IncubatorPurchaseDialog.ConsumableTelemetry.ConsumableTakenButFailed:
				telemetryParams.Add("Result", "Failed");
				telemetryParams.Add(text, "Failed");
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("ConsumablePurchase", telemetryParams, false);
		}

		public void OnIAPPurchaseEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			string text = (string)parameters[0];
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			GlobalGameData globalData = Singleton<GameDataStoreObject>.Instance.GlobalData;
			IAPPurchaseEvents iAPPurchaseEvents = (IAPPurchaseEvents)(object)evt;
			if (iAPPurchaseEvents == IAPPurchaseEvents.PurchaseComplete)
			{
				TelemetryParams telemetryParams = new TelemetryParams();
				telemetryParams.Add("Purchased Item", text);
				telemetryParams.Add("Number Of EggsHatched", GetGenericBucketedInt(data.IncubatorEggsHatched));
				telemetryParams.Add("Time spent playing", GetBucketedTimePeriod(new TimeSpan(data.TimeSpentPlaying)));
				telemetryParams.Add("Number of Sessions", BucketIntLog(data.m_numFurbySessions + data.m_numNoFurbySessions, 2));
				int stepPower = 2;
				string value = BucketIntLog(data.m_numFurbySessions, stepPower);
				telemetryParams.Add("Sessions with toy", value);
				string value2 = BucketIntLog(data.m_numNoFurbySessions, stepPower);
				telemetryParams.Add("Sessions without toy", value2);
				telemetryParams.Add("Furby Pattern", m_patternNames[FurbyGlobals.Player.Furby.AdultType]);
				foreach (GameData.IAPBundleStats bundleStat in data.m_BundleStats)
				{
					telemetryParams.Add("Number of " + bundleStat.m_BundleID, bundleStat.m_PurchaseCount.ToString());
				}
				telemetryParams.Add("Country", globalData.CountryCode);
				SingletonInstance<TelemetryManager>.Instance.LogEvent("IAP Purchase", telemetryParams, false);
			}
			TelemetryParams telemetryParams2 = new TelemetryParams();
			switch (iAPPurchaseEvents)
			{
			case IAPPurchaseEvents.PurchaseComplete:
				telemetryParams2.Add(text, "Success");
				telemetryParams2.Add("All", "Success");
				break;
			case IAPPurchaseEvents.PurchaseCancelled:
				telemetryParams2.Add(text, "Cancelled");
				telemetryParams2.Add("All", "Cancelled");
				break;
			case IAPPurchaseEvents.PurchaseFailed:
				telemetryParams2.Add(text, "Failed");
				telemetryParams2.Add("All", "Failed");
				break;
			}
			telemetryParams2.Add("Country", globalData.CountryCode);
			telemetryParams2.Add("Language", Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale.ToString());
			SingletonInstance<TelemetryManager>.Instance.LogEvent("All IAP Attempts", telemetryParams2, false);
		}

		public void OnIAPReceiptValidationEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			string text = (string)parameters[0];
			IAPReceiptValidationEvents iAPReceiptValidationEvents = (IAPReceiptValidationEvents)(object)evt;
			TelemetryParams telemetryParams = new TelemetryParams();
			switch (iAPReceiptValidationEvents)
			{
			case IAPReceiptValidationEvents.ReceiptValidationSuccess:
				telemetryParams.Add(text + " Receipt Validation", "Success");
				telemetryParams.Add("Success", text);
				break;
			case IAPReceiptValidationEvents.ReceiptValidationFailed:
				telemetryParams.Add(text + " Receipt Validation", "Failed");
				telemetryParams.Add("Failed", text);
				break;
			case IAPReceiptValidationEvents.ReceiptValidationUnresolved:
				telemetryParams.Add(text + " Receipt Validation", "Couldn't Resolve");
				telemetryParams.Add("Unresolved", text);
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("IAP Receipt Validation", telemetryParams, false);
		}

		public void SaveSlotSelectEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			string text = evt.ToString();
			telemetryParams.Add("Slot", text[text.Length - 1].ToString());
			if (Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastStatUpdate == 0L)
			{
				telemetryParams.Add("TimeSinceLastPlay", "First time on this save");
			}
			else
			{
				telemetryParams.Add("TimeSinceLastPlay", GetBucketedTimePeriod(Singleton<GameDataStoreObject>.Instance.Data.TimeOfLastSave, DateTime.Now.Ticks));
			}
			int num = 0;
			for (int i = 0; i < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); i++)
			{
				if (Singleton<GameDataStoreObject>.Instance.GetSlot(i).HasCompletedFirstTimeFlow)
				{
					num++;
				}
			}
			telemetryParams.Add("NumberOfUsedSlots", num.ToString());
			SingletonInstance<TelemetryManager>.Instance.LogEvent("SaveSlotSelected", telemetryParams, false);
			BuildAndSendPlayerSplits();
		}

		public void OnIAPWarningNavigation(Enum evt, GameObject gObj, object[] parameters)
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			switch ((IAPWarningEvent)(object)evt)
			{
			case IAPWarningEvent.IAP_Warning_Shown:
				telemetryParams.Add("Navigation Record", "Seen");
				break;
			case IAPWarningEvent.IAP_Warning_Accepted:
				telemetryParams.Add("Navigation Record", "Accepted");
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("IAP_WarningDialog", telemetryParams, false);
		}

		public void OnGeoCodeEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			string value = (string)parameters[0];
			string value2 = (string)parameters[1];
			if ((GeoCodeEvents)(object)evt == GeoCodeEvents.GeoCodeDownloaded)
			{
				TelemetryParams telemetryParams = new TelemetryParams();
				telemetryParams.Add("Country Code", value);
				telemetryParams.Add("Culture Code", value2);
				telemetryParams.Add("Language", Singleton<GameDataStoreObject>.Instance.GlobalData.m_Locale.ToString());
				SingletonInstance<TelemetryManager>.Instance.LogEvent("Geographical Location", telemetryParams, false);
			}
		}

		public void OnNonConsumablePurchaseResult(Enum evt, GameObject gObj, object[] parameters)
		{
			string key = (string)parameters[0];
			TelemetryParams telemetryParams = new TelemetryParams();
			switch ((InAppShopPurchaseState)(object)evt)
			{
			case InAppShopPurchaseState.InAppItem_PurchaseSuccess:
				telemetryParams.Add(key, "Purchased");
				break;
			case InAppShopPurchaseState.InAppItem_PurchaseFailed:
				telemetryParams.Add(key, "Failed");
				break;
			case InAppShopPurchaseState.InAppItem_PurchaseCancelled:
				telemetryParams.Add(key, "Cancelled");
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Non-ConsumablePurchase", telemetryParams, false);
		}

		public void OnCrystalUnlockEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			switch ((CrystalUnlockTelemetryEvents)(object)evt)
			{
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_EndDate:
				telemetryParams.Add("Crystal Unlock", "End Date");
				break;
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_Purchased:
				telemetryParams.Add("Crystal Unlock", "InApp Purchased");
				break;
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_ReceivedEggFromFriendsDevice:
				telemetryParams.Add("Crystal Unlock", "Received Egg (Friends Device)");
				break;
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_ReceivedEggFromFriendsFurbyToy:
				telemetryParams.Add("Crystal Unlock", "Received Egg (Friends Furby)");
				break;
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_ScannedFurby:
				telemetryParams.Add("Crystal Unlock", "Scanned Furby");
				break;
			case CrystalUnlockTelemetryEvents.CrystalUnlocked_ToyScan:
				telemetryParams.Add("Crystal Unlock", "Toy Scan");
				break;
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Crystal_Unlocked", telemetryParams, false);
		}

		public void DeleteSlotEvent(Enum evt, GameObject gObj, object[] parameters)
		{
			TelemetryParams telemParams = new TelemetryParams();
			SingletonInstance<TelemetryManager>.Instance.LogEvent("SaveSlotDeleted", telemParams, false);
		}

		private void AddGenericInfo(string previousScene)
		{
			if (m_sceneTelemetry == null)
			{
				m_sceneTelemetry = new TelemetryParams();
			}
			if (FurbyGlobals.Player.SelectedFurbyBaby == null)
			{
				m_sceneTelemetry.Add("Baby Tribe", "N/A");
				m_sceneTelemetry.Add("Tribe Iteration", "N/A");
			}
			else
			{
				m_sceneTelemetry.Add("Baby Tribe", FurbyGlobals.Player.SelectedFurbyBaby.m_persistantData.tribe);
				m_sceneTelemetry.Add("Tribe Iteration", FurbyGlobals.Player.SelectedFurbyBaby.m_persistantData.iter.ToString());
			}
			m_sceneTelemetry.Add("Furby Pattern", m_patternNames[FurbyGlobals.Player.Furby.AdultType]);
			m_sceneTelemetry.Add("Furby XP", GetGenericBucketedInt(FurbyGlobals.Player.XP));
			m_sceneTelemetry.Add("Furby Furbucks", GetGenericBucketedInt(FurbyGlobals.Wallet.Balance));
			m_sceneTelemetry.Add("Eggs in Carton", FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count().ToString());
			double[] multiples = new double[5] { 1.0, 2.0, 3.0, 5.0, 8.0 };
			m_sceneTelemetry.Add("Babies in Neighbourhood", GetGenericBucketedInt(FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Where((FurbyBaby x) => x.Progress == FurbyBabyProgresss.N && x.Tribe.TribeSet != Tribeset.Promo).Count(), multiples));
			m_sceneTelemetry.Add("Babies in Promo Neighbourhood", FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Where((FurbyBaby x) => x.Progress == FurbyBabyProgresss.N && x.Tribe.TribeSet == Tribeset.Promo).Count().ToString());
		}

		private void AddAdultInfo(string previousScene)
		{
			string value = m_patternNames[FurbyGlobals.Player.Furby.AdultType];
			m_sceneTelemetry.Add("Furby Pattern", value);
			m_sceneTelemetry.Add("Furby XP", GetGenericBucketedInt(FurbyGlobals.Player.XP));
			m_sceneTelemetry.Add("Furby Furbucks", GetGenericBucketedInt(FurbyGlobals.Wallet.Balance));
			m_sceneTelemetry.Add("Furby Happiness", BucketInt((int)(FurbyGlobals.Player.Happiness * 100f), 10));
			m_sceneTelemetry.Add("Furby Bowel Emptiness", BucketInt((int)(FurbyGlobals.Player.BowelEmptiness * 100f), 10));
			m_sceneTelemetry.Add("Furby Cleanliness", BucketInt((int)(FurbyGlobals.Player.Cleanliness * 100f), 10));
			m_sceneTelemetry.Add("Furby Satiatedness", BucketInt((int)(FurbyGlobals.Player.Satiatedness * 100f), 10));
			m_sceneTelemetry.Add("Furby Sickness", (!FurbyGlobals.Player.Sickness) ? "Not Sick" : "Sick");
		}

		private void AddBabyInfo(string previousScene)
		{
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			if (selectedFurbyBaby == null)
			{
				m_sceneTelemetry.Add("Baby Tribe", "N/A");
				m_sceneTelemetry.Add("Tribe Iteration", "N/A");
				m_sceneTelemetry.Add("Egg Tribe and Iteration", "N/A");
				m_sceneTelemetry.Add("Baby Name", "N/A");
				m_sceneTelemetry.Add("Baby Personality", "N/A");
				m_sceneTelemetry.Add("First Flair", "N/A");
				m_sceneTelemetry.Add("Attention", "N/A");
				m_sceneTelemetry.Add("Cleanliness", "N/A");
				m_sceneTelemetry.Add("Satiatedness", "N/A");
			}
			else
			{
				m_sceneTelemetry.Add("Baby Tribe", selectedFurbyBaby.m_persistantData.tribe);
				m_sceneTelemetry.Add("Tribe Iteration", selectedFurbyBaby.m_persistantData.iter.ToString());
				m_sceneTelemetry.Add("Egg Tribe and Iteration", selectedFurbyBaby.m_persistantData.tribe + " " + selectedFurbyBaby.m_persistantData.iter);
				m_sceneTelemetry.Add("Baby Name", (!selectedFurbyBaby.HasBeenNamed) ? "N/A" : selectedFurbyBaby.Name);
				m_sceneTelemetry.Add("Baby Personality", (selectedFurbyBaby.Progress == FurbyBabyProgresss.E) ? "N/A" : selectedFurbyBaby.Personality.ToString());
				m_sceneTelemetry.Add("First Flair", (selectedFurbyBaby.m_persistantData.flairs.Length <= 0) ? "N/A" : selectedFurbyBaby.m_persistantData.flairs[0]);
				m_sceneTelemetry.Add("Attention", BucketInt((int)(selectedFurbyBaby.Attention * 100f), 10));
				m_sceneTelemetry.Add("Cleanliness", BucketInt((int)(selectedFurbyBaby.Cleanliness * 100f), 10));
				m_sceneTelemetry.Add("Satiatedness", BucketInt((int)(selectedFurbyBaby.Satiatedness * 100f), 10));
			}
		}

		public void SendSceneActivity(Enum evt, GameObject gObj, object[] parameters)
		{
			if (m_sceneTelemetry == null)
			{
				m_sceneTelemetry = new TelemetryParams();
			}
			if (!m_sceneStart.HasValue)
			{
				m_sceneStart = DateTime.Now;
				return;
			}
			m_sceneTelemetry.Add("Time", GetBucketedTimePeriod(m_sceneStart.Value, DateTime.Now));
			string previousScene = (parameters[0] as string).ToLower();
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			int num = (int)data.IncrementSceneCount(previousScene);
			int stepPower = 4;
			m_sceneTelemetry.Add("Visits", BucketIntLog(num, stepPower));
			string text = string.Empty;
			if (m_adultScenes.Any((string x) => x.ToLower() == previousScene))
			{
				AddAdultInfo(previousScene);
				text = "AdultActivity_" + previousScene;
			}
			else if (m_babyScenes.Any((string x) => x.ToLower() == previousScene))
			{
				AddBabyInfo(previousScene);
				text = "BabyActivity_" + previousScene;
			}
			else if (!m_scenesToExclude.Any((string x) => x.ToLower() == previousScene))
			{
				AddGenericInfo(previousScene);
				text = "Activity_" + previousScene;
			}
			if (text.Length > 0)
			{
				SingletonInstance<TelemetryManager>.Instance.LogEvent(text, m_sceneTelemetry, false);
			}
			m_sceneTelemetry = new TelemetryParams();
			m_sceneStart = DateTime.Now;
		}

		public void ScannerEvents(Enum evt, GameObject obj, object[] parameters)
		{
			switch ((ScannerEvents)(object)evt)
			{
			case Furby.Scanner.ScannerEvents.ScanningStarted:
				if (m_lastScan == null)
				{
					m_lastScan = new ScanInformation();
					m_lastScan.ScanType = ScanType.Dashboard;
					m_successWillBeValid = true;
				}
				break;
			case Furby.Scanner.ScannerEvents.InitialScanComplete:
				if (m_lastScan == null)
				{
					m_lastScan = new ScanInformation();
					m_lastScan.ScanType = ScanType.FirstTime;
					m_successWillBeValid = true;
				}
				m_lastScan.NumAttempts++;
				m_lastScan.NamingWasRequired = true;
				m_lastScan.ScanType = ScanType.FirstTime;
				m_lastScan.Result = ScanResult.Success;
				SendScanData();
				break;
			case Furby.Scanner.ScannerEvents.InitialScanCompleteNamingNotRequired:
				if (m_lastScan == null)
				{
					m_lastScan = new ScanInformation();
					m_lastScan.ScanType = ScanType.FirstTime;
					m_successWillBeValid = true;
				}
				m_lastScan.NumAttempts++;
				m_lastScan.NamingWasRequired = false;
				m_lastScan.ScanType = ScanType.FirstTime;
				m_lastScan.Result = ScanResult.Success;
				SendScanData();
				break;
			case Furby.Scanner.ScannerEvents.ScanningSucceeded:
				if (m_successWillBeValid)
				{
					if (m_lastScan == null)
					{
						m_lastScan = new ScanInformation();
						m_lastScan.ScanType = ScanType.Dashboard;
					}
					SendScanResult(ScanResult.Success.ToString());
					m_lastScan.NumAttempts++;
					m_lastScan.Result = ScanResult.Success;
					m_lastScan.PlayedWithFurby = true;
					SendScanData();
				}
				else
				{
					m_successWillBeValid = true;
				}
				break;
			case Furby.Scanner.ScannerEvents.IncorrectFurbyFound:
				SendScanResult(ScanResult.IncorrectFurby.ToString());
				m_lastScan.Result = ScanResult.IncorrectFurby;
				m_lastScan.NumAttempts++;
				m_successWillBeValid = false;
				break;
			case Furby.Scanner.ScannerEvents.InitialScanFailed:
			case Furby.Scanner.ScannerEvents.NoFurbyFound:
				SendScanResult(ScanResult.NoFurbyFound.ToString());
				m_lastScan.Result = ScanResult.NoFurbyFound;
				m_lastScan.NumAttempts++;
				break;
			case Furby.Scanner.ScannerEvents.PlayWithoutFurbyTemporarily:
				m_lastScan.PlayedWithFurby = false;
				SendScanData();
				break;
			case Furby.Scanner.ScannerEvents.ScanningCancelled:
				SendScanData();
				break;
			case Furby.Scanner.ScannerEvents.OldFurbyFound:
				SendScanResult(ScanResult.Furby1Detected.ToString());
				m_lastScan.Result = ScanResult.Furby1Detected;
				m_lastScan.NumAttempts++;
				break;
			case Furby.Scanner.ScannerEvents.ScanningFailed:
			case Furby.Scanner.ScannerEvents.RequiresInitialScan:
			case Furby.Scanner.ScannerEvents.GoBackToModeChoice:
			case Furby.Scanner.ScannerEvents.FirstScanDisableInput:
				break;
			}
		}

		private void SendScanResult(string result)
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			telemetryParams.Add("Scan Result", result);
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Scan Result", telemetryParams, false);
		}

		private void AddAdultStatusToTelemetry(TelemetryParams telemetryParams)
		{
			telemetryParams.Add("Furby Pattern", m_patternNames[FurbyGlobals.Player.Furby.AdultType]);
			telemetryParams.Add("Furby Name", FurbyGlobals.Player.FullName);
			telemetryParams.Add("Furby Happiness", BucketInt((int)(FurbyGlobals.Player.Happiness * 100f), 10));
			telemetryParams.Add("Furby Bowel Emptiness", BucketInt((int)(FurbyGlobals.Player.BowelEmptiness * 100f), 10));
			telemetryParams.Add("Furby Cleanliness", BucketInt((int)(FurbyGlobals.Player.Cleanliness * 100f), 10));
			telemetryParams.Add("Furby Satiatedness", BucketInt((int)(FurbyGlobals.Player.Satiatedness * 100f), 10));
			telemetryParams.Add("Furby Sickness", (!FurbyGlobals.Player.Sickness) ? "Not Sick" : "Sick");
			telemetryParams.Add("Furby XP", GetGenericBucketedInt(FurbyGlobals.Player.XP));
			telemetryParams.Add("Furby Furbucks", GetGenericBucketedInt(FurbyGlobals.Wallet.Balance));
			telemetryParams.Add("Eggs in Carton", FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count().ToString());
			double[] multiples = new double[5] { 1.0, 2.0, 3.0, 5.0, 8.0 };
			telemetryParams.Add("Babies in Neighbourhood", GetGenericBucketedInt(FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Where((FurbyBaby x) => x.Progress == FurbyBabyProgresss.N && x.Tribe.TribeSet != Tribeset.Promo).Count(), multiples));
			telemetryParams.Add("Babies in Promo Neighbourhood", FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Where((FurbyBaby x) => x.Progress == FurbyBabyProgresss.N && x.Tribe.TribeSet == Tribeset.Promo).Count().ToString());
		}

		private void AddBabyStatusToTelemetry(TelemetryParams telemetryParams, FurbyBaby baby)
		{
			telemetryParams.Add("Baby Tribe", baby.m_persistantData.tribe);
			telemetryParams.Add("Tribe Iteration", baby.m_persistantData.iter.ToString());
			telemetryParams.Add("Egg Tribe and Iteration", baby.m_persistantData.tribe + " " + baby.m_persistantData.iter);
			telemetryParams.Add("Baby Name", (!baby.HasBeenNamed) ? "N/A" : baby.Name);
			telemetryParams.Add("Baby Personality", (baby.Progress == FurbyBabyProgresss.E) ? "N/A" : baby.Personality.ToString());
			telemetryParams.Add("First Flair", (baby.m_persistantData.flairs.Length <= 0) ? "N/A" : baby.m_persistantData.flairs[0]);
			telemetryParams.Add("Second Flair", (baby.m_persistantData.flairs.Length <= 1) ? "N/A" : baby.m_persistantData.flairs[1]);
		}

		private void SendScanData()
		{
			TelemetryParams telemetryParams = new TelemetryParams();
			telemetryParams.Add("Scan Type", m_lastScan.ScanType.ToString());
			telemetryParams.Add("Number of Attempts", m_lastScan.NumAttempts.ToString());
			telemetryParams.Add("Result", m_lastScan.Result.ToString());
			if (m_lastScan.NamingWasRequired.HasValue)
			{
				telemetryParams.Add("Naming Was Required", m_lastScan.NamingWasRequired.Value.ToString());
			}
			if (m_lastScan.PlayedWithFurby.HasValue)
			{
				telemetryParams.Add("Skipped scanning", (!m_lastScan.PlayedWithFurby.Value).ToString());
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Scan Sequence", telemetryParams, false);
			telemetryParams = new TelemetryParams();
			if (m_lastScan.FurbyStatus.HasValue)
			{
				telemetryParams.Add("Furby Generation", (!m_lastScan.FurbyStatus.Value.Supported) ? "Furby 1" : "Furby 2");
				if (m_lastScan.FurbyStatus.Value.Supported)
				{
					telemetryParams.Add("Furby Pattern", m_patternNames[AdultFurbyLibrary.ConvertComAirPatternToAdultType(m_lastScan.FurbyStatus.Value.Pattern)]);
					telemetryParams.Add("Furby Name", m_lastScan.FurbyStatus.Value.Name.ToString());
					telemetryParams.Add("Furby Personality", m_lastScan.FurbyStatus.Value.Personality.ToString());
					telemetryParams.Add("Furby Happiness", BucketInt(m_lastScan.FurbyStatus.Value.Happyness, 10));
					telemetryParams.Add("Furby Fullness", BucketInt(m_lastScan.FurbyStatus.Value.Fullness, 10));
					telemetryParams.Add("Furby Sickness", BucketInt(m_lastScan.FurbyStatus.Value.Sickness, 10));
				}
			}
			SingletonInstance<TelemetryManager>.Instance.LogEvent("Scan Furby Results", telemetryParams, false);
			m_lastScan = null;
		}

		public void OnFurbyDataEvent(Enum evt, GameObject obj, object[] parameters)
		{
			FurbyDataEvent furbyDataEvent = (FurbyDataEvent)(object)evt;
			if (furbyDataEvent.Equals(FurbyDataEvent.FurbyDataReceived))
			{
				FurbyStatus value = (FurbyStatus)parameters[0];
				if (m_lastScan != null)
				{
					m_lastScan.FurbyStatus = value;
				}
			}
		}

		private string SpaceOutCamelCase(string input)
		{
			input = char.ToUpper(input[0]) + input.Substring(1);
			input = Regex.Replace(input, "([A-Z])", " $1", RegexOptions.None).Trim();
			return input;
		}

		private string BucketInt(int value, int stepSize)
		{
			int num = value / stepSize * stepSize;
			return string.Format("{0} - {1}", num, num + stepSize - 1);
		}

		private string BucketFloatLog(float f, int stepPower)
		{
			float f2 = Mathf.Log(f, stepPower);
			int num = Mathf.FloorToInt(f2);
			float num2 = Mathf.Pow(stepPower, num);
			float num3 = Mathf.Pow(stepPower, num + 1) - 1f;
			return string.Format("{0} - {1}", num2, num3);
		}

		private string BucketIntLog(float i, int stepPower)
		{
			if (i == 0f)
			{
				return "s";
			}
			return BucketFloatLog(i, stepPower);
		}

		public string GetBucketedTimePeriod(long start, long end)
		{
			return GetBucketedTimePeriod(new DateTime(start), new DateTime(end));
		}

		public string GetBucketedTimePeriod(DateTime start, DateTime end)
		{
			return GetBucketedTimePeriod(end - start);
		}

		public string GetBucketedTimePeriod(TimeSpan timespan)
		{
			int num = (int)timespan.TotalSeconds;
			for (int i = 0; i < m_timeBuckets.Length; i++)
			{
				if (num <= m_timeBuckets[i].Key)
				{
					return m_timeBuckets[i].Value;
				}
			}
			return "More than 4 years";
		}

		public string GetGenericBucketedInt(double value)
		{
			double[] multiples = new double[4] { 1.0, 2.5, 5.0, 7.5 };
			return GetGenericBucketedInt(value, multiples);
		}

		public string GetGenericBucketedInt(double value, double[] multiples)
		{
			if (value == 0.0)
			{
				return "0";
			}
			double num = 1.0;
			for (double num2 = 1.0; num2 < 1000000000000.0; num2 *= 10.0)
			{
				foreach (double num3 in multiples)
				{
					double num4 = Math.Floor(num2 * num3);
					if (value <= num4)
					{
						if (num == num4)
						{
							return string.Format("{0}", num);
						}
						return string.Format("{0} - {1}", num, num4);
					}
					num = num4 + 1.0;
				}
			}
			return "Greater than 7500000000000";
		}

		public void OnBabyLifecycleEvent(Enum evt, GameObject gobj, object[] parameters)
		{
			BabyLifecycleEvent babyLifecycleEvent = (BabyLifecycleEvent)(object)evt;
			switch (babyLifecycleEvent)
			{
			case BabyLifecycleEvent.FromOwnFurbyToy:
			case BabyLifecycleEvent.FromVirtualFurby:
			case BabyLifecycleEvent.FromSpecialFriend:
			case BabyLifecycleEvent.FromFriendsFurbyToy:
			case BabyLifecycleEvent.FromQRCode:
			case BabyLifecycleEvent.FromShop:
			case BabyLifecycleEvent.FromFriendsDevice:
			{
				FurbyBaby furbyBaby3 = (FurbyBaby)parameters[0];
				TelemetryParams telemetryParams3 = new TelemetryParams();
				telemetryParams3.Add("Egg Source", SpaceOutCamelCase(babyLifecycleEvent.ToString().Substring(4)));
				telemetryParams3.Add("Egg Tribe", furbyBaby3.m_persistantData.tribe);
				telemetryParams3.Add("Tribe Iteration", furbyBaby3.m_persistantData.iter.ToString());
				telemetryParams3.Add("Egg Tribe and Iteration", furbyBaby3.m_persistantData.tribe + " " + furbyBaby3.m_persistantData.iter);
				telemetryParams3.Add("Time spent playing", GetBucketedTimePeriod(new TimeSpan(Singleton<GameDataStoreObject>.Instance.Data.TimeSpentPlaying)));
				SingletonInstance<TelemetryManager>.Instance.LogEvent("Egg Received " + SpaceOutCamelCase(babyLifecycleEvent.ToString()), telemetryParams3, false);
				if (furbyBaby3.Tribe.TribeSet == Tribeset.Golden)
				{
					telemetryParams3 = new TelemetryParams();
					telemetryParams3.Add("Furby Pattern", m_patternNames[FurbyGlobals.Player.Furby.AdultType]);
					telemetryParams3.Add("Furby XP", GetGenericBucketedInt(FurbyGlobals.Player.XP));
					telemetryParams3.Add("Furby Furbucks", GetGenericBucketedInt(FurbyGlobals.Wallet.Balance));
					telemetryParams3.Add("Time spent playing", GetBucketedTimePeriod(new TimeSpan(Singleton<GameDataStoreObject>.Instance.Data.TimeSpentPlaying)));
					SingletonInstance<TelemetryManager>.Instance.LogEvent("Golden_Egg_Received", telemetryParams3, false);
				}
				break;
			}
			case BabyLifecycleEvent.BabyNamed:
			{
				FurbyBaby furbyBaby2 = (FurbyBaby)parameters[0];
				TelemetryParams telemetryParams2 = new TelemetryParams();
				telemetryParams2.Add("Time Since Egg Laid", GetBucketedTimePeriod(furbyBaby2.m_persistantData.LayingTime, furbyBaby2.m_persistantData.HatchingTime));
				AddBabyStatusToTelemetry(telemetryParams2, furbyBaby2);
				telemetryParams2.Add("Time spent playing", GetBucketedTimePeriod(new TimeSpan(Singleton<GameDataStoreObject>.Instance.Data.TimeSpentPlaying)));
				SingletonInstance<TelemetryManager>.Instance.LogEvent("Baby Hatched & Named", telemetryParams2, false);
				break;
			}
			case BabyLifecycleEvent.BabyGraduated:
			{
				FurbyBaby furbyBaby = (FurbyBaby)parameters[0];
				TelemetryParams telemetryParams = new TelemetryParams();
				telemetryParams.Add("Time Since Hatching", GetBucketedTimePeriod(furbyBaby.m_persistantData.HatchingTime, furbyBaby.m_persistantData.GraduationTime));
				AddBabyStatusToTelemetry(telemetryParams, furbyBaby);
				telemetryParams.Add("Time spent playing", GetBucketedTimePeriod(new TimeSpan(Singleton<GameDataStoreObject>.Instance.Data.TimeSpentPlaying)));
				SingletonInstance<TelemetryManager>.Instance.LogEvent("Baby Graduated", telemetryParams, false);
				break;
			}
			}
		}

		public void OnVirtualItemPurchase(Enum evt, GameObject gobj, object[] parameters)
		{
			switch ((VirtualItemPurchase)(object)evt)
			{
			case VirtualItemPurchase.BabyItem:
			{
				ShopPurchaseableItem shopPurchaseableItem = parameters[0] as ShopPurchaseableItem;
				if (!(shopPurchaseableItem != null))
				{
					break;
				}
				TelemetryParams telemetryParams2 = new TelemetryParams();
				telemetryParams2.Add("Name", shopPurchaseableItem.GetItemName());
				telemetryParams2.Add("Cost", shopPurchaseableItem.GetFurbucksCost().ToString());
				telemetryParams2.Add("Source", "Shop");
				BabyUtilityShopItem babyUtilityShopItem = parameters[0] as BabyUtilityShopItem;
				if (babyUtilityShopItem != null)
				{
					if (babyUtilityShopItem.UtilityType == typeof(SalonItem))
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_SalonlItem", telemetryParams2, false);
					}
					if (babyUtilityShopItem.UtilityType == typeof(Ingredient))
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_BlenderlItem", telemetryParams2, false);
					}
				}
				SelectableFeatureShopItem selectableFeatureShopItem = parameters[0] as SelectableFeatureShopItem;
				if (selectableFeatureShopItem != null)
				{
					SelectableFeature selectableFeature2 = selectableFeatureShopItem.Feature as SelectableFeature;
					if (selectableFeature2 != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomFeature", telemetryParams2, false);
					}
					SelectableTheme selectableTheme2 = selectableFeatureShopItem.Feature as SelectableTheme;
					if (selectableTheme2 != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomTheme", telemetryParams2, false);
					}
				}
				PromoEggShopItem promoEggShopItem = parameters[0] as PromoEggShopItem;
				if (promoEggShopItem != null)
				{
					SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PromoEgg", telemetryParams2, false);
				}
				break;
			}
			case VirtualItemPurchase.InGamePurchaseable:
			{
				InGamePurchaseableItem inGamePurchaseableItem = parameters[0] as InGamePurchaseableItem;
				if (inGamePurchaseableItem != null)
				{
					TelemetryParams telemetryParams3 = new TelemetryParams();
					telemetryParams3.Add("Name", inGamePurchaseableItem.GetItemName());
					telemetryParams3.Add("Cost", inGamePurchaseableItem.GetFurbucksCost().ToString());
					telemetryParams3.Add("Source", "Carousel");
					Furby.Utilities.Blender.CarouselItem carouselItem = parameters[0] as Furby.Utilities.Blender.CarouselItem;
					if (carouselItem != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_BlenderlItem", telemetryParams3, false);
					}
					Furby.Utilities.Salon2.CarouselItem carouselItem2 = parameters[0] as Furby.Utilities.Salon2.CarouselItem;
					if (carouselItem2 != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_SalonlItem", telemetryParams3, false);
					}
					PlayroomFeatureSelect playroomFeatureSelect = parameters[0] as PlayroomFeatureSelect;
					if (playroomFeatureSelect != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomFeature", telemetryParams3, false);
					}
					PlayroomThemeSelect playroomThemeSelect = parameters[0] as PlayroomThemeSelect;
					if (playroomThemeSelect != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomTheme", telemetryParams3, false);
					}
				}
				break;
			}
			case VirtualItemPurchase.PlayroomItem:
			{
				IPlayroomSelectable playroomSelectable = parameters[0] as IPlayroomSelectable;
				if (playroomSelectable != null)
				{
					TelemetryParams telemetryParams = new TelemetryParams();
					telemetryParams.Add("Name", playroomSelectable.GetName());
					telemetryParams.Add("Cost", playroomSelectable.GetCost().ToString());
					SelectableFeature selectableFeature = parameters[0] as SelectableFeature;
					if (selectableFeature != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomFeature", telemetryParams, false);
					}
					SelectableTheme selectableTheme = parameters[0] as SelectableTheme;
					if (selectableTheme != null)
					{
						SingletonInstance<TelemetryManager>.Instance.LogEvent("VirtualPurchase_PlayroomTheme", telemetryParams, false);
					}
				}
				break;
			}
			}
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Telemetry"))
			{
				GUILayout.BeginVertical();
				GUILayout.Label("[Identifiers]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				OnInspectProperty("DeviceID", TelemetryManager.DeviceId);
				OnInspectProperty("DeviceID (Hashed)", Hash.ComputeHash(TelemetryManager.DeviceId, Hash.Algorithm.SHA256, "DeviceId"));
				OnInspectProperty("SessionID", TelemetryManager.SessionId);
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}

		private void OnInspectProperty(string title, string body)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(title + ":  ", RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(false));
			GUILayout.Label(" " + body, RelentlessGUIStyles.Style_Normal, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
		}
	}
}
