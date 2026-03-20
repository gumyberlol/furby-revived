using System.Collections;
using Furby.Playroom;
using Relentless;

namespace Furby
{
	public class EggUnlockingLogic : RelentlessMonoBehaviour
	{
		public SelectableLists m_SelectableLists;

		private string m_LastRecognizedScanCode = string.Empty;

		public SceneTargets m_SceneTargets;

		public float m_AnimationDuration = 5f;

		public IEnumerator Start()
		{
			yield return null;
			GameEventRouter.SendEvent(UnlockItemsEvents.FurblingItemsSequenceStarted);
			if (CaptureLastScanCode())
			{
				PopulateUnlockItemsUI();
			}
			Invoke("BroadcastSequenceComplete", m_AnimationDuration);
		}

		private void BroadcastSequenceComplete()
		{
			GameEventRouter.SendEvent(UnlockItemsEvents.FurblingItemsSequenceEnded);
		}

		private bool CaptureLastScanCode()
		{
			if (Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Count > 0)
			{
				int index = Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Count - 1;
				m_LastRecognizedScanCode = Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes[index];
				Logging.Log("m_LastRecognizedScanCode: " + m_LastRecognizedScanCode);
				return true;
			}
			return false;
		}

		private void PopulateUnlockItemsUI()
		{
			string empty = string.Empty;
			foreach (SelectableFeature lightFixture in m_SelectableLists.m_LightFixtures)
			{
				if (lightFixture.IsUnlockedByScannedQRCode())
				{
					empty = lightFixture.GetQRCode();
					if (m_LastRecognizedScanCode.Equals(empty))
					{
						m_SceneTargets.m_LightFixtureSprite.atlas = lightFixture.GetUIAtlas();
						m_SceneTargets.m_LightFixtureSprite.spriteName = lightFixture.GetSpriteName();
						m_SceneTargets.m_LightFixtureSprite.MakePixelPerfect();
					}
				}
			}
			foreach (SelectableFeature prop in m_SelectableLists.m_Props)
			{
				if (prop.IsUnlockedByScannedQRCode())
				{
					empty = prop.GetQRCode();
					if (m_LastRecognizedScanCode.Equals(empty))
					{
						m_SceneTargets.m_PropSprite.atlas = prop.GetUIAtlas();
						m_SceneTargets.m_PropSprite.spriteName = prop.GetSpriteName();
						m_SceneTargets.m_PropSprite.MakePixelPerfect();
					}
				}
			}
			foreach (SelectableFeature item in m_SelectableLists.m_Wallart)
			{
				if (item.IsUnlockedByScannedQRCode())
				{
					empty = item.GetQRCode();
					if (m_LastRecognizedScanCode.Equals(empty))
					{
						m_SceneTargets.m_WallArtSprite.atlas = item.GetUIAtlas();
						m_SceneTargets.m_WallArtSprite.spriteName = item.GetSpriteName();
						m_SceneTargets.m_WallArtSprite.MakePixelPerfect();
					}
				}
			}
		}
	}
}
