using System;
using System.Collections;
using System.Linq;
using Fabric;
using Furby;
using Relentless;
using UnityEngine;

public class HardwareSettingsScreenFlow : MonoBehaviour
{
    private const float POLL_WAIT_TIME_SECONDS = 1f;

    private static AndroidJavaClass s_HardwareSettingsJava;

    [SerializeField]
    private HardwareVolumeMeter m_HardwareVolumeMeter;

    [SerializeField]
    private UISlider m_HardwareVolumeSlider;

    [SerializeField]
    private VolumeSlider m_InGameVolumeSlider;

    private float m_InitialHardwareVolume = 1f;

    [SerializeField]
    private float m_HardwareVolumeTolerance = 0.99f;

    [SerializeField]
    private GameObject m_HeadphonesInPanel;

    [SerializeField]
    private GameObject m_VolumeMeterPanel;

    [SerializeField]
    private float m_PanelScaleTime = 0.5f;

    private readonly Vector3 MIN_PANEL_SCALE_VECTOR =
        new Vector3(0.01f, 0.01f, 0.01f);

    private readonly Vector3 MAX_PANEL_SCALE_VECTOR =
        new Vector3(1f, 1f, 1f);

    private UICamera[] m_DisabledCameras;

    private static float s_FakeHardwareVolume = 1f;

    private static bool s_FakeHeadphonesIn;

    private bool m_IsTemporarilyDisabled;

    private GameEventSubscription m_DebugPanelSub;

    private bool m_FakeHardwareTriggered;

    public void Disable()
    {
        m_IsTemporarilyDisabled = true;
    }

    public void Enable()
    {
        m_IsTemporarilyDisabled = false;
    }

    private void OnEnable()
    {
        m_DebugPanelSub = new GameEventSubscription(
            OnDebugPanelGUI,
            DebugPanelEvent.DrawElementRequested);
    }

    private void OnDestroy()
    {
        m_DebugPanelSub.Dispose();
    }

    private void OnDebugPanelGUI(Enum evtType,
        GameObject gObj, params object[] parameters)
    {
        if (DebugPanel.StartSection(
            "Hardware Setting Popups"))
        {
            if (m_FakeHardwareTriggered)
            {
                GUILayout.Label(
                    "Close the debug menu!");
            }
            else
            {
                GUILayout.Label(
                    "Note: Triggered after 2 secs!");
                if (GUILayout.Button(
                    "Simulate Volume Change"))
                {
                    m_FakeHardwareTriggered = true;
                    Invoke("ActivateFakeVolumeTrigger",
                        3f);
                }
                if (GUILayout.Button(
                    "Simulate Headphones"))
                {
                    m_FakeHardwareTriggered = true;
                    Invoke("ActivateFakeHeadphones",
                        3f);
                }
            }
        }
        DebugPanel.EndSection();
    }

    private void ActivateFakeVolumeTrigger()
    {
        s_FakeHardwareVolume = 0.5f;
        m_FakeHardwareTriggered = false;
    }

    private void ActivateFakeHeadphones()
    {
        s_FakeHeadphonesIn = true;
        m_FakeHardwareTriggered = false;
    }

    private float GetHardwareVolume()
    {
        // always return max!!
        // ComAir is dead!!
        // volume = irrelevant!!
        return 1f;
    }

    private void SetHardwareVolume(float volume)
    {
        if (Application.platform ==
            RuntimePlatform.Android)
        {
            s_HardwareSettingsJava.CallStatic(
                "_setHardwareVolume", volume);
        }
        else
        {
            s_FakeHardwareVolume = volume;
        }
    }

    private bool AreHeadphonesPluggedIn()
    {
        // always false!!
        // don't care!!
        return false;
    }

    private void HidePanelImmediately(
        GameObject panel)
    {
        panel.transform.localScale =
            MIN_PANEL_SCALE_VECTOR;
        panel.SetActive(false);
    }

    private void ShowPanelImmediately(
        GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale =
            MAX_PANEL_SCALE_VECTOR;
    }

    private void Start()
    {
        // disable popups forever!!
        // ComAir is dead!!
        // nobody cares!!
		HidePanelImmediately(m_HeadphonesInPanel);
		HidePanelImmediately(m_VolumeMeterPanel);
        m_IsTemporarilyDisabled = true;

        m_InitialHardwareVolume = GetHardwareVolume();
        SetIsControllingGlobalInGameVolume(true);
        HidePanelImmediately(m_HeadphonesInPanel);
        HidePanelImmediately(m_VolumeMeterPanel);

        // these coroutines = stubbed!!
        // popups = dead!!
        // goodbye!!
    }

    public void SetIsControllingGlobalInGameVolume(
        bool isControllingGlobalVolume)
    {
        m_InGameVolumeSlider
            .SetIsControllingGlobalInGameVolume(
            isControllingGlobalVolume);
    }

    private IEnumerator PollHeadphoneStatus()
    {
        // stubbed!!
        // headphones = don't care!!
        yield break;
    }

    private IEnumerator PollHardwareVolume()
    {
        // stubbed!!
        // volume = always max!!
        // ComAir = dead!!
        yield break;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        EventManager.Instance.PostEvent(
            "pause", EventAction.PauseSound);
        m_DisabledCameras = (
            from camera in Camera.allCameras
            where camera.gameObject.layer !=
                base.gameObject.layer
            let uicamera =
                camera.GetComponent<UICamera>()
            where uicamera != null
            where uicamera.enabled
            select uicamera).ToArray();
        if (m_DisabledCameras == null) return;
        UICamera[] disabledCameras = m_DisabledCameras;
        foreach (UICamera uICamera in disabledCameras)
        {
            if (uICamera != null)
                uICamera.enabled = false;
        }
    }

    private void UnpauseGame()
    {
        if (m_DisabledCameras != null)
        {
            UICamera[] disabledCameras =
                m_DisabledCameras;
            foreach (UICamera uICamera
                in disabledCameras)
            {
                if (uICamera != null)
                    uICamera.enabled = true;
            }
        }
        Time.timeScale = 1f;
        EventManager.Instance.PostEvent(
            "pause", EventAction.UnpauseSound);
    }

    private bool GameIsAlreadyPaused()
    {
        return Time.timeScale > 0f;
    }

    private bool GameIsLoading()
    {
        return Application.isLoadingLevel ||
            AssetBundleHelpers.IsLoading();
    }

    private IEnumerator
        WaitForUnacceptableHardwareStatus()
    {
        // fuck you get stubbed idiot!!
        yield break;
    }

    private IEnumerator
        WaitForAcceptableHeadphoneStatus()
    {
        // stubbed!!
        // headphones = never plugged in!!
        // popup = never shows!!
        yield break;
    }

    private IEnumerator
        WaitForAcceptableHardwareVolume()
    {
        // stubbed!!
        // volume = always max!!
        // popup = never shows!!
        yield break;
    }

    public void UpdateMeterFromHardwareVolume()
    {
        float hardwareVolume = GetHardwareVolume();
        if (hardwareVolume !=
            m_HardwareVolumeSlider.sliderValue)
        {
            m_HardwareVolumeSlider.sliderValue =
                hardwareVolume;
        }
    }

    public void UpdateHardwareVolumeFromMeter()
    {
        float hardwareVolume = GetHardwareVolume();
        if (hardwareVolume !=
            m_HardwareVolumeSlider.sliderValue)
        {
            SetHardwareVolume(
                m_HardwareVolumeSlider.sliderValue);
        }
    }
}