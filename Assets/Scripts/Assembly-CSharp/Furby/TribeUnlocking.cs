using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class TribeUnlocking : SingletonInstance<TribeUnlocking>
	{
		[SerializeField]
		public UnlockDate m_FailsafeEligibilityDateForCrystal;

		public float m_MaxTimeSecsToWaitForInternet = 10f;

		private GameEventSubscription m_DebugPanelSub;

		private static float m_ColumnWidth = 300f;

		public override void Awake()
		{
			StartCoroutine(InitializeSelf());
		}

		private IEnumerator InitializeSelf()
		{
			DebugUtils.Log_InCyan("TribeUnlocking:: Waiting for network...");
			yield return StartCoroutine(WaitForNetwork());
			DebugUtils.Log_InCyan("TribeUnlocking:: Got network, waiting for GameConfig resolution...");
			yield return StartCoroutine(WaitForGameConfigResolution());
			DebugUtils.Log_InCyan("TribeUnlocking:: Got GameConfig resolution, adjudicating status");
			DecideEligibilityAndUnlockStatus();
		}

		private IEnumerator WaitForGameConfigResolution()
		{
			while (SingletonInstance<GameConfigDownloader>.Instance.AmDownloading)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForNetwork()
		{
			Logging.Log("<color=cyan>TribeUnlocking:: Waiting for network...</color>");
			while (!SetupNetworking.IsReady)
			{
				yield return new WaitForEndOfFrame();
			}
			if (SingletonInstance<SetupNetworking>.Instance != null)
			{
				float timeNow = Time.time;
				float timeEnd = timeNow + m_MaxTimeSecsToWaitForInternet;
				while (Time.time <= timeEnd && !SetupNetworking.IsReady)
				{
					yield return new WaitForEndOfFrame();
				}
				if (!SetupNetworking.IsReady)
				{
					Logging.Log("<color=cyan>TribeUnlocking:: Gave up waiting for network</color> Waited: " + m_MaxTimeSecsToWaitForInternet + " seconds.");
				}
			}
		}

		private void DecideEligibilityAndUnlockStatus()
		{
			DebugUtils.Log_InCyan("TribeUnlocking::DecideEligibilityAndUnlockStatus\n");
			DebugUtils.Log_InCyan("Status - CountryCode  - " + Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode);
			DebugUtils.Log_InCyan("Status - CurrentDate  - " + DateTime.Now.ToString("yyyy-mm-dd"));
			DebugUtils.Log_InCyan("Status - Expire Date  - " + m_FailsafeEligibilityDateForCrystal.UnlockDateTime.ToString("yyyy-mm-dd"));
			DebugUtils.Log_InCyan("Eligible for Spring?  - " + ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring) ? "NO" : "YES"));
			DebugUtils.Log_InCyan("Eligible for Crystal? - " + ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal) ? "NO" : "YES"));
			DebugUtils.Log_InCyan("Unlocked for Crystal? - " + ((!Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked) ? "NO" : "YES"));
			GameDataStoreObject instance = Singleton<GameDataStoreObject>.Instance;
			if (!instance.GlobalData.AmEligibleForSpring && IsGeographicalLocationEligibleForTribe(Tribeset.Spring))
			{
				instance.GlobalData.MakeSpringEligible();
				instance.Save();
			}
			if (!instance.GlobalData.AmEligibleForCrystal)
			{
				Logging.Log("<color=cyan>TribeUnlocking::Currently NOT eligible for Crystal</color>");
				if (IsGeographicalLocationEligibleForTribe(Tribeset.CrystalGem))
				{
					Logging.Log("<color=cyan>TribeUnlocking::...however the CountryCode says otherwise, making eligible!</color>");
					instance.GlobalData.MakeCrystalEligible();
					instance.Save();
				}
				if (DateTime.Now >= m_FailsafeEligibilityDateForCrystal.UnlockDateTime)
				{
					Logging.Log("<color=cyan>TribeUnlocking::...however the Date says otherwise, making eligible!</color>");
					instance.GlobalData.MakeCrystalEligible();
					instance.Save();
				}
			}
		}

		public static bool IsGeographicalLocationEligibleForTribe(Tribeset tribe)
		{
			if (SetupNetworking.IsReady)
			{
				GameConfigBlob gameConfigBlob = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob();
				if (gameConfigBlob != null)
				{
					return gameConfigBlob.DoesGeoCodeAllowTribe(tribe);
				}
				return false;
			}
			return false;
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			m_DebugPanelSub.Dispose();
		}

		private static void ShowState(string title, bool state, string pos, string neg)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(title, RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
			GUILayout.Label((!state) ? neg : pos, (!state) ? RelentlessGUIStyles.Style_RsRed : RelentlessGUIStyles.Style_RsGreen, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Tribe Eligibility"))
			{
				GUILayout.Space(10f);
				GUILayout.Label("[Eligibility Rules]", RelentlessGUIStyles.Style_Header);
				GUILayout.BeginHorizontal();
				GUILayout.Label("CountryCode:  ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUILayout.Label(Singleton<GameDataStoreObject>.Instance.GlobalData.CountryCode.ToString(), RelentlessGUIStyles.Style_Normal);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("CurrentDate:  ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUILayout.Label(DateTime.Now.ToString("yyyy-mm-dd_HH"), RelentlessGUIStyles.Style_Normal);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Expire Date:  ", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUILayout.Label(m_FailsafeEligibilityDateForCrystal.UnlockDateTime.ToString("yyyy-mm-dd_HH"), RelentlessGUIStyles.Style_Normal);
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.Label("[Current Status]", RelentlessGUIStyles.Style_Header);
				ShowState("Eligible for Spring?", Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring, "Eligible", "Ineligible");
				ShowState("Eligible for Crystal?", Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal, "Eligible", "Ineligible");
				ShowState("Crystal Unlocked?", Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked, "Unlocked", "Locked");
				GUILayout.Space(10f);
				GUILayout.Label("[Overrides]", RelentlessGUIStyles.Style_Header);
				ShowSpringControl();
				ShowCrystalControl();
				ShowCrystalUnlockControl();
			}
			DebugPanel.EndSection();
		}

		private void ShowSpringControl()
		{
			GUILayout.BeginHorizontal();
			if (!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring)
			{
				GUILayout.Label("Spring:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (GUILayout.Button("Make Eligible", GUILayout.Width(m_ColumnWidth)))
				{
					Singleton<GameDataStoreObject>.Instance.GlobalData.MakeSpringEligible();
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
			else
			{
				GUILayout.Label("Spring:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (!GUILayout.Button("Make Ineligible", GUILayout.Width(m_ColumnWidth)))
				{
				}
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
		}

		private void ShowCrystalControl()
		{
			GUILayout.BeginHorizontal();
			if (!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal)
			{
				GUILayout.Label("Crystal:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (GUILayout.Button("Make Eligible", GUILayout.Width(m_ColumnWidth)))
				{
					Singleton<GameDataStoreObject>.Instance.GlobalData.MakeSpringEligible();
					Singleton<GameDataStoreObject>.Instance.GlobalData.MakeCrystalEligible();
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
			else
			{
				GUILayout.Label("Crystal:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (!GUILayout.Button("Make Ineligible", GUILayout.Width(m_ColumnWidth)))
				{
				}
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
		}

		private void ShowCrystalUnlockControl()
		{
			GUILayout.BeginHorizontal();
			if (!Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
			{
				GUILayout.Label("Crystal:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (GUILayout.Button("Unlock", GUILayout.Width(m_ColumnWidth)))
				{
					Singleton<GameDataStoreObject>.Instance.GlobalData.UnlockCrystal();
					Singleton<GameDataStoreObject>.Instance.Save();
				}
			}
			else
			{
				GUILayout.Label("Crystal:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(m_ColumnWidth));
				GUI.backgroundColor = ((!Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked) ? new Color(0.25f, 0.88f, 0.25f) : new Color(0.88f, 0.25f, 0.25f));
				if (!GUILayout.Button("Lock", GUILayout.Width(m_ColumnWidth)))
				{
				}
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
		}
	}
}
