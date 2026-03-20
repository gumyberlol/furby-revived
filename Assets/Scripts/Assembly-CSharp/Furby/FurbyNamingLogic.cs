using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyNamingLogic : GameEventReceiver
	{
		private FurbyNamingScreenSetup m_namingScreenSetup;

		[SerializeField]
		private float m_timeOut = 6f;

		[SerializeField]
		private float m_holdStillToSayNameTime = 1f;

		private GameEventSubscription m_debugPanelSub;

		private bool m_forceSuccess;

		private string m_previousName;

		private float m_heldStillTimer;

		private bool m_hasChangedNameYet;

		private bool m_isSpeaking;

		private bool m_hasSaidCurrentName;

		private bool m_namingIsInProgress;

		public override Type EventType
		{
			get
			{
				return typeof(FurbyNameGameEvent);
			}
		}

		private void Start()
		{
			m_namingScreenSetup = (FurbyNamingScreenSetup)UnityEngine.Object.FindObjectOfType(typeof(FurbyNamingScreenSetup));
			m_previousName = m_namingScreenSetup.GetCurrentName();
		}

		private void Update()
		{
			if (!m_namingIsInProgress)
			{
				m_heldStillTimer += Time.deltaTime;
				string currentName = m_namingScreenSetup.GetCurrentName();
				if (currentName != m_previousName)
				{
					m_heldStillTimer = 0f;
					m_previousName = currentName;
					m_hasChangedNameYet = true;
					m_hasSaidCurrentName = false;
				}
				if (m_hasChangedNameYet && m_heldStillTimer > m_holdStillToSayNameTime && !m_hasSaidCurrentName && !m_isSpeaking)
				{
					StartCoroutine(SpeakName());
				}
			}
		}

		private IEnumerator SpeakName()
		{
			m_isSpeaking = true;
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(false);
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(FurbyCommand.Name, null))
			{
				yield return null;
			}
			yield return new WaitForSeconds(1f);
			while (true)
			{
				string enumString = m_namingScreenSetup.GetCurrentName();
				FurbyTransmitName furbyNameVal = EnumExtensions.Parse<FurbyTransmitName>(enumString);
				if (m_namingScreenSetup.IsNameAllowed() && Singleton<FurbyDataChannel>.Instance.PostName(furbyNameVal, null))
				{
					break;
				}
				yield return null;
			}
			m_hasSaidCurrentName = true;
			m_isSpeaking = false;
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			m_debugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		protected override void OnDisable()
		{
			m_debugPanelSub.Dispose();
			base.OnDisable();
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((FurbyNameGameEvent)(object)enumValue)
			{
			case FurbyNameGameEvent.NamingScreenConfirmName:
				if (m_namingScreenSetup.IsNameAllowed())
				{
					m_namingIsInProgress = true;
					StopCoroutine("SpeakName");
					StartCoroutine(DoFurbyNaming());
				}
				else
				{
					GameEventRouter.SendEvent(FurbyNameGameEvent.NamingScreenFurbyNameDidntSend);
				}
				break;
			case FurbyNameGameEvent.NamingScreenFurbyConfirmsNameSent:
				Singleton<GameDataStoreObject>.Instance.Data.HasCompletedFirstTimeFlow = true;
				m_namingScreenSetup.SaveName();
				GameEventRouter.SendEvent(FurbyNameGameEvent.NamingScreenSaveGameUpdated);
				break;
			}
		}

		private void EndFurbyNaming(bool wasSuccessful)
		{
			m_namingIsInProgress = false;
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			if (wasSuccessful)
			{
				GameEventRouter.SendEvent(FurbyNameGameEvent.NamingScreenFurbyConfirmsNameSent);
			}
			else
			{
				GameEventRouter.SendEvent(FurbyNameGameEvent.NamingScreenFurbyNameDidntSend);
			}
		}

		private IEnumerator WaitProgressSlider(float endTarget, float time)
		{
			for (float timeDelay = 0f; timeDelay < time; timeDelay += Time.deltaTime)
			{
				yield return null;
			}
		}

		private IEnumerator DoFurbyNaming()
		{
			string enumString = m_namingScreenSetup.GetCurrentName();
			FurbyTransmitName furbyNameVal = EnumExtensions.Parse<FurbyTransmitName>(enumString);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(false);
			Invoke(TimeOutFunction, m_timeOut);
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(FurbyCommand.Name, null))
			{
				yield return null;
			}
			CancelInvoke(TimeOutFunction);
			yield return StartCoroutine(WaitProgressSlider(0.1f, 2.5f));
			Invoke(TimeOutFunction, m_timeOut);
			while (!Singleton<FurbyDataChannel>.Instance.PostName(furbyNameVal, null))
			{
				yield return null;
			}
			CancelInvoke(TimeOutFunction);
			yield return StartCoroutine(WaitProgressSlider(0.2f, 2.5f));
			Invoke(TimeOutFunction, m_timeOut);
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(FurbyCommand.ConfirmName, null))
			{
				yield return null;
			}
			CancelInvoke(TimeOutFunction);
			yield return StartCoroutine(WaitProgressSlider(0.3f, 2.5f));
			Invoke(TimeOutFunction, m_timeOut);
			while (!Singleton<FurbyDataChannel>.Instance.PostName(furbyNameVal, null))
			{
				yield return null;
			}
			CancelInvoke(TimeOutFunction);
			yield return StartCoroutine(WaitProgressSlider(0.4f, 4.5f));
			Invoke(TimeOutFunction, m_timeOut);
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(FurbyCommand.Status, null))
			{
				yield return null;
			}
			CancelInvoke(TimeOutFunction);
			GameEventRouter.AddDelegateForType(typeof(FurbyDataEvent), RecieveDataEvent);
			Invoke(TimeOutFunction, m_timeOut);
			yield return StartCoroutine(WaitProgressSlider(1f, 1f));
		}

		private void RecieveDataEvent(Enum eventType, GameObject gObj, params object[] paramlist)
		{
			FurbyDataEvent furbyDataEvent = (FurbyDataEvent)(object)eventType;
			if (furbyDataEvent == FurbyDataEvent.FurbyDataReceived)
			{
				CancelInvoke(TimeOutFunction);
				GameEventRouter.RemoveDelegateForType(typeof(FurbyDataEvent), RecieveDataEvent);
				string currentName = m_namingScreenSetup.GetCurrentName();
				FurbyReceiveName furbyReceiveName = EnumExtensions.Parse<FurbyReceiveName>(currentName);
				FurbyStatus furbyStatus = (FurbyStatus)paramlist[0];
				Logging.Log(string.Concat("Furby says its name is ", furbyStatus.Name, " and we are expecting: ", furbyReceiveName));
				if (furbyStatus.Name == furbyReceiveName)
				{
					EndFurbyNaming(true);
				}
				else
				{
					EndFurbyNaming(false);
				}
			}
		}

		public void TimeOutFunction()
		{
			StopAllCoroutines();
			EndFurbyNaming(false || m_forceSuccess);
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Furby Naming"))
			{
				m_forceSuccess = GUILayout.Toggle(m_forceSuccess, new GUIContent("Fake naming success"));
			}
			DebugPanel.EndSection();
		}
	}
}
