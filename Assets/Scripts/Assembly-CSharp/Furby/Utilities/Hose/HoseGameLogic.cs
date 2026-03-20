using System.Collections;
using Furby.Utilities.Pantry;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class HoseGameLogic : MonoBehaviour
	{
		[SerializeField]
		private DigitalHoseAdapter m_hose;

		[SerializeField]
		private FurbyHoseReactions m_furbyHose;

		[SerializeField]
		private FurbyPuddleReactions m_furbyPuddle;

		[SerializeField]
		private PuddleFactory m_puddleFactory;

		[SerializeField]
		private Transform m_droppedItemSpawnPoint;

		[SerializeField]
		private Dial m_pressureDial;

		[SerializeField]
		private Dial m_temperatureDial;

		public HintState m_SuggestTurningWaterOn = new HintState();

		public HintState m_SuggestTurningWaterOff = new HintState();

		[SerializeField]
		private BounceItem m_BounceItem;

		[SerializeField]
		private Vector3 m_BounceItemResetPosition = new Vector3(0f, 120f, 0f);

		[SerializeField]
		private float m_WaitBeforeActivatingBounceItem = 2f;

		[SerializeField]
		private GameObject m_DroppedItemOffsetNode;

		private void Update()
		{
			m_SuggestTurningWaterOn.TestAndBroadcastState();
			m_SuggestTurningWaterOff.TestAndBroadcastState();
		}

		private IEnumerator ActivateBounceItem()
		{
			m_DroppedItemOffsetNode.transform.localScale = Vector3.one;
			m_BounceItem.gameObject.SetActive(false);
			m_BounceItem.ResetPosition(m_BounceItemResetPosition);
			yield return new WaitForSeconds(m_WaitBeforeActivatingBounceItem);
			m_BounceItem.gameObject.SetActive(true);
		}

		private void Start()
		{
			m_BounceItem.gameObject.SetActive(false);
			m_BounceItem.OnGiven += OnItemGiven;
			m_BounceItem.OnBounce += OnItemBounced;
			m_BounceItem.OnFlicked += OnItemFlicked;
			m_SuggestTurningWaterOn.Enable();
			m_SuggestTurningWaterOff.Disable();
			SetupDial(m_pressureDial, delegate(float f)
			{
				m_hose.Hose.SetPressure(f);
			}, HoseGameEvent.PressureAmount, HoseGameEvent.PressureFullOff, HoseGameEvent.PressureFullOn, HoseGameEvent.TouchPressureControl);
			SetupDial(m_temperatureDial, delegate(float f)
			{
				m_hose.Hose.SetTemperature(f);
			}, HoseGameEvent.TemperatureAmount, HoseGameEvent.TemeratureFullOff, HoseGameEvent.TemperatureFullOn, HoseGameEvent.TouchTemperatureControl);
			m_hose.StatusChanged += delegate
			{
				m_furbyHose.StartReacting(m_hose);
			};
			SetupPuddles(m_hose);
			m_furbyHose.HasBeenWashed += delegate
			{
				GameEventRouter.SendEvent(HoseGameEvent.FurbyIsRelieved);
			};
			m_furbyHose.Relieved += delegate
			{
				m_furbyPuddle.DropItem();
			};
			m_furbyPuddle.ItemDropped += delegate(DroppedItem item)
			{
				StartCoroutine(ActivateBounceItem());
				item.transform.parent = m_droppedItemSpawnPoint;
				item.transform.localPosition = Vector3.zero;
				item.transform.localScale = Vector3.one;
				DigitalHoseAdapter.ChangeHandler disperseWhenHoseOn = null;
				disperseWhenHoseOn = delegate
				{
					m_BounceItem.gameObject.SetActive(false);
					item.ArriveAndDisperse.DisperseWhenReady();
					m_hose.SwitchedOn -= disperseWhenHoseOn;
					item.ArriveAndDisperse.DispersalCompleted += delegate
					{
						Object.Destroy(item.gameObject);
					};
				};
				m_hose.SwitchedOn += disperseWhenHoseOn;
			};
			m_hose.SwitchedOn += delegate
			{
				m_SuggestTurningWaterOn.Disable();
			};
			m_furbyHose.HasBeenWashed += delegate
			{
				if (m_hose.IsOn() && !m_SuggestTurningWaterOff.IsEnabled())
				{
					m_SuggestTurningWaterOff.Enable();
				}
			};
			m_hose.Hose.Changed += delegate
			{
				base.gameObject.SendGameEvent(HintEvents.Hose_HoseAdjusted);
			};
			m_hose.SwitchedOff += delegate
			{
				m_SuggestTurningWaterOff.Disable();
			};
			SetupFirstTimeFlow();
		}

		private void SetupPuddles(DigitalHoseAdapter hose)
		{
			Puddle p = null;
			hose.SwitchedOff += delegate
			{
				if (p != null)
				{
					p.ArriveAndDisperse.DisperseWhenReady();
				}
			};
			hose.SwitchedOn += delegate
			{
				if (p != null)
				{
					p.ArriveAndDisperse.CancelDispersalRequest();
				}
				else
				{
					p = m_puddleFactory.CreatePuddle(m_furbyHose);
					ArriveAndDisperse arriveAndDisperse = p.ArriveAndDisperse;
					arriveAndDisperse.DispersalStarted += delegate
					{
						p = null;
					};
				}
			};
			m_puddleFactory.PuddleCreated += delegate(Puddle puddle)
			{
				if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					ConnectPuddleCleanlinessToToy(m_furbyHose, puddle);
				}
				else
				{
					puddle.SetImmediateCleanliness(1f);
				}
			};
		}

		private static void ConnectPuddleCleanlinessToToy(FurbyHoseReactions furby, Puddle puddle)
		{
			puddle.SetImmediateCleanliness(furby.Cleanliness);
			FurbyHoseReactions.Handler puddleReflectFurby = delegate
			{
				puddle.TendTowardsCleanliness(furby.Cleanliness);
			};
			furby.HasBeenWashed += puddleReflectFurby;
			puddle.Destroying += delegate
			{
				furby.HasBeenWashed -= puddleReflectFurby;
			};
		}

		private void SetupFirstTimeFlow()
		{
			if (FurbyGlobals.Player.FlowStage != FlowStage.Hose)
			{
				return;
			}
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				DigitalHoseAdapter.ChangeHandler xpOnSwitchOff = null;
				xpOnSwitchOff = delegate
				{
					GameEventRouter.SendEvent(HoseGameEvent.NoFurbyFirstTimeGiveXP);
					m_hose.SwitchedOff -= xpOnSwitchOff;
				};
				m_hose.SwitchedOff += xpOnSwitchOff;
			}
			else
			{
				DigitalHoseAdapter.ChangeHandler setWashedImmediately = null;
				setWashedImmediately = delegate
				{
					m_furbyHose.SetWashed();
					m_hose.SwitchedOn -= setWashedImmediately;
				};
				m_hose.SwitchedOn += setWashedImmediately;
			}
		}

		private void SetupDial(Dial dial, Dial.ValueHandler onValueChange, HoseGameEvent adjustment, HoseGameEvent hitMin, HoseGameEvent hitMax, HoseGameEvent touch)
		{
			dial.ValueChanged += onValueChange;
			dial.ValueChanged += delegate(float f)
			{
				dial.gameObject.SendGameEvent(adjustment, f);
			};
			dial.Touched += delegate
			{
				dial.gameObject.SendGameEvent(touch);
			};
			dial.HitMin += delegate
			{
				dial.gameObject.SendGameEvent(hitMin);
			};
			dial.HitMax += delegate
			{
				dial.gameObject.SendGameEvent(hitMax);
			};
		}

		private void OnItemGiven()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodGiven, null, null);
		}

		private void OnItemBounced()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodBounced, null, null);
		}

		private void OnItemFlicked()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodFlicked, null, null);
		}
	}
}
