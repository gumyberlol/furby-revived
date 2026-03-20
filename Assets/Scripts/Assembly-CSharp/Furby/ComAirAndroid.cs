using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ComAirAndroid : MonoBehaviour
	{
		private enum eComAirErrCode
		{
			COMAIR_NOERR = 0,
			COMAIR_AUDIOUINTFAILED = -1,
			COMAIR_ENABLEIORECFAILED = -2,
			COMAIR_SETFORMATFAILED = -3,
			COMAIR_SETRECCALLBACKFAILED = -4,
			COMAIR_ALLOCBUFFAILED = -5,
			COMAIR_AUDIONOTINIT = -6,
			COMAIR_UNSUPPORTMODE = -7,
			COMAIR_UNSUPPORTTHRESHOLD = -8,
			COMAIR_SETREGCODEFAILED = -9,
			COMAIR_PLAYCOMAIRSOUNDFAILED = -10,
			COMAIR_PROPERTYNOTFOUND = -11,
			COMAIR_PROPERTYOPERATIONFAILED = -12
		}

		public enum eAudioDecodeMode
		{
			Is1Sec = 1,
			Is05Sec = 2
		}

		public enum eAudioEncodeMode
		{
			Is1Sec = 24,
			Is05Sec = 48
		}

		public enum eComAirProperty
		{
			RegCode = 0,
			CentralFreq = 1,
			iDfValue = 2,
			Threshold = 3,
			VolumeCtrl = 4,
			WaveFormType = 5,
			Mode = 6,
			ChannelSel = 7,
			ChannelEnable = 8,
			ClockLostThreshold = 9,
			BoostMode = 10
		}

		public enum eComAirPropertyTarget
		{
			Both = 0,
			Encode = 1,
			Decode = 2
		}

		public enum eComAirChannelSel
		{
			Ch1 = 0,
			Ch2 = 1
		}

		public enum eComAirChannelEnable
		{
			Disable = 0,
			Enable = 1
		}

		public enum eComAirBoostMode
		{
			Disable = 0,
			Is18Bit = 1
		}

		private static AndroidJavaObject s_comairJava;

		private static string s_ComAirPinCode = "ZewEhexk";

		private static Queue<long> s_sentCodes = new Queue<long>();

		private static Queue<long> s_rcvCodes = new Queue<long>();

		private static Queue<Action> s_commands = new Queue<Action>();

		private static Queue<string> s_log = new Queue<string>();

		private static float s_volume = 0f;

		private bool m_showLog;

		private GameEventSubscription m_debugSubs;

		private static eComAirErrCode SetComAirProperty(eComAirPropertyTarget target, eComAirProperty property, object value)
		{
			AndroidJNI.AttachCurrentThread();
			string text = value as string;
			if (text != null)
			{
				return (eComAirErrCode)s_comairJava.Call<int>("WrapSetComAirPropertyString", new object[3]
				{
					(int)target,
					(int)property,
					text
				});
			}
			int num = (int)value;
			return (eComAirErrCode)s_comairJava.Call<int>("WrapSetComAirPropertyInt", new object[3]
			{
				(int)target,
				(int)property,
				num
			});
		}

		private static eComAirErrCode StartComAirDecode()
		{
			return (eComAirErrCode)s_comairJava.Call<int>("StartComAir2ChDecodeProcess", new object[0]);
		}

		private static eComAirErrCode StopComAirDecode()
		{
			return (eComAirErrCode)s_comairJava.Call<int>("StopComAir2ChDecodeProcess", new object[0]);
		}

		public static int ComAirInitialiseEvent()
		{
			s_comairJava = new AndroidJavaObject("generalplus.com.GPLib.ComAir2ChWrapper");
			eComAirErrCode eComAirErrCode2 = eComAirErrCode.COMAIR_NOERR;
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelSel, eComAirChannelSel.Ch2);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelEnable, eComAirChannelEnable.Disable);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelSel, eComAirChannelSel.Ch1);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelEnable, eComAirChannelEnable.Enable);
			SetComAirProperty(eComAirPropertyTarget.Encode, eComAirProperty.Mode, eAudioEncodeMode.Is05Sec);
			SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.Mode, eAudioDecodeMode.Is05Sec);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.RegCode, s_ComAirPinCode);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.CentralFreq, 17500);
			SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.BoostMode, eComAirBoostMode.Disable);
			return (int)StartComAirDecode();
		}

		public static int ComAirShutdownEvent()
		{
			StopComAirDecode();
			s_comairJava.Call("Destroy");
			s_comairJava.Dispose();
			s_comairJava = null;
			return 1;
		}

		public static int ComAirDualBoostModeEventActual(bool active)
		{
			if (active)
			{
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.ChannelSel, eComAirChannelSel.Ch2);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.ChannelEnable, eComAirChannelEnable.Enable);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.Mode, eAudioDecodeMode.Is05Sec);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.RegCode, s_ComAirPinCode);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.CentralFreq, 18000);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.iDfValue, 100);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.BoostMode, eComAirBoostMode.Is18Bit);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.ChannelSel, eComAirChannelSel.Ch1);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.ChannelEnable, eComAirChannelEnable.Enable);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.Mode, eAudioDecodeMode.Is05Sec);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.RegCode, s_ComAirPinCode);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.CentralFreq, 17000);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.iDfValue, 100);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.BoostMode, eComAirBoostMode.Is18Bit);
			}
			else
			{
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelSel, eComAirChannelSel.Ch2);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelEnable, eComAirChannelEnable.Disable);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelSel, eComAirChannelSel.Ch1);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.ChannelEnable, eComAirChannelEnable.Enable);
				SetComAirProperty(eComAirPropertyTarget.Encode, eComAirProperty.Mode, eAudioEncodeMode.Is05Sec);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.RegCode, s_ComAirPinCode);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.CentralFreq, 17500);
				SetComAirProperty(eComAirPropertyTarget.Decode, eComAirProperty.iDfValue, 562);
				SetComAirProperty(eComAirPropertyTarget.Both, eComAirProperty.BoostMode, eComAirBoostMode.Disable);
			}
			s_comairJava.Call("SetBoostMode", active);
			s_log.Enqueue(string.Format("---->>>> Set Boost Mode to {0}", active));
			return 0;
		}

		public static int ComAirDualBoostModeEvent(bool active)
		{
			s_commands.Enqueue(delegate
			{
				ComAirDualBoostModeEventActual(active);
			});
			return 0;
		}

		[DllImport("libGPLibComAir2Ch")]
		private static extern int GenerateComAirCommand(long commandVal);

		public static void ComAirSendEventActual(long code)
		{
			s_volume = Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel;
			s_comairJava.Call("PlayComAirCmd", code, s_volume);
			s_sentCodes.Enqueue(code);
			s_log.Enqueue(string.Format("---->>>> Sent code {0}", code));
		}

		public static int ComAirSendEvent(long code, float ignore)
		{
			s_commands.Enqueue(delegate
			{
				ComAirSendEventActual(code);
			});
			return 0;
		}

		private void Update()
		{
			if (s_comairJava == null)
			{
				return;
			}
			long num = s_comairJava.Call<long>("GetLastCode", new object[0]);
			if (num != -1)
			{
				lock (s_sentCodes)
				{
					if (s_sentCodes.Count == 0 || num != s_sentCodes.Dequeue())
					{
						s_sentCodes.Clear();
						lock (s_rcvCodes)
						{
							s_rcvCodes.Enqueue(num);
						}
					}
					else
					{
						s_log.Enqueue(string.Format("<<<<---- Discarded code {0}", num));
					}
				}
			}
			lock (s_commands)
			{
				while (s_commands.Count > 5)
				{
					s_commands.Dequeue();
				}
				while (s_commands.Count != 0)
				{
					s_commands.Dequeue()();
				}
			}
			if (s_log.Count > 30)
			{
				s_log.Dequeue();
			}
		}

		public static long ComAirReceiveEvent()
		{
			lock (s_rcvCodes)
			{
				if (s_rcvCodes.Count != 0)
				{
					long num = s_rcvCodes.Dequeue();
					s_log.Enqueue(string.Format("<<<<---- Recieved code {0}", num));
					return num;
				}
			}
			return -1L;
		}

		private void OnEnable()
		{
			m_debugSubs = new GameEventSubscription(OnDebugRender, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDisable()
		{
			m_debugSubs.Dispose();
			m_debugSubs = null;
		}

		private void OnDebugRender(Enum evt, GameObject originator, params object[] p)
		{
			if (DebugPanel.StartSection("ComAir Android"))
			{
				m_showLog = GUILayout.Toggle(m_showLog, "Show ComAir Log");
				GUILayout.BeginHorizontal();
				GUILayout.Label("Volume");
				GUILayout.Label(s_volume.ToString());
				if (GUILayout.Button("   -0.1    "))
				{
					s_volume = Mathf.Clamp01(s_volume - 0.1f);
				}
				if (GUILayout.Button("   +0.1    "))
				{
					s_volume = Mathf.Clamp01(s_volume + 0.1f);
				}
				GUILayout.EndHorizontal();
				GUILayout.Label(string.Format("Current Hardware: {0}", SystemInfo.deviceModel));
			}
			DebugPanel.EndSection();
		}
	}
}
