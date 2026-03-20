using System;
using Furby;
using UnityEngine;

namespace Relentless
{
	public class DeviceSettings : RelentlessMonoBehaviour
	{
		public GameObject m_AndroidDatabase;

		public GameObject m_iPhoneDatabase;

		public DeviceProperties m_DefaultEditor = new DeviceProperties();

		public DeviceProperties m_DefaultAndroid = new DeviceProperties();

		public DeviceProperties m_DefaultIDevice = new DeviceProperties();

		private DeviceRegister m_DeviceRegister;

		private DeviceProperties m_CurrentDevice;

		public bool m_DeviceIsKnown;

		private GameEventSubscription m_DebugPanelSub;

		public bool DeviceIsKnown
		{
			get
			{
				return m_DeviceIsKnown;
			}
		}

		public DeviceProperties DeviceProperties
		{
			get
			{
				if (m_CurrentDevice == null)
				{
					GetDeviceProperties();
				}
				return m_CurrentDevice;
			}
		}

		public void Start()
		{
			ExtractRegisterFromRelevantDatabase();
			GetDeviceProperties();
			ApplyApplicationProperties();
		}

		private void DEBUG_LogDeviceSettings()
		{
			if (m_CurrentDevice != null)
			{
				Logging.Log("----------------------------------------------------------");
				Logging.Log("Relentless.DeviceSettings for device (" + m_CurrentDevice.m_DisplayName + ")");
				Logging.Log("----------------------------------------------------------");
				Logging.Log("Application, ComAir volume = " + m_CurrentDevice.m_ApplicationModifiers.m_ComAirVolume);
				Logging.Log("Application, Resolution Mult. = " + m_CurrentDevice.m_ApplicationModifiers.m_ResolutionMultiplier);
				Logging.Log("Application, Orientation: " + GetAppropriateScreenOrientation(m_CurrentDevice));
				Logging.Log("----------------------------------------------------------");
			}
		}

		private void ExtractRegisterFromRelevantDatabase()
		{
			Logging.Log("Relentless.DeviceSettings <ANDROID>");
			m_DeviceRegister = m_AndroidDatabase.GetComponent<DeviceRegister>();
		}

		public static ScreenOrientation GetAppropriateScreenOrientation(DeviceProperties deviceProps)
		{
			if (deviceProps.m_ApplicationModifiers.m_Orientation == DeviceOrientation.Portrait)
			{
				return ScreenOrientation.Portrait;
			}
			if (deviceProps.m_ApplicationModifiers.m_Orientation == DeviceOrientation.PortraitUpsideDown)
			{
				return ScreenOrientation.PortraitUpsideDown;
			}
			return ScreenOrientation.Portrait;
		}

		private void ApplyApplicationProperties()
		{
			if (m_CurrentDevice != null)
			{
				Screen.SetResolution((int)((float)Screen.width * m_CurrentDevice.m_ApplicationModifiers.m_ResolutionMultiplier), (int)((float)Screen.height * m_CurrentDevice.m_ApplicationModifiers.m_ResolutionMultiplier), true);
				if (!Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel_UserCustomized)
				{
					Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel = m_CurrentDevice.m_ApplicationModifiers.m_ComAirVolume;
				}
				Singleton<GameDataStoreObject>.Instance.GlobalData.ApplyCommsLevel();
			}
		}

		public ScreenOrientation GetScreenOrientationForDevice()
		{
			return GetAppropriateScreenOrientation(m_CurrentDevice);
		}

		public void GetDeviceProperties()
		{
			if (m_CurrentDevice == null)
			{
				string text = SystemInfo.deviceModel.ToLower();
				string text2 = SystemInfo.deviceName.ToLower();
				Logging.Log("Device Model: " + text);
				Logging.Log("Device Name: " + text2);
				m_CurrentDevice = m_DeviceRegister.GetAndroidPropertiesFromDeviceModel(text);
				if (m_CurrentDevice == null)
				{
					m_CurrentDevice = m_DefaultAndroid;
					m_DeviceIsKnown = false;
				}
				else
				{
					m_DeviceIsKnown = true;
				}
			}
			Logging.Log("Relentless.DeviceSettings <ANDROID> Chose device: " + m_CurrentDevice.m_DeviceIdentifier);
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
		}

