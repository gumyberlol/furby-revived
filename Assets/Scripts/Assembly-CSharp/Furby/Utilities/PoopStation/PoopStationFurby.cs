using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class PoopStationFurby : MonoBehaviour
	{
		public delegate void PoopHandler(Poop p);

		public delegate void Handler();

		[SerializeField]
		private float m_prePoopWait = 2f;

		[SerializeField]
		private float m_postPoopWait = 5f;

		private bool m_isPreparing;

		private bool m_isPooping;

		[SerializeField]
		private PoopReactionLibrary m_poopReactions;

		private Poop m_nextPoopPrefab;

		public event PoopHandler PoopPrepared;

		public event PoopHandler Pooped;

		public event Handler FailedToPoop;

		public void PrepareToPoop()
		{
			if (!IsPreparing())
			{
				StartCoroutine(PreparationFlow());
			}
		}

		public bool IsPreparing()
		{
			return m_isPreparing;
		}

		private IEnumerator PreparationFlow()
		{
			m_isPreparing = true;
			Logging.Log(string.Format("{0} is preparing to poop.", base.gameObject.name));
			m_nextPoopPrefab = ChoosePoopPrefab();
			Logging.Log(string.Format("{0} says:  My next poop will be {1} of type {2}", base.gameObject.name, m_nextPoopPrefab.gameObject.name, m_nextPoopPrefab.GetPoopType().ToString()));
			if (this.PoopPrepared != null)
			{
				this.PoopPrepared(m_nextPoopPrefab);
			}
			Logging.Log("Checking toy presence.");
			FurbyDataChannel data = Singleton<FurbyDataChannel>.Instance;
			float timeoutThreshold = 8f;
			float timeoutTime = Time.time + timeoutThreshold;
			bool posted = false;
			bool waiting = true;
			bool ok = false;
			bool timeout = false;
			FurbyReply reply = delegate(bool b)
			{
				Logging.Log("820 Reply with " + b);
				waiting = false;
				ok = b;
			};
			while (!posted && !timeout)
			{
				posted = data.PostCommand(FurbyCommand.Application, reply);
				Logging.Log(string.Format("820 post request returned {0}", posted));
				if (!posted)
				{
					float sleepTime = 0.5f;
					Logging.Log("Failed to post 820 during preparation.");
					Logging.Log(string.Format("Sleeping for {0}s, and will retry", sleepTime));
					yield return new WaitForSeconds(sleepTime);
				}
				timeout = Time.time >= timeoutTime;
			}
			Logging.Log("Waiting for reply from 820.");
			while (waiting && !timeout)
			{
				yield return null;
				timeout = Time.time >= timeoutTime;
			}
			Logging.Log(string.Format("Finished waiting for 820.  ok={0}, timeout={1}", ok, timeout));
			if (!ok)
			{
				m_nextPoopPrefab = null;
				if (this.PoopPrepared != null)
				{
					this.PoopPrepared(m_nextPoopPrefab);
				}
			}
			m_isPreparing = false;
		}

		public void DoPoop()
		{
			if (IsPreparing())
			{
				throw new ApplicationException(string.Format("{0} is still preparing to poop.  Cannot call DoPoop yet.", base.gameObject.name));
			}
			if (!IsPooping())
			{
				StartCoroutine(PoopFlow());
			}
		}

		public bool IsPooping()
		{
			return m_isPooping;
		}

		private IEnumerator PoopFlow()
		{
			m_isPooping = true;
			bool commsOK = m_nextPoopPrefab != null;
			Logging.Log(string.Format("At the start of PoopFlow, commsOK={0}.", commsOK));
			if (commsOK)
			{
				FurbyDataChannel data = Singleton<FurbyDataChannel>.Instance;
				data.PostAction(FurbyAction.Poop, null);
				yield return new WaitForSeconds(m_prePoopWait);
				Poop poop = UnityEngine.Object.Instantiate(m_nextPoopPrefab) as Poop;
				poop.Activate();
				if (this.Pooped != null)
				{
					this.Pooped(poop);
				}
				yield return new WaitForSeconds(m_postPoopWait);
				Reset();
			}
			else if (this.FailedToPoop != null)
			{
				this.FailedToPoop();
			}
		}

		public void Reset()
		{
			m_isPooping = false;
			m_nextPoopPrefab = null;
		}

		private Poop ChoosePoopPrefab()
		{
			List<Poop> list = new List<Poop>();
			Poop[] componentsInChildren = base.gameObject.GetComponentsInChildren<Poop>();
			Poop[] array = componentsInChildren;
			foreach (Poop poop in array)
			{
				if (poop.IsAppropriateFor(this))
				{
					list.Add(poop);
				}
			}
			float num = 0f;
			foreach (Poop item in list)
			{
				Logging.Log(string.Format("Candidate Poop: {0} with likelihood {1}", item.gameObject.name, item.Likelihood));
				num += item.Likelihood;
			}
			float num2 = UnityEngine.Random.value * num;
			Logging.Log(string.Format("Choice is {0} out of {1}", num2, num));
			foreach (Poop item2 in list)
			{
				num2 -= item2.Likelihood;
				if (num2 <= 0f)
				{
					return item2;
				}
			}
			throw new ApplicationException(string.Format("Failed to chose a random one of {0} Poop prefabs", list.Count));
		}

		public void ReactToPoop(Poop poop)
		{
			Poop.PoopType poopType = poop.GetPoopType();
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			FurbyAction reactionFor = m_poopReactions.GetReactionFor(personality, poopType);
			Logging.Log(string.Format("Furby {0} reacts with {1} to {2} poop", personality, reactionFor, poopType));
			Singleton<FurbyDataChannel>.Instance.PostAction(reactionFor, null);
		}
	}
}
