using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class PoopStationFlow : MonoBehaviour
	{
		public GameObject mainCamera;

		public Toilet toilet;

		[SerializeField]
		private PoopStationFurby m_furby;

		[SerializeField]
		private DialogPanel m_commsFailDialog;

		private HashSet<Smell> m_smells = new HashSet<Smell>();

		public void Start()
		{
			if (m_commsFailDialog == null)
			{
				throw new ApplicationException(string.Format("{0} does not have m_commsFailDialog set", base.gameObject.name));
			}
		}

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(typeof(PoopStationEvent), React);
			m_furby.PoopPrepared += delegate(Poop p)
			{
				Poop.PoopType poopType = ((p != null) ? p.GetPoopType() : Poop.PoopType.None);
				EventManager.Instance.PostEvent("PoopStation_PoopSwitch", EventAction.SetSwitch, poopType.ToString());
				base.gameObject.SendGameEvent(PoopStationEvent.PoopPrepared, poopType.ToString());
			};
			m_furby.Pooped += delegate(Poop p)
			{
				toilet.GetBowl().ReceivePoop(p);
				p.m_smellCreated += delegate(Smell s)
				{
					m_smells.Add(s);
					s.m_died += delegate
					{
						m_smells.Remove(s);
					};
				};
				base.gameObject.SendGameEvent(PoopStationEvent.PoopCreated);
			};
			m_furby.FailedToPoop += delegate
			{
				if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					StartCoroutine(HandleCommsError(m_furby));
				}
				else
				{
					m_furby.Reset();
				}
			};
			Spray spray = toilet.GetSpray();
			spray.m_switchedOn += delegate
			{
				StartCoroutine(SprayMaintainSurpression(spray));
			};
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PoopStationEvent), React);
			}
		}

		private IEnumerator SprayMaintainSurpression(Spray spray)
		{
			while (spray.IsOn())
			{
				foreach (Smell s in m_smells)
				{
					s.Surpress();
				}
				yield return null;
			}
		}

		private void React(Enum eventType, GameObject origin, params object[] parameters)
		{
			if (eventType.Equals(PoopStationEvent.LidActivated))
			{
				StartCoroutine(MainFlow());
			}
		}

		private IEnumerator MainFlow()
		{
			Logging.Log("MainFlow");
			m_furby.PrepareToPoop();
			yield return StartCoroutine(new WaitForGameEvent().WaitForEvent(PoopStationEvent.LidLiftCompleted));
			base.gameObject.SendGameEvent(PoopStationEvent.MoveToToyStarted);
			mainCamera.animation.Play();
			while (mainCamera.animation.isPlaying)
			{
				yield return null;
			}
			base.gameObject.SendGameEvent(PoopStationEvent.MoveToToyCompleted);
			while (m_furby.IsPreparing())
			{
				yield return null;
			}
			base.gameObject.SendGameEvent(PoopStationEvent.ReadyForPoop);
			m_furby.DoPoop();
			while (m_furby.IsPooping())
			{
				yield return null;
			}
			Animation anim = mainCamera.animation;
			using (new AnimReverser(anim))
			{
				base.gameObject.SendGameEvent(PoopStationEvent.MoveFromToy);
				anim.Play();
				while (anim.isPlaying)
				{
					yield return null;
				}
			}
			Bowl bowl = toilet.GetBowl();
			if (bowl.HasPoops())
			{
				FurbyReactToPoopsInBowl(m_furby, bowl);
				using (toilet.GetBowl().GetEnabledPeriod())
				{
					WaitForGameEvent wait = new WaitForGameEvent();
					yield return StartCoroutine(wait.WaitForEvent(PoopStationEvent.FlushCompleted));
				}
				while (m_smells.Count > 0)
				{
					yield return null;
				}
			}
			yield return StartCoroutine(toilet.GetLid().DropDown());
		}

		private static void FurbyReactToPoopsInBowl(PoopStationFurby furby, Bowl bowl)
		{
			IEnumerator<Poop> poopEnumerator = bowl.GetPoopEnumerator();
			while (poopEnumerator.MoveNext())
			{
				furby.ReactToPoop(poopEnumerator.Current);
			}
		}

		private IEnumerator HandleCommsError(PoopStationFurby furby)
		{
			Logging.Log("Handling comms error.");
			base.gameObject.SendGameEvent(PoopStationEvent.PoopPrepared, Poop.PoopType.None);
			base.gameObject.SendGameEvent(SharedGuiEvents.DialogShow, "CommsError");
			WaitForGameEvent w = new WaitForGameEvent();
			yield return StartCoroutine(w.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel, SharedGuiEvents.DialogHide));
			Logging.Log("User has dismissed the dialog.  Resetting Furby Pooping status.");
			furby.Reset();
		}
	}
}