		private void OnInspectProperty(string title, string body)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(title + ":  ", RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(false));
			GUILayout.Label(" " + body, RelentlessGUIStyles.Style_Normal, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Device Settings"))
			{
				GUILayout.BeginVertical();
				GUILayout.Label("[System Info - Unity]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
				OnInspectProperty("Device Model", SystemInfo.deviceModel);
				OnInspectProperty("Device Name", SystemInfo.deviceName);
				if (m_CurrentDevice != null)
				{
					GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
					GUILayout.Label("[System Info - Furby]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
					GUILayout.BeginHorizontal();
					GUILayout.Label("Status:  ", RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(false));
					GUILayout.Label("Device ", RelentlessGUIStyles.Style_Normal, GUILayout.ExpandWidth(false));
					GUILayout.Label((!FurbyGlobals.DeviceSettings.DeviceIsKnown) ? " IS NOT " : " IS ", RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(false));
					GUILayout.Label(" recognized.", RelentlessGUIStyles.Style_Normal, GUILayout.ExpandWidth(false));
					GUILayout.EndHorizontal();
					OnInspectProperty("Device Family", FurbyGlobals.DeviceSettings.DeviceProperties.m_DisplayName);
					GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
					GUILayout.Label("[Current Settings]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
					GUILayout.BeginHorizontal();
					GUILayout.Label("ComAir Volume");
					GUILayout.Label(Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel.ToString("N2"));
					Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel = GUILayout.HorizontalSlider(Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel, 0f, 1f);
					GUILayout.EndHorizontal();
					OnInspectProperty("Orientation", Screen.orientation.ToString());
					OnInspectProperty("Render", "W:" + Screen.width + ", H:" + Screen.height);
					GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
					GUILayout.Label("[Device Defaults]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
					OnInspectProperty("ComAir Volume", m_CurrentDevice.m_ApplicationModifiers.m_ComAirVolume.ToString());
					bool screenOrientation_UserCustomized = Singleton<GameDataStoreObject>.Instance.GlobalData.m_ScreenOrientation_UserCustomized;
					ScreenOrientation customizedScreenOrientation = Singleton<GameDataStoreObject>.Instance.GlobalData.GetCustomizedScreenOrientation();
					ScreenOrientation screenOrientationForDevice = GetScreenOrientationForDevice();
					if (screenOrientation_UserCustomized)
					{
						OnInspectProperty("Orientation (Override!)", customizedScreenOrientation.ToString());
					}
					else
					{
						OnInspectProperty("Orientation", screenOrientationForDevice.ToString());
					}
					OnInspectProperty("Resolution", "x" + m_CurrentDevice.m_ApplicationModifiers.m_ResolutionMultiplier);
					GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
					GUILayout.Label("[Controls]", RelentlessGUIStyles.Style_Header, GUILayout.ExpandWidth(true));
					GUILayout.BeginHorizontal();
					GUILayout.Label("Orientation: ", RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(false));
					Color color = new Color(0.25f, 0.88f, 0.25f);
					Color white = Color.white;
					bool flag = screenOrientationForDevice == ScreenOrientation.Portrait;
					GUI.backgroundColor = ((!flag) ? white : color);
					if (GUILayout.Button("Portrait", GUILayout.Width(150f)))
					{
						Screen.orientation = ScreenOrientation.Portrait;
					}
					GUI.backgroundColor = (flag ? white : color);
					if (GUILayout.Button("UpsideDown", GUILayout.Width(150f)))
					{
						Screen.orientation = ScreenOrientation.PortraitUpsideDown;
					}
					GUI.backgroundColor = Color.white;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
		}
	}
}
