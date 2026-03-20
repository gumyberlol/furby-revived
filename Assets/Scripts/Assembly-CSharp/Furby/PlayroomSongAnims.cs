using System;
using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PlayroomSongAnims : MonoBehaviour
	{
		[SerializeField]
		private AnimationClip m_toughGirlAnim;

		[SerializeField]
		private AnimationClip m_rockStarAnim;

		[SerializeField]
		private AnimationClip m_kookyAnim;

		[SerializeField]
		private AnimationClip m_gobblerAnim;

		[SerializeField]
		private AnimationClip m_sweetBelleAnim;

		[SerializeField]
		private GameObject m_target;

		private void Start()
		{
			GameEventRouter.AddDelegateForEnums(OnSongEvent, PlayroomGameEvent.SadSinging_ReceivedResponse);
		}

		private void OnDestroy()
		{
			GameEventRouter.RemoveDelegateForEnums(OnSongEvent, PlayroomGameEvent.SadSinging_ReceivedResponse);
		}

		private void OnSongEvent(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			string text = null;
			string[] flairs = selectedFurbyBaby.m_persistantData.flairs;
			foreach (string text2 in flairs)
			{
				FlairLibrary.PrefabLoader prefabLoader = FurbyGlobals.FlairLibrary.GetPrefabLoader(text2);
				if (!string.IsNullOrEmpty(prefabLoader.Flair.VocalSwitch))
				{
					text = prefabLoader.Flair.VocalSwitch;
					break;
				}
			}
			GameObject gameObject2 = m_target;
			ModelInstance component = gameObject2.GetComponent<ModelInstance>();
			if (component != null)
			{
				gameObject2 = component.Instance;
			}
			string text3 = null;
			switch (text)
			{
			case "Gobbler":
				text3 = m_gobblerAnim.name;
				break;
			case "Kooky":
				text3 = m_kookyAnim.name;
				break;
			case "Rockstar":
				text3 = m_rockStarAnim.name;
				break;
			case "Sweetbelle":
				text3 = m_sweetBelleAnim.name;
				break;
			case "Toughgirl":
				text3 = m_toughGirlAnim.name;
				break;
			}
			Logging.Log("Personality " + text);
			if (text3 != null && gameObject2 != null)
			{
				Logging.Log("Playing " + text3);
				gameObject2.animation.Play(text3);
			}
		}
	}
}
