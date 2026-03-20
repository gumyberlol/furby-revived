using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyResponseTest : MonoBehaviour
	{
		private const int AttemptCount = 10;

		private string StatusText = string.Empty;

		private void Start()
		{
			StartCoroutine(MainTestLoop());
		}

		private IEnumerator MainTestLoop()
		{
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			FurbyAction[] furballActions = new FurbyAction[2]
			{
				FurbyAction.CopyCat_Celebration,
				FurbyAction.Furball_Miss
			};
			FurbyAction[] hideseekActions = new FurbyAction[16]
			{
				FurbyAction.CopyCat_Disappointed,
				FurbyAction.CopyCat_Celebration,
				FurbyAction.HideSeek_Ready,
				FurbyAction.HideSeek_Freezing,
				FurbyAction.HideSeek_Cold,
				(FurbyAction)794,
				FurbyAction.HideSeek_Warm,
				FurbyAction.HideSeek_Hot,
				FurbyAction.HideSeek_Boiling,
				FurbyAction.HideSeek_Thinking,
				FurbyAction.HideSeek_Found,
				FurbyAction.HideSeek_Concerned,
				FurbyAction.HideSeek_Excited,
				FurbyAction.HideSeek_Happy,
				FurbyAction.HideSeek_Sad,
				FurbyAction.Angry
			};
			List<long> lastTone = new List<long>();
			bool lastResult = false;
			bool late = false;
			dataChannel.DisconnectDetection = false;
			dataChannel.ToneEvent += delegate(FurbyMessageType msgType, long msgTone)
			{
				lastTone.Add((!late) ? msgTone : (-msgTone));
			};
			FurbyReply furbyReply = delegate(bool result)
			{
				lastResult = result;
			};
			while (true)
			{
				yield return this.ConnectAndWaitOnReply(FurbyCommand.Furball);
				dataChannel.SetHeartBeatActive(false);
				yield return this.WaitWhileComAirIsBusy();
				FurbyAction[] array = furballActions;
				foreach (FurbyAction i in array)
				{
					for (int j = 0; j < 10; j++)
					{
						yield return this.CommandAndWaitOnSend(FurbyCommand.Maintenance);
						lastTone.Clear();
						lastResult = false;
						late = false;
						yield return this.ActionAndWaitOnSend(i, furbyReply);
						yield return new WaitForSeconds(4.125f);
						late = true;
						yield return new WaitForSeconds(2f);
						if (lastTone != null)
						{
							if (lastResult)
							{
								StatusText = string.Format("{0} - {1}({2}) : Response ", j, i, (long)i);
							}
							else
							{
								StatusText = string.Format("{0} - {1}({2}) : Late ", j, i, (long)i);
							}
						}
						else
						{
							StatusText = string.Format("{0} - {1}({2}) : No Response ", j, i, (long)i);
						}
						foreach (long k in lastTone)
						{
							StatusText += ((k >= 0) ? (k + " ") : (-k + "L "));
						}
					}
				}
				yield return this.ConnectAndWaitOnReply(FurbyCommand.Application);
				dataChannel.SetHeartBeatActive(false);
				yield return this.WaitWhileComAirIsBusy();
				FurbyAction[] array2 = hideseekActions;
				foreach (FurbyAction i2 in array2)
				{
					for (int j2 = 0; j2 < 10; j2++)
					{
						yield return this.CommandAndWaitOnSend(FurbyCommand.Maintenance);
						lastTone.Clear();
						lastResult = false;
						late = false;
						yield return this.ActionAndWaitOnSend(i2, furbyReply);
						yield return new WaitForSeconds(4.125f);
						late = true;
						yield return new WaitForSeconds(2f);
						if (lastTone != null)
						{
							if (lastResult)
							{
								StatusText = string.Format("{0} - {1}({2}) : Response ", j2, i2, (long)i2);
							}
							else
							{
								StatusText = string.Format("{0} - {1}({2}) : Late ", j2, i2, (long)i2);
							}
						}
						else
						{
							StatusText = string.Format("{0} - {1}({2}) : No Response ", j2, i2, (long)i2);
						}
						foreach (long k2 in lastTone)
						{
							StatusText += ((k2 >= 0) ? (k2 + " ") : (-k2 + "L "));
						}
					}
				}
			}
		}
	}
}
