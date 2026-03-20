using System;
using Furby.Utilities.Salon2;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	[Serializable]
	public class SalonItem : BabyUtilityItem
	{
		public GameObject Prefab;

		public GameObject PressParticles;

		public GameObject EffectPrefab;

		public SalonItemUseEvent PressEvent;

		public GameObject ProgressionEffectPrefab;

		public bool HasFinalArtwork;

		public SalonItemUseEvent ReleaseEvent
		{
			get
			{
				if (PressEvent == SalonItemUseEvent.NONE)
				{
					throw new ApplicationException(string.Format("Cannot create ReleaseEvent for PressEvent {0}", PressEvent.ToString()));
				}
				string text = PressEvent.ToString();
				int length = text.LastIndexOf("_START");
				text = text.Substring(0, length);
				text += "_END";
				return (SalonItemUseEvent)(int)Enum.Parse(typeof(SalonItemUseEvent), text);
			}
		}
	}
}
