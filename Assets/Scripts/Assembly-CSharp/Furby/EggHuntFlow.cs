using System;
using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class EggHuntFlow : MonoBehaviour
	{
		[SerializeField]
		private float m_scanTimeout = 7f;

		[SerializeField]
		private string m_failedTimeoutDialogName = "FailedTimeout";

		[SerializeField]
		private string m_failedWrongQRDialogName = "FailedUnrecognisedQRCode";

		[SerializeField]
		private string m_failedWrongEggDialogName = "FailedUnrecognisedEgg";

		[SerializeField]
		private BabyInstance m_eggInstance;

		[SerializeField]
		private LevelReference m_successScreen;

		[SerializeField]
		private GameObject m_qrCode;

		[SerializeField]
		private GameObject m_eggNode;

		[SerializeField]
		private GameObject m_eggVfxNode;

		[SerializeField]
		private GameObject m_textBoxBackground;

		[SerializeField]
		private GameObject m_eggUnlockedBackground;

		[SerializeField]
		private GameObject[] m_scanNodes;

		[SerializeField]
		private float m_cornerScanPause = 0.15f;

		[SerializeField]
		private float m_scanningPause = 1f;

		private string m_decodedQR;

		private string m_fakeQRcode;

		private ProcessOnThread<UnityLuminanceSource, QrCodeDecode.DetectedQrCode> m_qrProcessorThread;

		private QrCodeDecode m_decoder;

		private QrCodeDecode.DetectedQrCode m_detectedQrCode;

		private GameEventSubscription m_debugPanelSubs;

		private Color32[] m_savedPixels;

		private int m_textureWidth;

		private int m_textureHeight;

		private Texture2D m_savedTexture;

		private IEnumerator Start()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			int babyIndex = -1;
			m_debugPanelSubs = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
			if (WebCamTexture.devices.Length == 0)
			{
				yield return new WaitForSeconds(1.5f);
				GameEventRouter.SendEvent(EggHuntEvent.ScanFailedCameraNotFound);
				yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
				FurbyGlobals.ScreenSwitcher.BackScreen();
				yield break;
			}
			try
			{
				GameEventRouter.SendEvent(VideoCameraCommand.StartCapture);
				GameEventRouter.SendEvent(EggHuntEvent.ScanStarted);
				m_decodedQR = null;
				yield return StartCoroutine(ScanForQRCode());
				bool m_timedout = false;
				bool m_wrongQR = false;
				if (string.IsNullOrEmpty(m_decodedQR))
				{
					m_timedout = true;
				}
				else if (!IsValidEggURL(m_decodedQR))
				{
					m_wrongQR = true;
				}
				else
				{
					string code = m_decodedQR.Substring(m_decodedQR.Length - 5);
					babyIndex = FurbyGlobals.BabyLibrary.TypeList.FindIndex((FurbyBabyTypeInfo x) => x.Code == code);
					Logging.Log("Code " + code);
				}
				if (m_timedout)
				{
					GameEventRouter.SendEvent(EggHuntEvent.ScanFailed);
					GameEventRouter.SendEvent(SharedGuiEvents.DialogShow, base.gameObject, m_failedTimeoutDialogName);
					yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
					if (!waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
					{
						GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
						FurbyGlobals.ScreenSwitcher.SwitchScreen("EggHunt", false);
						yield break;
					}
					FurbyGlobals.ScreenSwitcher.BackScreen();
				}
				else
				{
					GameEventRouter.SendEvent(VideoCameraCommand.PauseCapture);
					GameEventRouter.SendEvent(EggHuntEvent.ScanFakeScanStarted);
					yield return StartCoroutine(FakeQRScanning());
					if (babyIndex == -1)
					{
						GameEventRouter.SendEvent(EggHuntEvent.ScanFailed);
						if (m_wrongQR)
						{
							GameEventRouter.SendEvent(SharedGuiEvents.DialogShow, base.gameObject, m_failedWrongQRDialogName);
						}
						else
						{
							GameEventRouter.SendEvent(SharedGuiEvents.DialogShow, base.gameObject, m_failedWrongEggDialogName);
						}
						yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
						if (!waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
						{
							GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
							FurbyGlobals.ScreenSwitcher.SwitchScreen("EggHunt", false);
							yield break;
						}
						FurbyGlobals.ScreenSwitcher.BackScreen();
					}
					else if (FurbyGlobals.BabyRepositoryHelpers.AllFurblings.Any((FurbyBaby x) => x.Type.Equals(FurbyGlobals.BabyLibrary.TypeList[babyIndex].TypeID)))
					{
						GameEventRouter.SendEvent(EggHuntEvent.ScanFailed);
						GameEventRouter.SendEvent(EggHuntEvent.ScanFailedAlreadyHaveEgg);
						GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
						yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept));
						FurbyGlobals.ScreenSwitcher.BackScreen();
						yield break;
					}
				}
			}
			finally
			{
				m_debugPanelSubs.Dispose();
				GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
			}
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(FurbyGlobals.BabyLibrary.TypeList[babyIndex].TypeID);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			m_eggInstance.SetTargetFurbyBaby(furbyBaby);
			GameEventRouter.SendEvent(BabyLifecycleEvent.FromQRCode, null, furbyBaby);
			GameEventRouter.SendEvent(EggHuntEvent.ScanSucceeded);
			yield return StartCoroutine(PresentEgg());
			while (m_eggNode.animation.isPlaying)
			{
				yield return null;
			}
			GameEventRouter.SendEvent(EggHuntEvent.PresentationFinished);
			yield return StartCoroutine(waiter.WaitForEvent(EggHuntEvent.ButtonContinue));
			FurbyGlobals.ScreenSwitcher.SwitchScreen(m_successScreen);
		}

		private bool IsValidEggURL(string url)
		{
			for (int i = -5; i < 0; i++)
			{
				int num = url.Length + i;
				if (num < 0)
				{
					return false;
				}
				if (url[num] < 'A' || url[num] > 'Z')
				{
					return false;
				}
			}
			return true;
		}

		private IEnumerator PresentEgg()
		{
			Texture2D white = new Texture2D(1, 1);
			white.SetPixel(0, 0, Color.white);
			white.Apply();
			Rect screenRect = Camera.allCameras[0].rect;
			screenRect.x *= Screen.width;
			screenRect.width *= Screen.width;
			screenRect.y *= Screen.height;
			screenRect.height *= Screen.height;
			Rect textRect = new Rect(0f, 0f, 1f, 1f);
			yield return new WaitForEndOfFrame();
			Graphics.DrawTexture(screenRect, white, textRect, 1, 1, 1, 1, Color.white);
			m_textBoxBackground.SetActive(false);
			ExtractQRCodeFromTexture();
			m_qrCode.renderer.material.SetTexture("_MainTex", m_savedTexture);
			m_eggNode.SetActive(true);
			m_eggNode.animation.Play();
			m_eggVfxNode.animation.Play();
			StartCoroutine(ShowEggUnlockedBackground());
			float colorValue = 0.5f;
			while (colorValue > 0f)
			{
				yield return new WaitForEndOfFrame();
				colorValue = Mathf.Clamp01(colorValue - Time.deltaTime / 2f);
				Graphics.DrawTexture(color: new Color(1f, 1f, 1f, colorValue * 2f), screenRect: screenRect, texture: white, sourceRect: textRect, leftBorder: 1, rightBorder: 1, topBorder: 1, bottomBorder: 1);
			}
		}

		private IEnumerator ShowEggUnlockedBackground()
		{
			yield return new WaitForSeconds(4f);
			m_eggUnlockedBackground.SetActive(true);
		}

		private IEnumerator FakeQRScanning()
		{
			GameObject[] scanNodes = m_scanNodes;
			foreach (GameObject gObj in scanNodes)
			{
				gObj.SetActive(false);
			}
			int index = 0;
			float xScale = 600f / (float)m_textureWidth;
			float yScale = 450f / (float)m_textureHeight;
			GameObject[] scanNodes2 = m_scanNodes;
			foreach (GameObject gObj2 in scanNodes2)
			{
				if (index < m_detectedQrCode.CornerPoints.Length)
				{
					gObj2.transform.localPosition = new Vector3(m_detectedQrCode.CornerPoints[index].x * xScale, (0f - m_detectedQrCode.CornerPoints[index].y) * yScale, 0f);
					gObj2.SetActive(true);
					gObj2.animation.Play();
				}
				else
				{
					gObj2.SetActive(false);
				}
				index++;
				yield return new WaitForSeconds(m_cornerScanPause);
			}
			yield return new WaitForSeconds(m_scanningPause);
		}

		private IEnumerator ScanForQRCode()
		{
			m_decodedQR = null;
			m_decoder = new QrCodeDecode();
			float timeout = m_scanTimeout;
			using (new GameEventSubscription(typeof(VideoCameraEvent), OnReceivedTexture))
			{
				using (m_qrProcessorThread = new ProcessOnThread<UnityLuminanceSource, QrCodeDecode.DetectedQrCode>(ReadQrCodeFromTexture))
				{
					m_qrProcessorThread.Start();
					do
					{
						IL_0228:
						if (m_qrProcessorThread.GetResult(out m_detectedQrCode))
						{
							continue;
						}
						if (m_fakeQRcode != null)
						{
							m_detectedQrCode = new QrCodeDecode.DetectedQrCode();
							m_detectedQrCode.CornerPoints = new Vector2[4];
							m_detectedQrCode.CornerPoints[0] = new Vector2(100f, 100f);
							m_detectedQrCode.CornerPoints[1] = new Vector2(200f, 100f);
							m_detectedQrCode.CornerPoints[2] = new Vector2(200f, 200f);
							m_detectedQrCode.CornerPoints[3] = new Vector2(100f, 200f);
							m_detectedQrCode.DecodeStage = QrCodeDecode.DecodeStage.kCodeDecoded;
							m_detectedQrCode.Text = "http://www.hasbro.com/furby/eggs/" + m_fakeQRcode;
							continue;
						}
						timeout -= Time.deltaTime;
						if (timeout <= 0f)
						{
							yield break;
						}
						yield return null;
						goto IL_0228;
					}
					while (m_detectedQrCode.DecodeStage != QrCodeDecode.DecodeStage.kCodeDecoded);
				}
			}
			m_decodedQR = m_detectedQrCode.Text;
		}

		private void ExtractQRCodeFromTexture()
		{
			Bounds bounds = new Bounds(m_detectedQrCode.CornerPoints[0], Vector3.zero);
			Vector2[] cornerPoints = m_detectedQrCode.CornerPoints;
			foreach (Vector2 vector in cornerPoints)
			{
				Logging.Log(vector);
				bounds.Encapsulate(vector);
			}
			bounds.size *= 1.5f;
			int num = Mathf.Clamp((int)bounds.size.x, 1, m_textureWidth);
			int num2 = Mathf.Clamp((int)bounds.size.y, 1, m_textureHeight);
			int num3 = Mathf.Clamp((int)bounds.min.x, 0, m_textureWidth - num);
			int num4 = Mathf.Clamp((int)bounds.min.y, 0, m_textureHeight - num2);
			Logging.Log(string.Format("{0} x {1}", num, num2));
			Color32[] array = new Color32[num * num2];
			for (int j = 0; j < num2; j++)
			{
				int num5 = m_textureHeight - 1 - (j + num4);
				Array.Copy(m_savedPixels, num3 + num5 * m_textureWidth, array, j * num, num);
			}
			m_savedTexture = new Texture2D(num, num2, TextureFormat.RGBA32, false);
			m_savedTexture.SetPixels32(array);
			m_savedTexture.Apply(false);
		}

		private void OnReceivedTexture(Enum eventType, GameObject originator, params object[] parameters)
		{
			VideoCameraEvent videoCameraEvent = (VideoCameraEvent)(object)eventType;
			if (videoCameraEvent != VideoCameraEvent.FrameCaptured || m_qrProcessorThread.WaitingCount != 0)
			{
				return;
			}
			WebCamTexture webCamTexture = parameters[0] as WebCamTexture;
			if (webCamTexture != null)
			{
				UnityLuminanceSource unityLuminanceSource = new UnityLuminanceSource(webCamTexture.width, webCamTexture.height);
				unityLuminanceSource.SetLuminances(webCamTexture);
				if (m_savedPixels == null || webCamTexture.GetPixels32().Length != m_savedPixels.Length)
				{
					m_savedPixels = new Color32[webCamTexture.GetPixels32().Length];
					m_textureWidth = webCamTexture.width;
					m_textureHeight = webCamTexture.height;
				}
				Array.Copy(webCamTexture.GetPixels32(), m_savedPixels, webCamTexture.GetPixels32().Length);
				m_qrProcessorThread.PushJob(unityLuminanceSource);
			}
		}

		private QrCodeDecode.DetectedQrCode ReadQrCodeFromTexture(UnityLuminanceSource source)
		{
			QrCodeDecode.DetectedQrCode detectedQrCode = null;
			try
			{
				m_decoder.SetLuminanceSource(source);
				return m_decoder.ExtractQrCode();
			}
			catch (Exception)
			{
				return new QrCodeDecode.DetectedQrCode();
			}
		}

		private void OnDebugPanelGUI(Enum evt, GameObject gObj, params object[] p)
		{
			if (DebugPanel.StartSection("Egg Hunt"))
			{
				if (GUILayout.Button("Fake scan Promo 1"))
				{
					m_fakeQRcode = "QKDIW";
				}
				if (m_detectedQrCode != null)
				{
					GUILayout.Label("QR Detection state: " + m_detectedQrCode.DecodeStage);
					GUILayout.Label("QR Detected Corners: " + m_detectedQrCode.CornerPoints.ToString());
					GUILayout.Label("QR Detected Text: " + m_detectedQrCode.Text);
				}
				else
				{
					GUILayout.Label("Not currently scanning.");
				}
			}
			DebugPanel.EndSection();
		}
	}
}
