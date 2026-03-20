using System;
using UnityEngine;

namespace Relentless
{
	public class VideoCamera : MonoBehaviour
	{
		public bool PreferFrontFacing = true;

		public int Width = 384;

		public int Height = 288;

		public int FPS = 30;

		private WebCamTexture m_webCamTexture;

		private bool m_isCapturing;

		private bool m_isPaused;

		private GameEventSubscription m_debugPanelSub;

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(typeof(VideoCameraCommand), OnVideoCameraCommand);
			m_debugPanelSub = new GameEventSubscription(OnDebugGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDisable()
		{
			m_debugPanelSub.Dispose();
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(VideoCameraCommand), OnVideoCameraCommand);
			}
			StopCapture();
		}

		private void OnVideoCameraCommand(Enum eventCode, GameObject originator, params object[] parameters)
		{
			switch ((VideoCameraCommand)(object)eventCode)
			{
			case VideoCameraCommand.StartCapture:
				m_isCapturing = true;
				EnsureCaptureStarted();
				break;
			case VideoCameraCommand.StopCapture:
				StopCapture();
				break;
			case VideoCameraCommand.PauseCapture:
				PauseCapture();
				break;
			}
		}

		private void EnsureCaptureStarted()
		{
			if (m_webCamTexture == null)
			{
				if (WebCamTexture.devices.Length == 0)
				{
					base.gameObject.SendGameEvent(VideoCameraEvent.NoDetectedCamera);
					return;
				}
				WebCamDevice webCamDevice = WebCamTexture.devices[0];
				m_webCamTexture = new WebCamTexture(webCamDevice.name, Width, Height, FPS);
			}
			if (m_webCamTexture != null && !m_webCamTexture.isPlaying)
			{
				m_webCamTexture.Play();
			}
			m_isPaused = false;
		}

		private void StopCapture()
		{
			if (m_webCamTexture != null)
			{
				base.gameObject.SendGameEvent(VideoCameraEvent.FrameCaptured, new Texture2D(16, 16));
				m_webCamTexture.Stop();
			}
			m_webCamTexture = null;
			m_isCapturing = false;
		}

		private void PauseCapture()
		{
			if (m_webCamTexture != null)
			{
				m_webCamTexture.Pause();
				m_isPaused = true;
			}
		}

		private void Update()
		{
			if (m_isCapturing && !m_isPaused)
			{
				EnsureCaptureStarted();
				if (m_webCamTexture != null && m_webCamTexture.isPlaying && m_webCamTexture.didUpdateThisFrame)
				{
					base.gameObject.SendGameEvent(VideoCameraEvent.FrameCaptured, m_webCamTexture);
				}
			}
		}

		private void OnDebugGUI(Enum enumType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Camera Diagnostics"))
			{
				GUILayout.Label("Is Capturing : " + m_isCapturing);
				if (m_webCamTexture != null)
				{
					GUILayout.Label(m_webCamTexture.deviceName);
					GUILayout.Label(string.Format("Resolution : {0}x{1}", m_webCamTexture.width, m_webCamTexture.height));
				}
				else
				{
					WebCamDevice[] devices = WebCamTexture.devices;
					for (int i = 0; i < devices.Length; i++)
					{
						WebCamDevice webCamDevice = devices[i];
						GUILayout.BeginHorizontal();
						GUILayout.Label(webCamDevice.name);
						if (webCamDevice.isFrontFacing)
						{
							GUILayout.Label("(is front facing)");
						}
						GUILayout.EndHorizontal();
					}
					if (WebCamTexture.devices.Length == 0)
					{
						GUILayout.Label("No devices detected.");
					}
				}
			}
			DebugPanel.EndSection();
		}
	}
}
