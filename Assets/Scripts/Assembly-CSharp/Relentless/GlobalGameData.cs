using System;
using System.Collections.Generic;
using Furby;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class GlobalGameData
	{
		public float m_PreSaveGameLoadAudioVolume = 0.5f;

		public float m_CommsLevel = 1f;

		public bool m_CommsLevel_UserCustomized;

		private ScreenOrientation m_ScreenOrientation = ScreenOrientation.Portrait;

		public bool m_ScreenOrientation_UserCustomized;

		public long TimeOfLastSave;

		public Locale m_Locale;

		public List<string> m_videosPlayed = new List<string>();

		public bool m_amEligibleForSpring = true;

		public bool m_amEligibleForCrystal;

		public bool m_crystalUnlocked;

		public bool m_InAppWarningDialogHasBeenShown;

		public string m_LastKnownCountryCode = string.Empty;

		public float CommsLevel
		{
			get
			{
				return m_CommsLevel;
			}
			set
			{
				m_CommsLevel = value;
				ComAirChannel.CommsVolume = m_CommsLevel;
			}
		}

		public bool CommsLevel_UserCustomized
		{
			get
			{
				return m_CommsLevel_UserCustomized;
			}
			set
			{
				m_CommsLevel_UserCustomized = value;
				ApplyCommsLevel();
			}
		}

		public bool AmEligibleForSpring
		{
			get
			{
				return m_amEligibleForSpring;
			}
			private set
			{
				m_amEligibleForSpring = value;
			}
		}

		public bool AmEligibleForCrystal
		{
			get
			{
				return m_amEligibleForCrystal;
			}
			private set
			{
				m_amEligibleForCrystal = value;
			}
		}

		public bool CrystalUnlocked
		{
			get
			{
				return m_crystalUnlocked;
			}
			private set
			{
				m_crystalUnlocked = value;
			}
		}

		public bool InAppWarningDialogHasBeenShown
		{
			get
			{
				return m_InAppWarningDialogHasBeenShown;
			}
			set
			{
				m_InAppWarningDialogHasBeenShown = value;
			}
		}

		public string CountryCode
		{
			get
			{
				if (m_LastKnownCountryCode == null)
				{
					m_LastKnownCountryCode = string.Empty;
				}
				return m_LastKnownCountryCode;
			}
			set
			{
				m_LastKnownCountryCode = value;
			}
		}

		public float GetPreSaveGameLoadAudioVolume()
		{
			return m_PreSaveGameLoadAudioVolume;
		}

		public void SetPreSaveGameLoadAudioVolume(float volume)
		{
			m_PreSaveGameLoadAudioVolume = volume;
		}

		public void ApplyCommsLevel()
		{
		}

		public ScreenOrientation GetCustomizedScreenOrientation()
		{
			return m_ScreenOrientation;
		}

		public void StoreCustomizedScreenOrientation(ScreenOrientation orient)
		{
			m_ScreenOrientation = orient;
		}

		public void MakeSpringEligible()
		{
			AmEligibleForSpring = true;
			FurbyGlobals.AdultLibrary.BuildDictionary();
			FurbyGlobals.BabyLibrary.BuildDictionary();
		}

		public void MakeCrystalEligible()
		{
			AmEligibleForCrystal = true;
			FurbyGlobals.AdultLibrary.BuildDictionary();
			FurbyGlobals.BabyLibrary.BuildDictionary();
		}

		public void UnlockCrystal()
		{
			CrystalUnlocked = true;
		}

		public void LockCrystal()
		{
			CrystalUnlocked = false;
		}

		public void MakeSpringIneligible()
		{
			AmEligibleForSpring = false;
		}

		public void MakeCrystalIneligible()
		{
			AmEligibleForCrystal = false;
		}
	}
}
