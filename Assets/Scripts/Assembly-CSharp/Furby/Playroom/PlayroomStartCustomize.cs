using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomStartCustomize : RelentlessMonoBehaviour
	{
		public TutorialVideoEvents m_VideoEvent;

		private void OnClick()
		{
			StartCoroutine(InvokeSequence());
		}

		[ContextMenu("Simulate Click")]
		public IEnumerator InvokeSequence()
		{
			if (InNoFurbyMode())
			{
				yield return StartCoroutine(PlayVideoAndWaitForCompletion());
			}
			else if (!HavePlayedVideoAlready_PerSaveSlot())
			{
				yield return StartCoroutine(PlayVideoAndWaitForCompletion());
			}
			BeginCustomization();
		}

		private IEnumerator PlayVideoAndWaitForCompletion()
		{
			if (FurbyGlobals.VideoSettings.m_showVideos)
			{
				GameEventRouter.SendEvent(m_VideoEvent);
				yield return StartCoroutine(WaitForVideoToFinish());
			}
		}

		private IEnumerator WaitForVideoToFinish()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(VideoPlayerGameEvents.VideoHasFinished));
			MarkVideoAsPlayed_PerSaveSlot();
		}

		private bool InNoFurbyMode()
		{
			return FurbyGlobals.Player.NoFurbyOnSaveGame();
		}

		private string GetVideoFilename()
		{
			return FurbyGlobals.VideoFilenameLookup.GetVideoName(m_VideoEvent);
		}

		private bool HavePlayedVideoAlready_PerSaveSlot()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(GetVideoFilename());
		}

		private void MarkVideoAsPlayed_PerSaveSlot()
		{
			if (!Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Contains(GetVideoFilename()))
			{
				Singleton<GameDataStoreObject>.Instance.Data.m_videosPlayed.Add(GetVideoFilename());
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private void BeginCustomization()
		{
			Singleton<PlayroomModeController>.Instance.SetInCustomizeMode();
			GameEventRouter.SendEvent(PlayroomGameEvent.ExitPlayroom);
			GameEventRouter.SendEvent(PlayroomGameEvent.Customization_SequenceBegun);
			Singleton<PlayroomIdlingController>.Instance.Enable = false;
			GameObject gameObject = GameObject.Find("FurbyInteraction");
			PlayroomInteractionMediator playroomInteractionMediator = gameObject.GetComponent("PlayroomInteractionMediator") as PlayroomInteractionMediator;
			playroomInteractionMediator.InterruptSadSinging();
		}
	}
}
