using System;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorDebug : GameEventReceiver
	{
		[SerializeField]
		private IncubatorLogic m_EggObject;

		private GameEventSubscription m_DebugPanelEvent;

		private IncubatorLevel.InteractionType? m_GameState;

		private static readonly FurbyPersonality[] m_PersonalityArray = new FurbyPersonality[5]
		{
			FurbyPersonality.Gobbler,
			FurbyPersonality.Kooky,
			FurbyPersonality.Base,
			FurbyPersonality.SweetBelle,
			FurbyPersonality.ToughGirl
		};

		private static readonly IncubatorLevel.InteractionType[] m_AttentionArray = new IncubatorLevel.InteractionType[3]
		{
			IncubatorLevel.InteractionType.Cold,
			IncubatorLevel.InteractionType.Scared,
			IncubatorLevel.InteractionType.Dusty
		};

		public override Type EventType
		{
			get
			{
				return typeof(IncubatorGameEvent);
			}
		}

		public void Start()
		{
			m_DebugPanelEvent = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
			base.gameObject.SetActive(Debug.isDebugBuild);
		}

		public void OnDestroy()
		{
			m_DebugPanelEvent.Dispose();
		}

		public void OnDebugPanelGUI(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			if (DebugPanel.StartSection("Incubator"))
			{
				int incubatorFastForwards = FurbyGlobals.Player.IncubatorFastForwards;
				bool incubationFastForwarded = IncubatorLogic.FurbyBaby.IncubationFastForwarded;
				GUILayout.BeginHorizontal();
				GUILayout.Label("Number of FF Available: " + incubatorFastForwards);
				GUILayout.EndHorizontal();
				if (m_GameState.HasValue)
				{
					if (m_GameState.Value == IncubatorLevel.InteractionType.Imprinting)
					{
						GUILayout.Label("Personality to Imprint");
						FurbyPersonality[] personalityArray = m_PersonalityArray;
						foreach (FurbyPersonality furbyPersonality in personalityArray)
						{
							if (GUILayout.Button(furbyPersonality.ToString()))
							{
								OnImprint(furbyPersonality);
								break;
							}
						}
					}
				}
				else
				{
					GUILayout.Label("Interactions", RelentlessGUIStyles.Style_Header);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Imprint"))
					{
						m_EggObject.OnForceImprint();
					}
					GUILayout.Space(10f);
					IncubatorLevel.InteractionType[] attentionArray = m_AttentionArray;
					foreach (IncubatorLevel.InteractionType interactionType in attentionArray)
					{
						if (GUILayout.Button(interactionType.ToString()))
						{
							m_EggObject.OnForceAttention(interactionType);
						}
					}
					GUILayout.Space(10f);
					if (GUILayout.Button("Hatch"))
					{
						m_EggObject.OnForceHatch();
					}
					GUILayout.EndHorizontal();
					GUILayout.Space(10f);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Fast-Forwards Available:", RelentlessGUIStyles.Style_Normal, GUILayout.Width(300f));
					GUILayout.Label(incubatorFastForwards.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(true));
					incubatorFastForwards -= Convert.ToInt32(GUILayout.Button("REMOVE", GUILayout.Height(25f)));
					incubatorFastForwards += Convert.ToInt32(GUILayout.Button("ADD ONE", GUILayout.Height(25f)));
					if (GUILayout.Button("CLEAR", GUILayout.Height(25f)))
					{
						incubatorFastForwards = 0;
					}
					GUILayout.EndHorizontal();
					FurbyGlobals.Player.IncubatorFastForwards = Mathf.Abs(incubatorFastForwards);
					GUILayout.Space(20f);
					int fastForwardAvailabilityThreshold = SingletonInstance<GameConfiguration>.Instance.GetGameConfigBlob().m_FastForwardAvailabilityThreshold;
					GUILayout.BeginHorizontal();
					GUILayout.Label("FF Threshold (GameConfig)", RelentlessGUIStyles.Style_Normal, GUILayout.Width(300f));
					GUILayout.Label(fastForwardAvailabilityThreshold.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal();
					int incubatorEggsHatched = Singleton<GameDataStoreObject>.Instance.Data.IncubatorEggsHatched;
					GUILayout.BeginHorizontal();
					GUILayout.Label("Num Eggs Incubated", RelentlessGUIStyles.Style_Normal, GUILayout.Width(300f));
					incubatorEggsHatched -= Convert.ToInt32(GUILayout.Button("REMOVE", GUILayout.Height(25f)));
					incubatorEggsHatched += Convert.ToInt32(GUILayout.Button("ADD ONE", GUILayout.Height(25f)));
					GUILayout.Label(incubatorEggsHatched.ToString(), RelentlessGUIStyles.Style_Column, GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal();
					Singleton<GameDataStoreObject>.Instance.Data.IncubatorEggsHatched = incubatorEggsHatched;
					GUILayout.Space(20f);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Fast-Forward Active", RelentlessGUIStyles.Style_Normal, GUILayout.Width(300f));
					if (GUILayout.Button(incubationFastForwarded.ToString(), GUILayout.Height(25f)))
					{
						IncubatorLogic.FurbyBaby.IncubationFastForwarded ^= true;
						Application.LoadLevel(Application.loadedLevelName);
					}
					GUILayout.EndHorizontal();
				}
			}
			DebugPanel.EndSection();
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, object[] paramList)
		{
			switch ((IncubatorGameEvent)(object)enumValue)
			{
			case IncubatorGameEvent.Cleaning_Start:
				m_GameState = IncubatorLevel.InteractionType.Dusty;
				break;
			case IncubatorGameEvent.Warming_Start:
				m_GameState = IncubatorLevel.InteractionType.Cold;
				break;
			case IncubatorGameEvent.Comforting_Start:
				m_GameState = IncubatorLevel.InteractionType.Scared;
				break;
			case IncubatorGameEvent.Cleaning_Finished:
				m_GameState = null;
				break;
			case IncubatorGameEvent.Warming_Finished:
				m_GameState = null;
				break;
			case IncubatorGameEvent.Comforting_Finished:
				m_GameState = null;
				break;
			case IncubatorGameEvent.Imprint_Point:
				m_GameState = IncubatorLevel.InteractionType.Imprinting;
				break;
			case IncubatorGameEvent.Imprint_Personality:
				m_GameState = null;
				break;
			case IncubatorGameEvent.Hatching_Start:
				m_GameState = IncubatorLevel.InteractionType.Hatching;
				break;
			}
		}

		private void OnImprint(FurbyPersonality furbyPersonality)
		{
			GameEventRouter.SendEvent(IncubatorGameEvent.Imprint_Personality);
			m_EggObject.ImprintFinished(furbyPersonality, false);
		}
	}
}
