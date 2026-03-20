using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public static class ComAirExtensions
	{
		public static IEnumerator WaitWhileComAirIsBusy()
		{
			while (Singleton<FurbyDataChannel>.Instance.IsBusy)
			{
				yield return null;
			}
		}

		public static YieldInstruction WaitWhileComAirIsBusy(this MonoBehaviour component)
		{
			return component.StartCoroutine(WaitWhileComAirIsBusy());
		}

		public static IEnumerator ConnectAndWaitOnReply(FurbyCommand furbyCommand)
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(furbyCommand);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
			while (!Singleton<FurbyDataChannel>.Instance.FurbyConnected)
			{
				yield return null;
			}
		}

		public static YieldInstruction ConnectAndWaitOnReply(this MonoBehaviour component, FurbyCommand furbyCommand)
		{
			return component.StartCoroutine(ConnectAndWaitOnReply(furbyCommand));
		}

		public static IEnumerator ConnectAndWaitOnReply(FurbyCommand furbyCommand, float retryTime)
		{
			IEnumerator connector = ConnectAndWaitOnReply(furbyCommand);
			while (connector.MoveNext() && retryTime >= 0f)
			{
				retryTime -= Time.deltaTime;
				yield return null;
			}
		}

		public static YieldInstruction ConnectAndWaitOnReply(this MonoBehaviour component, FurbyCommand furbyCommand, float retryTime)
		{
			return component.StartCoroutine(ConnectAndWaitOnReply(furbyCommand, retryTime));
		}

		public static IEnumerator HeartBeatAndWaitOnSend()
		{
			while (!Singleton<FurbyDataChannel>.Instance.PostHeartBeat())
			{
				yield return null;
			}
		}

		public static YieldInstruction HeartBeatAndWaitOnSend(this MonoBehaviour component)
		{
			return component.StartCoroutine(HeartBeatAndWaitOnSend());
		}

		public static IEnumerator CommandAndWaitOnSend(FurbyCommand furbyCommand)
		{
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(furbyCommand, null))
			{
				yield return null;
			}
		}

		public static YieldInstruction CommandAndWaitOnSend(this MonoBehaviour component, FurbyCommand furbyCommand)
		{
			return component.StartCoroutine(CommandAndWaitOnSend(furbyCommand));
		}

		public static IEnumerator CommandAndWaitOnSend(FurbyCommand furbyCommand, FurbyReply furbyReply)
		{
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(furbyCommand, furbyReply))
			{
				yield return null;
			}
		}

		public static YieldInstruction CommandAndWaitOnSend(this MonoBehaviour component, FurbyCommand furbyAction, FurbyReply furbyReply)
		{
			return component.StartCoroutine(CommandAndWaitOnSend(furbyAction, furbyReply));
		}

		public static IEnumerator CommandAwaitReply(FurbyCommand furbyCommand, FurbyReply furbyReply)
		{
			bool completed = false;
			FurbyReply furbyReply2 = default(FurbyReply);
			FurbyReply proxyReply = delegate(bool acknowledged)
			{
				furbyReply2(acknowledged);
				completed = true;
			};
			while (!Singleton<FurbyDataChannel>.Instance.PostCommand(furbyCommand, proxyReply))
			{
				yield return null;
			}
			while (!completed)
			{
				yield return null;
			}
		}

		public static YieldInstruction CommandAwaitReply(this MonoBehaviour component, FurbyCommand furbyAction, FurbyReply furbyReply)
		{
			return component.StartCoroutine(CommandAwaitReply(furbyAction, furbyReply));
		}

		public static IEnumerator ActionAndWaitOnSend(FurbyAction furbyAction, FurbyReply furbyReply)
		{
			while (!Singleton<FurbyDataChannel>.Instance.PostAction(furbyAction, furbyReply))
			{
				yield return null;
			}
		}

		public static YieldInstruction ActionAndWaitOnSend(this MonoBehaviour component, FurbyAction furbyAction, FurbyReply furbyReply)
		{
			return component.StartCoroutine(ActionAndWaitOnSend(furbyAction, furbyReply));
		}

		public static IEnumerator SendAction(FurbyAction furbyAction, float retryTime, FurbyReply furbyReply)
		{
			IEnumerator infiniteSender = ActionAndWaitOnSend(furbyAction, furbyReply);
			while (infiniteSender.MoveNext() && retryTime > 0f)
			{
				retryTime -= Time.deltaTime;
				yield return null;
			}
		}

		public static YieldInstruction SendAction(this MonoBehaviour component, float retryTime, FurbyAction furbyAction, FurbyReply furbyReply)
		{
			return component.StartCoroutine(SendAction(furbyAction, retryTime, furbyReply));
		}
	}
}
