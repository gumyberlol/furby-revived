using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class HideAndSeekGameFlow : MonoBehaviour
	{
		[Serializable]
		public class SpecialTryRange
		{
			public int m_MinTry;

			public int m_MaxTry;
		}

		[Serializable]
		public class LevelData
		{
			public int m_numTries = 8;

			public SpecialTryRange[] m_SpecialTriesRanges;

			public int m_extraTries = 4;

			public int m_loseTries = -2;

			public int m_chainReactionLength = 4;
		}

		public enum SpecialBalloonType
		{
			Unset = 0,
			AddTurns = 1,
			LoseTurns = 2,
			ChainReaction = 3,
			MoveBaby = 4
		}

		[SerializeField]
		private GameObject m_FurbyBaby;

		[SerializeField]
		private int m_numLevels = 3;

		[SerializeField]
		private Generate3DHideObjects m_objectGenerator;

		[SerializeField]
		private GameObject[] m_popReactions;

		[SerializeField]
		private GameObject m_popReactionNegative;

		[SerializeField]
		private GameObject m_popReactionNoFurbyMode;

		[SerializeField]
		private float m_popWaitTime = 2f;

		[SerializeField]
		private float m_hideWait = 3f;

		[SerializeField]
		private float m_announceWait = 3f;

		[SerializeField]
		private float m_shortWaitTime = 10f;

		[SerializeField]
		private float m_longWaitTime = 30f;

		[SerializeField]
		private bool m_showDialogBubbles;

		[SerializeField]
		private GameObject m_dialogBubbles;

		public LevelData[] m_levelData;

		[SerializeField]
		private float m_furbyRunSpeed = 2f;

		private int m_currentLevel;

		private GameEventSubscription m_roundEventsSub;

		private GameObject m_specialBalloon;

		public HintState m_HintStatePopBalloon = new HintState();

		private int m_turnsLeft;

		private int m_score;

		private int m_turnCount;

		private bool m_furbyFound;

		private HashSet<int> m_specialTurns;

		private GameEventSubscription m_debugPanelSubs;

		[SerializeField]
		private SpecialBalloonType[] m_specialBalloonRandomBag;

		private SpecialBalloonType m_nextSpecialBalloon;

		private bool m_forceSpecial;

		private float m_IdleModeTimer;

		private bool OutOfTurns
		{
			get
			{
				return TurnsLeft <= 0;
			}
		}

		private int TurnsLeft
		{
			get
			{
				return m_turnsLeft;
			}
			set
			{
				m_turnsLeft = value;
				GameEventRouter.SendEvent(HideAndSeekGameEvent.TurnsChanged, null, Mathf.Clamp(m_turnsLeft, 0, 100));
			}
		}

		private int Score
		{
			get
			{
				return m_score;
			}
			set
			{
				m_score = value;
				GameEventRouter.SendEvent(HideAndSeekGameEvent.ScoreChanged, null, m_score);
			}
		}

		private void Start()
		{
			ShowDialogBoxes(m_showDialogBubbles);
			StartCoroutine("MainGameFlow");
		}

		private void OnEnable()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			m_debugPanelSubs = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDisable()
		{
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			m_debugPanelSubs.Dispose();
		}

		private IEnumerator MainGameFlow()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			do
			{
				m_turnsLeft = 0;
				m_score = 0;
				Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(false);
				for (m_currentLevel = 0; m_currentLevel < m_numLevels; m_currentLevel++)
				{
					yield return StartCoroutine(PlayLevel(m_currentLevel));
					if (OutOfTurns)
					{
						break;
					}
				}
				Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
				if (OutOfTurns)
				{
					base.gameObject.SendGameEvent(HideAndSeekGameEvent.GameLost);
					base.gameObject.SendGameEvent(HideAndSeekGameEvent.ScoreShown);
					base.gameObject.SendGameEvent(BabyEndMinigameEvent.SetScore, m_score);
					base.gameObject.SendGameEvent(BabyEndMinigameEvent.ShowDialog);
					yield return StartCoroutine(waiter.WaitForEvent(BabyEndMinigameEvent.DialogFinished));
				}
				else
				{
					base.gameObject.SendGameEvent(HideAndSeekGameEvent.GameWon);
					base.gameObject.SendGameEvent(HideAndSeekGameEvent.ScoreShown);
					base.gameObject.SendGameEvent(BabyEndMinigameEvent.SetScore, m_score);
					base.gameObject.SendGameEvent(BabyEndMinigameEvent.ShowDialog);
					yield return StartCoroutine(waiter.WaitForEvent(BabyEndMinigameEvent.DialogFinished));
				}
			}
			while (!waiter.ReturnedEvent.Equals(BabyEndMinigameEvent.DialogFinished));
			GameEventRouter.SendEvent(SharedGuiEvents.Quit);
		}

		public void Update()
		{
			if (m_HintStatePopBalloon.IsEnabled())
			{
				m_HintStatePopBalloon.TestAndBroadcastState();
			}
		}

		private IEnumerator PlayLevel(int level)
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			GameEventRouter.SendEvent(HideAndSeekGameEvent.RoundStarted);
			TurnsLeft = m_levelData[m_currentLevel].m_numTries;
			m_turnCount = 0;
			m_specialTurns = new HashSet<int>();
			SpecialTryRange[] specialTriesRanges = m_levelData[m_currentLevel].m_SpecialTriesRanges;
			foreach (SpecialTryRange range in specialTriesRanges)
			{
				m_specialTurns.Add(UnityEngine.Random.Range(range.m_MinTry, range.m_MaxTry + 1));
			}
			yield return StartCoroutine(AnnounceRound());
			m_objectGenerator.GenerateObjects(m_currentLevel);
			try
			{
				yield return new WaitForSeconds(m_hideWait);
				yield return StartCoroutine(FurbyDiveSequence());
				base.gameObject.SendGameEvent(HideAndSeekGameEvent.RoundSetupComplete);
				m_furbyFound = false;
				while (!m_furbyFound && !OutOfTurns)
				{
					m_IdleModeTimer = 0f;
					Invoke("IdleTimeEvent", m_shortWaitTime);
					m_HintStatePopBalloon.Enable();
					yield return StartCoroutine(waiter.WaitForEvent(HideAndSeekGameEvent.BallonPoppedSpecial, HideAndSeekGameEvent.BalloonPopped));
					m_HintStatePopBalloon.Disable();
					CancelInvoke("IdleTimeEvent");
					m_turnCount++;
					yield return StartCoroutine(HandlePoppedBalloon(waiter.ReturnedGameObject));
				}
			}
			finally
			{
				m_FurbyBaby.SetLayerInChildren(26);
				m_objectGenerator.RemoveObjects();
			}
			GameEventRouter.SendEvent(HideAndSeekGameEvent.RoundOver);
			if (!OutOfTurns)
			{
				yield return StartCoroutine(ShowRoundScore());
			}
		}

		private IEnumerator ShowRoundScore()
		{
			int scoreThisTurn = TurnsLeft * 100;
			Score += scoreThisTurn;
			Vector3 direction = Vector3.right * ((!(m_FurbyBaby.transform.position.x < 0f)) ? 1f : (-1f));
			float time = m_announceWait;
			while (time > 0f)
			{
				m_FurbyBaby.transform.localPosition += direction * Time.deltaTime * m_furbyRunSpeed;
				time -= Time.deltaTime;
				yield return null;
			}
		}

		private IEnumerator AnnounceRound()
		{
			m_FurbyBaby.transform.localPosition = Vector3.zero;
			m_FurbyBaby.transform.localScale = Vector3.one;
			m_FurbyBaby.transform.localRotation = Quaternion.identity;
			base.gameObject.SendGameEvent(HideAndSeekGameEvent.RoundAnnounced);
			base.gameObject.SendGameEvent(SharedGuiEvents.DialogShow, "Round", m_currentLevel + 1);
			yield return new WaitForSeconds(m_announceWait);
			base.gameObject.SendGameEvent(SharedGuiEvents.DialogHide);
			base.gameObject.SendGameEvent(HideAndSeekGameEvent.RoundSetup);
		}

		private IEnumerator FurbyDiveSequence()
		{
			GameObject furbModel = m_FurbyBaby.GetComponentInChildren<ModelInstance>().Instance;
			if ((bool)furbModel.animation)
			{
				furbModel.animation.Play("dive");
				while (furbModel.animation.isPlaying)
				{
					yield return null;
				}
			}
			m_FurbyBaby.SetLayerInChildren(24);
			m_objectGenerator.WibbleObjects();
			m_FurbyBaby.GetComponent<HideFurby>().ReHideBaby();
		}

		private IEnumerator FurbyFoundSequence()
		{
			yield return null;
		}

		private void IdleTimeEvent()
		{
			HideAndSeekGameEvent hideAndSeekGameEvent = HideAndSeekGameEvent.ShortTimePassed;
			m_IdleModeTimer += m_shortWaitTime;
			if (m_IdleModeTimer >= m_longWaitTime)
			{
				hideAndSeekGameEvent = HideAndSeekGameEvent.LongTimePassed;
			}
			GameEventRouter.SendEvent(hideAndSeekGameEvent);
			Invoke("IdleTimeEvent", m_shortWaitTime);
			Logging.Log("IdleTimeEvent");
		}

		private IEnumerator HandlePoppedBalloon(GameObject balloonObject)
		{
			HexGridPosition furbyHex = m_FurbyBaby.GetComponent<HexGridPosition>();
			HexGridPosition objectHex = balloonObject.GetComponent<HexGridPosition>();
			int distance = Mathf.Clamp(Mathf.RoundToInt(furbyHex.GetDistance(objectHex)), 0, 5);
			int oldNumTurns = TurnsLeft;
			if (balloonObject == m_specialBalloon)
			{
				UnSpeclializeABalloon();
				yield return StartCoroutine(PopSpecialBalloon(balloonObject, distance));
			}
			else
			{
				UnSpeclializeABalloon();
				yield return StartCoroutine(PopBalloon(balloonObject, distance));
			}
			if (distance == 0)
			{
				GameEventRouter.SendEvent(HideAndSeekGameEvent.BabiesFound);
				m_furbyFound = true;
				yield return StartCoroutine(FurbyFoundSequence());
			}
			else if (m_specialTurns.Contains(m_turnCount) || m_forceSpecial)
			{
				SpecializeABalloon();
			}
			if (distance != 0 && oldNumTurns == TurnsLeft)
			{
				GameEventRouter.SendEvent(HideAndSeekGameEvent.TurnsReduced);
				TurnsLeft--;
			}
		}

		private void SpecializeABalloon()
		{
			int childCount = m_objectGenerator.transform.childCount;
			if (childCount > 1)
			{
				while (m_specialBalloon == null)
				{
					Transform transform = null;
					transform = m_objectGenerator.transform.GetChild(UnityEngine.Random.Range(0, childCount));
					if (transform.gameObject != m_FurbyBaby.GetComponent<HideFurby>().CurrentHideObject)
					{
						m_specialBalloon = transform.gameObject;
					}
				}
			}
			if (m_specialBalloon != null)
			{
				ParticleSystem componentInChildrenIncludeInactive = m_specialBalloon.GetComponentInChildrenIncludeInactive<ParticleSystem>();
				if (componentInChildrenIncludeInactive != null)
				{
					componentInChildrenIncludeInactive.gameObject.SetActive(true);
					componentInChildrenIncludeInactive.gameObject.layer = 26;
					componentInChildrenIncludeInactive.Play();
				}
				m_specialBalloon.tag = "special";
			}
			m_forceSpecial = false;
		}

		private void UnSpeclializeABalloon()
		{
			if (m_specialBalloon != null)
			{
				ParticleSystem componentInChildren = m_specialBalloon.GetComponentInChildren<ParticleSystem>();
				if (componentInChildren != null)
				{
					componentInChildren.Stop();
					componentInChildren.gameObject.SetActive(false);
				}
				m_specialBalloon.tag = "Untagged";
			}
		}

		private IEnumerator PopBalloon(GameObject hitObject, int distance)
		{
			if (TurnsLeft != 1)
			{
				GameEventRouter.SendEvent((HideAndSeekGameEvent)(distance + 48));
				if (m_showDialogBubbles)
				{
					GameEventRouter.SendEvent(HideAndSeekGameEvent.SpeechBubbleShown);
				}
			}
			else if (distance != 0)
			{
				GameEventRouter.SendEvent(HideAndSeekGameEvent.HitLastTurn);
			}
			else
			{
				GameEventRouter.SendEvent(HideAndSeekGameEvent.HitDistanceFound);
			}
			SingletonInstance<PrefabPool>.Instance.ReturnToPool(hitObject);
			int popReactionIndex = distance;
			if (distance > m_popReactions.Length)
			{
				popReactionIndex = m_popReactions.Length - 1;
			}
			GameObject vfxPrefab = m_popReactions[popReactionIndex];
			if (FurbyGlobals.Player.NoFurbyOnSaveGame() && popReactionIndex != 0)
			{
				vfxPrefab = m_popReactionNoFurbyMode;
			}
			UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
			yield return new WaitForSeconds(m_popWaitTime);
		}

		private IEnumerator PopSpecialBalloon(GameObject hitObject, int distance)
		{
			SingletonInstance<PrefabPool>.Instance.ReturnToPool(hitObject);
			if (m_nextSpecialBalloon == SpecialBalloonType.Unset)
			{
				m_nextSpecialBalloon = m_specialBalloonRandomBag[UnityEngine.Random.Range(0, m_specialBalloonRandomBag.Length)];
			}
			switch (m_nextSpecialBalloon)
			{
			case SpecialBalloonType.AddTurns:
			{
				GameObject vfxPrefab = m_popReactions[m_currentLevel];
				UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
				yield return StartCoroutine(AddTries());
				break;
			}
			case SpecialBalloonType.LoseTurns:
			{
				GameObject vfxPrefab = m_popReactionNegative;
				UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
				yield return StartCoroutine(SubtractTries());
				break;
			}
			case SpecialBalloonType.MoveBaby:
			{
				GameObject vfxPrefab = m_popReactions[m_currentLevel];
				UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
				yield return StartCoroutine(ReHideBaby());
				break;
			}
			case SpecialBalloonType.ChainReaction:
			{
				GameObject vfxPrefab = m_popReactions[m_currentLevel];
				UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
				yield return StartCoroutine(ChainReaction());
				break;
			}
			default:
			{
				GameObject vfxPrefab = m_popReactions[m_currentLevel];
				UnityEngine.Object.Instantiate(vfxPrefab, hitObject.transform.position, hitObject.transform.rotation);
				break;
			}
			}
			m_nextSpecialBalloon = SpecialBalloonType.Unset;
		}

		private IEnumerator AddTries()
		{
			GameEventRouter.SendEvent(HideAndSeekGameEvent.ExtraTurnAdded);
			GameEventRouter.SendEvent(HideAndSeekGameEvent.SpeechBubbleShown);
			TurnsLeft += m_levelData[m_currentLevel].m_extraTries;
			yield return new WaitForSeconds(m_popWaitTime);
		}

		private IEnumerator SubtractTries()
		{
			GameEventRouter.SendEvent(HideAndSeekGameEvent.ExtraTurnLost);
			GameEventRouter.SendEvent(HideAndSeekGameEvent.SpeechBubbleShown);
			TurnsLeft -= m_levelData[m_currentLevel].m_loseTries;
			yield return new WaitForSeconds(m_popWaitTime);
		}

		private IEnumerator ReHideBaby()
		{
			GameEventRouter.SendEvent(HideAndSeekGameEvent.BabyMoved);
			GameEventRouter.SendEvent(HideAndSeekGameEvent.SpeechBubbleShown);
			m_objectGenerator.WibbleObjects();
			m_FurbyBaby.GetComponent<HideFurby>().ReHideBaby();
			yield return new WaitForSeconds(m_popWaitTime);
		}

		private IEnumerator ChainReaction()
		{
			GameEventRouter.SendEvent(HideAndSeekGameEvent.BalloonPopChainStarted);
			GameEventRouter.SendEvent(HideAndSeekGameEvent.SpeechBubbleShown);
			int count = m_levelData[m_currentLevel].m_chainReactionLength;
			while (count > 0)
			{
				int numRemainingBalloons = m_objectGenerator.transform.childCount;
				if (numRemainingBalloons < 2)
				{
					break;
				}
				Transform balloonXform = m_objectGenerator.transform.GetChild(UnityEngine.Random.Range(0, numRemainingBalloons));
				if (balloonXform.gameObject != m_FurbyBaby.GetComponent<HideFurby>().CurrentHideObject)
				{
					SingletonInstance<PrefabPool>.Instance.ReturnToPool(balloonXform.gameObject);
					GameObject vfxPrefab = m_popReactions[m_currentLevel];
					UnityEngine.Object.Instantiate(vfxPrefab, balloonXform.position, balloonXform.rotation);
					count--;
					yield return new WaitForSeconds(0.25f);
				}
			}
		}

		private void OnDebugPanelGUI(Enum evt, GameObject gObj, params object[] p)
		{
			if (DebugPanel.StartSection("Hide and Seek"))
			{
				GUILayout.Label("Next special balloon type:");
				GUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				GUILayout.BeginVertical();
				if (GUILayout.Toggle(m_nextSpecialBalloon == SpecialBalloonType.Unset, "Random"))
				{
					m_nextSpecialBalloon = SpecialBalloonType.Unset;
				}
				if (GUILayout.Toggle(m_nextSpecialBalloon == SpecialBalloonType.AddTurns, "Add Tries"))
				{
					m_nextSpecialBalloon = SpecialBalloonType.AddTurns;
				}
				if (GUILayout.Toggle(m_nextSpecialBalloon == SpecialBalloonType.LoseTurns, "Subtract Tries"))
				{
					m_nextSpecialBalloon = SpecialBalloonType.LoseTurns;
				}
				if (GUILayout.Toggle(m_nextSpecialBalloon == SpecialBalloonType.MoveBaby, "Hide Baby"))
				{
					m_nextSpecialBalloon = SpecialBalloonType.MoveBaby;
				}
				if (GUILayout.Toggle(m_nextSpecialBalloon == SpecialBalloonType.ChainReaction, "Chain Reaction"))
				{
					m_nextSpecialBalloon = SpecialBalloonType.ChainReaction;
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				m_forceSpecial = GUILayout.Toggle(m_forceSpecial, "Force special balloon next turn");
				GUILayout.BeginHorizontal();
				GUILayout.Label("Turns Left");
				GUILayout.TextField(TurnsLeft.ToString());
				if (GUILayout.Button("-1"))
				{
					TurnsLeft--;
				}
				if (GUILayout.Button("+1"))
				{
					TurnsLeft++;
				}
				GUILayout.EndHorizontal();
				bool flag = GUILayout.Toggle(m_showDialogBubbles, "Show Dialog Bubbles");
				if (flag != m_showDialogBubbles)
				{
					m_showDialogBubbles = flag;
					ShowDialogBoxes(m_showDialogBubbles);
				}
			}
			DebugPanel.EndSection();
		}

		private void ShowDialogBoxes(bool visible)
		{
			List<ActiveOnEvent> list = new List<ActiveOnEvent>();
			m_dialogBubbles.GetComponentsInChildrenIncludeInactive(list);
			foreach (ActiveOnEvent item in list)
			{
				item.Action = (visible ? ActiveOnEvent.ActionType.Active : ActiveOnEvent.ActionType.Disabled);
			}
		}
	}
}
