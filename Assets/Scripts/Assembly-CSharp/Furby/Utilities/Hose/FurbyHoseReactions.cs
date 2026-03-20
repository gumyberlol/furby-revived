using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class FurbyHoseReactions : MonoBehaviour
	{
		[Serializable]
		public class HoseReaction
		{
			public Temperature m_temperature;

			public Pressure m_pressure;

			public FurbyAction m_action;
		}

		public delegate void Handler();

		private bool m_washed;

		private bool m_hasAppliedXP;

		[SerializeField]
		private List<HoseReaction> m_hoseReactions;

		[SerializeField]
		private float m_waitAfterDialsMoved = 2f;

		public float Cleanliness
		{
			get
			{
				return FurbyGlobals.Player.NewCleanliness;
			}
		}

		public event Handler HasBeenWashed;

		public event Handler Relieved;

		public event Handler WaitingForWater;

		public void SetWashed()
		{
			if (!m_washed)
			{
				m_washed = true;
				Logging.Log("Furby is washed:  You can turn the hose off now and you'll get a dropped item.");
				if (this.HasBeenWashed != null)
				{
					this.HasBeenWashed();
				}
			}
		}

		private void Relieve()
		{
			if (m_washed && !m_hasAppliedXP && this.Relieved != null)
			{
				this.Relieved();
			}
		}

		public bool IsWashed()
		{
			return m_washed;
		}

		public void StartReacting(DigitalHoseAdapter hose)
		{
			if (hose == null)
			{
				throw new ApplicationException("Cannot react to a null hose.");
			}
			StopAllCoroutines();
			StartCoroutine(ReactionFlow(hose));
		}

		private IEnumerator ReactionFlow(DigitalHoseAdapter hose)
		{
			yield return new WaitForSeconds(m_waitAfterDialsMoved);
			FurbyHoseReactions furbyHoseReactions = this;
			IEnumerator routine;
			if (hose.IsOn())
			{
				IEnumerator enumerator = ReactHoseOn(hose);
				routine = enumerator;
			}
			else
			{
				routine = ReactHoseOff(hose);
			}
			yield return furbyHoseReactions.StartCoroutine(routine);
		}

		private IEnumerator ReactHoseOn(DigitalHoseAdapter hose)
		{
			while (true)
			{
				yield return StartCoroutine(DoHoseOnFlow(hose));
			}
		}

		private IEnumerator DoHoseOnFlow(DigitalHoseAdapter hose)
		{
			yield return StartCoroutine(Wash(hose));
			Pressure pressure = hose.GetPressure();
			Temperature temperature = hose.GetTemperature();
			if (pressure.Equals(Pressure.Med) && temperature.Equals(Temperature.Nice))
			{
				yield return StartCoroutine(Sing());
				yield return StartCoroutine(Bored());
			}
		}

		private IEnumerator Wash(DigitalHoseAdapter hose)
		{
			Logging.Log(string.Format("Searching for reaction for temp: {0}, pressure: {1}", hose.GetTemperature(), hose.GetPressure()));
			HoseReaction reaction = m_hoseReactions.Find((HoseReaction r) =>
			{
				DigitalHoseAdapter hose2 = default(DigitalHoseAdapter);
				return IsReactionGoodForHose(hose2, r);
			});
			if (reaction == null)
			{
				throw new ApplicationException(string.Format("No good reaction for temp {0} pressure {1}", hose.GetTemperature(), hose.GetPressure()));
			}
			FurbyAction action = reaction.m_action;
			yield return StartCoroutine(ActionAndSetWashed(action));
			yield return new WaitForSeconds(2f);
		}

		private IEnumerator Sing()
		{
			FurbyAction sing = FurbyAction.Utility_ShowerSong;
			yield return StartCoroutine(ActionAndSetWashed(sing));
		}

		private IEnumerator Bored()
		{
			while (true)
			{
				int seconds = 10;
				Logging.Log(string.Format("Waiting {0} seconds before getting bored.", seconds));
				yield return new WaitForSeconds(seconds);
				FurbyAction bored = FurbyAction.Game_Bored;
				FurbyDataChannel data = Singleton<FurbyDataChannel>.Instance;
				Logging.Log("Sending Bored action.");
				data.PostAction(bored, null);
			}
		}

		private IEnumerator ActionAndSetWashed(FurbyAction action)
		{
			FurbyReply setWashed = delegate(bool b)
			{
				if (b)
				{
					SetWashed();
				}
			};
			yield return StartCoroutine(ActionAndCallback(action, setWashed));
		}

		private IEnumerator ActionAndCallback(FurbyAction action, FurbyReply reply)
		{
			FurbyDataChannel data = Singleton<FurbyDataChannel>.Instance;
			bool success = false;
			FurbyAction action2 = default(FurbyAction);
			FurbyReply reply2 = default(FurbyReply);
			while (!success)
			{
				bool waiting = true;
				FurbyReply wrappedReply = delegate(bool b)
				{
					Logging.Log(string.Format("Reply to {0} ({1})", action2.ToString(), b));
					success = b;
					waiting = false;
					if (reply2 != null)
					{
						reply2(b);
					}
				};
				bool posted = data.PostAction(action, wrappedReply);
				Logging.Log(string.Format("PostAction ({0}) returned {1}", action.ToString(), posted));
				if (posted)
				{
					while (waiting)
					{
						yield return null;
					}
				}
				else
				{
					float waitTime = 0.5f;
					Logging.Log(string.Format("Post failed, trying again in {0}s", waitTime));
					yield return new WaitForSeconds(waitTime);
				}
			}
		}

		private static bool IsReactionGoodForHose(DigitalHoseAdapter h, HoseReaction r)
		{
			if (!h.GetTemperature().Equals(r.m_temperature))
			{
				return false;
			}
			if (!h.GetTemperature().Equals(Temperature.Nice))
			{
				return true;
			}
			return h.GetPressure().Equals(r.m_pressure);
		}

		private IEnumerator ReactHoseOff(DigitalHoseAdapter hose)
		{
			Logging.Log(string.Format("Reacting to hose OFF (IsWashed={0}", IsWashed()));
			if (IsWashed())
			{
				yield return StartCoroutine(Cleaned());
			}
			else
			{
				yield return StartCoroutine(WaitForWater());
			}
		}

		private IEnumerator Cleaned()
		{
			Relieve();
			m_washed = false;
			yield return null;
		}

		private IEnumerator WaitForWater()
		{
			while (true)
			{
				yield return new WaitForSeconds(10f);
				Singleton<FurbyDataChannel>.Instance.PostAction(FurbyAction.Utility_NoWater, null);
				if (this.WaitingForWater != null)
				{
					this.WaitingForWater();
				}
			}
		}
	}
}
