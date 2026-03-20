using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyGlobals : Singleton<FurbyGlobals>
	{
		public const int HARDWARE_SETTINGS_LAYER = 31;

		[SerializeField]
		private GameObject[] m_prefabbedGlobals;

		private GameObject m_InstancedAdultLibrary;

		private GameObject m_InstancedBabyLibrary;

		[SerializeField]
		private FurblingLibraryDataset m_Dataset;

		private bool m_AmInitialized;

		private AdultFurbyLibrary m_AdultLibrary;

		private BabyFurbyLibrary m_BabyLibrary;

		private Localisation m_localisation;

		private PlayerFurby m_player;

		private FurbucksWallet m_wallet;

		private FurbyGUI m_gui;

		private BabyRepositoryHelpers m_babies;

		private SpsScreenSwitcher m_screenSwitcher;

		private FlairLibrary m_flairLibrary;

		private PersonalityLibrary m_personalityLibrary;

		private InputInactivity m_inputInactivity;

		private DeviceSettings m_DeviceSettings;

		private VideoSettings m_videoSettings;

		private SettingsHelper m_settingsHelper;

		private VideoDecision m_VideoDecider;

		private VideoEventToVideoFile m_VideoFilenameLookup;

		private HardwareSettingsScreenFlow m_HardwareSettingsScreenFlow;

		private LoadingScreenPresentation m_LoadingScreenPresentation;

		private ThemePeriodChooser m_ThemePeriodChooser;

		private TribeUnlocking m_TribeUnlocking;

		private GiftList m_GiftList;

		private UpsellMessaging m_UpsellMessaging;

		public static AdultFurbyLibrary AdultLibrary
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_AdultLibrary;
			}
		}

		public static BabyFurbyLibrary BabyLibrary
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_BabyLibrary;
			}
		}

		public static Localisation Localisation
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_localisation;
			}
		}

		public static PlayerFurby Player
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_player;
			}
		}

		public static SpsScreenSwitcher ScreenSwitcher
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_screenSwitcher;
			}
		}

		public static FurbucksWallet Wallet
		{
			get
			{
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_wallet;
			}
		}

		public static FurbyGUI Overlay
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_gui;
			}
		}

		public static BabyRepositoryHelpers BabyRepositoryHelpers
		{
			get
			{
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_babies;
			}
		}

		public static FlairLibrary FlairLibrary
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_flairLibrary;
			}
		}

		public static PersonalityLibrary PersonalityLibrary
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_personalityLibrary;
			}
		}

		public static InputInactivity InputInactivity
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_inputInactivity;
			}
		}

		public static VideoSettings VideoSettings
		{
			get
			{
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_videoSettings;
			}
		}

		public static SettingsHelper SettingsHelper
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_settingsHelper;
			}
		}

		public static DeviceSettings DeviceSettings
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_DeviceSettings;
			}
		}

		public static VideoDecision VideoDecider
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_VideoDecider;
			}
		}

		public static VideoEventToVideoFile VideoFilenameLookup
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_VideoFilenameLookup;
			}
		}

		public static HardwareSettingsScreenFlow HardwareSettingsScreenFlow
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_HardwareSettingsScreenFlow;
			}
		}

		public static LoadingScreenPresentation LoadingScreenPresentation
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_LoadingScreenPresentation;
			}
		}

		public static ThemePeriodChooser ThemePeriodChooser
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_ThemePeriodChooser;
			}
		}

		public static TribeUnlocking TribeUnlocking
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_TribeUnlocking;
			}
		}

		public static GiftList GiftList
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_GiftList;
			}
		}

		public static UpsellMessaging UpsellMessaging
		{
			get
			{
				InitializeInstanceIfNeeded();
				return (!Singleton<FurbyGlobals>.Instance) ? null : Singleton<FurbyGlobals>.Instance.m_UpsellMessaging;
			}
		}

		private void Awake()
		{
			GameObject[] prefabbedGlobals = m_prefabbedGlobals;
			foreach (GameObject gameObject in prefabbedGlobals)
			{
				GameObject gameObject2 = (GameObject)Object.Instantiate(gameObject, base.transform.position, base.transform.rotation);
				gameObject2.name = gameObject.name;
				gameObject2.transform.parent = base.transform;
			}
			GameObject adultLibraryPrefab = m_Dataset.m_AdultLibraryPrefab;
			GameObject furblingLibraryPrefab = m_Dataset.m_FurblingLibraryPrefab;
			m_InstancedAdultLibrary = (GameObject)Object.Instantiate(adultLibraryPrefab, base.transform.position, base.transform.rotation);
			m_InstancedAdultLibrary.name = "AdultFurbyLibrary";
			m_InstancedAdultLibrary.transform.parent = base.transform;
			m_AdultLibrary = m_InstancedAdultLibrary.GetComponentInChildren<AdultFurbyLibrary>();
			m_InstancedBabyLibrary = (GameObject)Object.Instantiate(furblingLibraryPrefab, base.transform.position, base.transform.rotation);
			m_InstancedBabyLibrary.name = "BabyFurbyLibrary";
			m_InstancedBabyLibrary.transform.parent = base.transform;
			m_BabyLibrary = m_InstancedBabyLibrary.GetComponentInChildren<BabyFurbyLibrary>();
			m_localisation = GetComponentInChildren<Localisation>();
			m_wallet = GetComponentInChildren<FurbucksWallet>();
			m_player = GetComponentInChildren<PlayerFurby>();
			m_gui = GetComponentInChildren<FurbyGUI>();
			m_babies = GetComponentInChildren<BabyRepositoryHelpers>();
			m_screenSwitcher = GetComponentInChildren<SpsScreenSwitcher>();
			m_flairLibrary = GetComponentInChildren<FlairLibrary>();
			m_personalityLibrary = GetComponentInChildren<PersonalityLibrary>();
			m_inputInactivity = GetComponentInChildren<InputInactivity>();
			m_videoSettings = GetComponentInChildren<VideoSettings>();
			m_settingsHelper = GetComponentInChildren<SettingsHelper>();
			m_DeviceSettings = GetComponentInChildren<DeviceSettings>();
			m_VideoDecider = GetComponentInChildren<VideoDecision>();
			m_VideoFilenameLookup = GetComponentInChildren<VideoEventToVideoFile>();
			m_HardwareSettingsScreenFlow = GetComponentInChildren<HardwareSettingsScreenFlow>();
			m_LoadingScreenPresentation = GetComponentInChildren<LoadingScreenPresentation>();
			m_ThemePeriodChooser = GetComponentInChildren<ThemePeriodChooser>();
			m_TribeUnlocking = GetComponentInChildren<TribeUnlocking>();
			m_GiftList = GetComponentInChildren<GiftList>();
			m_UpsellMessaging = GetComponentInChildren<UpsellMessaging>();
			m_AmInitialized = false;
		}

		public void Initialize()
		{
			m_AmInitialized = true;
			BabyLibrary.BuildDictionary();
			AdultLibrary.BuildDictionary();
		}

		public static void InitializeInstanceIfNeeded()
		{
			if ((bool)Singleton<FurbyGlobals>.Instance && !Singleton<FurbyGlobals>.Instance.m_AmInitialized)
			{
				Singleton<FurbyGlobals>.Instance.Initialize();
			}
		}
	}
}
