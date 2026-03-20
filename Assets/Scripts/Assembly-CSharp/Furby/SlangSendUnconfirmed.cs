using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SlangSendUnconfirmed : MonoBehaviour
	{
		[SerializeField]
		private float m_minSleep = 2f;

		[SerializeField]
		private float m_maxSleep = 8f;

		[SerializeField]
		private float m_timeout = 2f;

		private IEnumerator Start()
		{
			GameData gameData = Singleton<GameDataStoreObject>.Instance.Data;
			List<string> unconfirmed = new List<string>();
			foreach (string s in gameData.m_unconfirmedSlangs)
			{
				unconfirmed.Add(s);
			}
			Shuffle(unconfirmed);
			Logging.Log(string.Format("Iterating over {0} unconfirmed slang tones.", unconfirmed.Count));
			foreach (string s2 in unconfirmed)
			{
				float sleepTime = UnityEngine.Random.Range(m_minSleep, m_maxSleep);
				Logging.Log(string.Format("Slang sleeping for {0} before sending another unconfirmed tone.", sleepTime));
				yield return new WaitForSeconds(sleepTime);
				yield return StartCoroutine(TryToConfirm(s2, gameData));
			}
			Logging.Log(string.Format("Tried to send all {0} unconfirmed tones.", unconfirmed.Count));
			Logging.Log("No more slang tones will be sent by this scene.");
		}

		private void Shuffle(List<string> l)
		{
			int num = l.Count - 1;
			while (num > 1)
			{
				num--;
				int index = UnityEngine.Random.Range(0, num + 1);
				string value = l[index];
				l[index] = l[num];
				l[num] = value;
			}
		}

		private IEnumerator TryToConfirm(string phrase, GameData gameData)
		{
			Logging.Log(string.Format("Trying to confirm slang phrase \"{0}\"", phrase));
			FurbyAction action = (FurbyAction)(int)Enum.Parse(typeof(FurbyAction), phrase);
			bool waiting = true;
			string phrase2 = default(string);
			GameData gameData2 = default(GameData);
			FurbyReply reply = delegate(bool b)
			{
				Logging.Log(string.Format("Slang unlock for \"{0}\" got reply({1})", phrase2, b));
				if (b)
				{
					Logging.Log(string.Format("{0} removed from unconfirmed phrase list", phrase2));
					gameData2.m_unconfirmedSlangs.Remove(phrase2);
					waiting = false;
				}
			};
			if (Singleton<FurbyDataChannel>.Instance.PostAction(action, reply))
			{
				float untilTimeout = m_timeout;
				Logging.Log(string.Format("Posting {0} was successful, waiting for reply...", phrase));
				while (waiting)
				{
					untilTimeout -= Time.deltaTime;
					if (untilTimeout <= 0f)
					{
						Logging.Log(string.Format("Timeout out waiting for reply for {0}", phrase));
						waiting = false;
					}
					yield return null;
				}
			}
			else
			{
				Logging.Log(string.Format("Failed to post {0}", phrase));
			}
		}
	}
}
