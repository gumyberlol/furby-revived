using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Translator
{
	public class FurbishTranslator : MonoBehaviour
	{
		private int m_EntryOrdinal = 65536;

		[SerializeField]
		private UIGrid m_ListView;

		[SerializeField]
		private GameObject m_EntryPrefab;

		private int m_entryIndex;

		[SerializeField]
		private float m_ConnectionAttemptDuration = 12f;

		public UIDraggablePanel m_draggablePanel;

		private GameEventSubscription m_debugPanelSubscription;

		public void Start()
		{
			Singleton<FurbyDataChannel>.Instance.ToneEvent += OnTranslatorTone;
			Singleton<FurbyDataChannel>.Instance.DisconnectDetection = false;
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Translator);
			m_debugPanelSubscription = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
			StartCoroutine(Execute());
		}

		public void OnDestroy()
		{
			Singleton<FurbyDataChannel>.Instance.ToneEvent -= OnTranslatorTone;
			Singleton<FurbyDataChannel>.Instance.DisconnectDetection = true;
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			m_debugPanelSubscription.Dispose();
			StopAllCoroutines();
		}

		private IEnumerator Execute()
		{
			while (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
			{
				GameEventRouter.SendEvent(FurbishTranslatorEvent.TranslatorOpened);
				yield return StartCoroutine(ConnectionFlow(m_ConnectionAttemptDuration));
				if (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
				{
					yield return StartCoroutine(ConnectionErrorDialogFlow());
				}
			}
			Singleton<FurbyDataChannel>.Instance.PostAction(FurbyAction.F2F_Greeting, null);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(false);
			GameEventRouter.SendEvent(FurbishTranslatorEvent.Synchronized);
		}

		private IEnumerator ConnectionFlow(float attemptDuration)
		{
			float abortTime = attemptDuration + Time.time;
			while (!Singleton<FurbyDataChannel>.Instance.FurbyConnected && abortTime > Time.time)
			{
				yield return null;
			}
		}

		private IEnumerator ConnectionErrorDialogFlow()
		{
			WaitForGameEvent eventWaiter = new WaitForGameEvent();
			Type translatorEvents = typeof(FurbishTranslatorEvent);
			IEnumerator enumerator = eventWaiter.WaitForAnyEventOfType(translatorEvents);
			GameEventRouter.SendEvent(FurbishTranslatorEvent.ConnectionFailed);
			while (enumerator.MoveNext())
			{
				if (Singleton<FurbyDataChannel>.Instance.FurbyConnected)
				{
					GameEventRouter.SendEvent(FurbishTranslatorEvent.ConnectionRetry);
					yield break;
				}
				yield return null;
			}
			if ((FurbishTranslatorEvent)(object)eventWaiter.ReturnedEvent == FurbishTranslatorEvent.ConnectionAbort)
			{
				StopAllCoroutines();
				FurbyGlobals.ScreenSwitcher.BackScreen();
			}
		}

		private void OnTranslatorTone(FurbyMessageType msgType, long msgTone)
		{
			if (msgType == FurbyMessageType.Translator)
			{
				string text = string.Format("TRANSLATOR_FURBISH_{0:0000}", msgTone);
				string text2 = string.Format("TRANSLATOR_ENGLISH_{0:0000}", msgTone);
				string text3 = Singleton<Localisation>.Instance.GetText(text);
				string text4 = Singleton<Localisation>.Instance.GetText(text2);
				Logging.Log(string.Format("{0} = {1}", text, text3));
				if (text3 != text && text4 != text2)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_EntryPrefab);
					UILabel uILabel = gameObject.FindComponent<UILabel>("RightQuoteLabel");
					UILabel uILabel2 = gameObject.FindComponent<UILabel>("LeftQuoteLabel");
					gameObject.name = string.Format("Entry {0}", m_EntryOrdinal);
					m_EntryOrdinal--;
					gameObject.transform.parent = m_ListView.transform;
					gameObject.transform.localScale = Vector3.one;
					uILabel.text = text4;
					uILabel2.text = text3;
					GameEventRouter.SendEvent(FurbishTranslatorEvent.Translated);
					m_ListView.Reposition();
					m_draggablePanel.ResetPosition();
					m_entryIndex++;
				}
			}
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (GUILayout.Button("Generate Random Translator Tone"))
			{
				OnDebugFakeTone();
			}
		}

		private void OnDebugFakeTone()
		{
			string format = "TRANSLATOR_FURBISH_{0:0000}";
			int num;
			string text;
			string text2;
			do
			{
				num = UnityEngine.Random.Range(0, 699);
				text = string.Format(format, num);
				text2 = Singleton<Localisation>.Instance.GetText(text);
			}
			while (!(text != text2));
			OnTranslatorTone(FurbyMessageType.Translator, num);
		}
	}
}
