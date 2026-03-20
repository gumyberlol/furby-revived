using System.Collections.Generic;
using Relentless;

namespace Furby
{
	public class VideoEventToVideoFile : RelentlessMonoBehaviour
	{
		public Dictionary<TutorialVideoEvents, string> m_Dictionary = new Dictionary<TutorialVideoEvents, string>();

		public Dictionary<TutorialVideoEvents, string> Dictionary
		{
			get
			{
				return m_Dictionary;
			}
		}

		private void Start()
		{
			PopulateDictionary();
		}

		private void PopulateDictionary()
		{
			m_Dictionary.Clear();
			m_Dictionary.Add(TutorialVideoEvents.BlenderVideo, "Video/Video_Blender.ogg");
			m_Dictionary.Add(TutorialVideoEvents.FurballVideo, "Video/Video_Furball.ogg");
			m_Dictionary.Add(TutorialVideoEvents.HideAndSeekVideo, "Video/Video_HideAndSeek.ogg");
			m_Dictionary.Add(TutorialVideoEvents.HoseVideo, "Video/Video_Hose.ogg");
			m_Dictionary.Add(TutorialVideoEvents.HatchingVideo, "Video/Video_Incubator.ogg");
			m_Dictionary.Add(TutorialVideoEvents.WinningTheGameVideo, "Video/Video_Intro.ogg");
			m_Dictionary.Add(TutorialVideoEvents.PantryVideo, "Video/Video_Pantry.ogg");
			m_Dictionary.Add(TutorialVideoEvents.PlayroomDecoratingVideo, "Video/Video_PlayroomCustomisation.ogg");
			m_Dictionary.Add(TutorialVideoEvents.PlayroomInteraction, "Video/Video_PlayroomInteraction.ogg");
			m_Dictionary.Add(TutorialVideoEvents.PoopStationVideo, "Video/Video_PoopStation.ogg");
			m_Dictionary.Add(TutorialVideoEvents.SpaVideo, "Video/Video_Salon.ogg");
			m_Dictionary.Add(TutorialVideoEvents.SickbayVideo, "Video/Video_Sickbay.ogg");
			m_Dictionary.Add(TutorialVideoEvents.SingAlongVideo, "Video/Video_Singalong.ogg");
		}

		public string GetVideoName(TutorialVideoEvents evt)
		{
			string value = string.Empty;
			if (Dictionary.TryGetValue(evt, out value))
			{
			}
			return value;
		}
	}
}
