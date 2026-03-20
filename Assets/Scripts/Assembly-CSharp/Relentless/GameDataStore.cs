using System;
using System.Collections;
using System.Collections.Generic;

namespace Relentless
{
	public class GameDataStore : PersistedDataStore<GameData>
	{
		private delegate Hashtable MiniJsonVersionUpdater(Hashtable game);

		protected override int Version
		{
			get
			{
				return 28;
			}
		}

		public GameDataStore(int slotIndex)
		{
			base.Name = "FurbyGameData" + slotIndex;
			AddVersionUpdater(21, (string s) => Encrypt(s));
			AddVersionUpdater(22, WrapEncryption(WrapMiniJsonVersionUpdater(addNumFurblingsWhenLoaded)));
			AddVersionUpdater(23, WrapEncryption(WrapMiniJsonVersionUpdater(delegate(Hashtable game)
			{
				game["m_numSessionsIsCountingFromStart"] = false;
				return game;
			})));
			AddVersionUpdater(24, WrapEncryption(WrapMiniJsonVersionUpdater(delegate(Hashtable game)
			{
				game["m_LastGameConfig"] = null;
				game["m_HaveStoredADownloadedGameConfig"] = false;
				return game;
			})));
			AddVersionUpdater(25, WrapEncryption(WrapMiniJsonVersionUpdater(delegate(Hashtable game)
			{
				game["m_LastGameConfig"] = null;
				game["m_HaveStoredADownloadedGameConfig"] = false;
				return game;
			})));
			AddVersionUpdater(26, WrapEncryption(WrapMiniJsonVersionUpdater(PopulateFoodQRCodes)));
			AddVersionUpdater(27, WrapEncryption(WrapMiniJsonVersionUpdater(delegate(Hashtable game)
			{
				game["m_HaveSentConversionTelemetry"] = false;
				return game;
			})));
			AddVersionUpdater(28, WrapEncryption(WrapMiniJsonVersionUpdater(PopulateQRScanHistory)));
		}

		private VersionUpdater WrapMiniJsonVersionUpdater(MiniJsonVersionUpdater func)
		{
			return delegate(string s)
			{
				Hashtable hashtable = MiniJSON.jsonDecode(s) as Hashtable;
				if (hashtable == null)
				{
					throw new ApplicationException("Failed to parse as JSON object.");
				}
				hashtable = func(hashtable);
				s = hashtable.toJson();
				return s;
			};
		}

		private Hashtable addNumFurblingsWhenLoaded(Hashtable game)
		{
			ArrayList arrayList = game["BabyInstancesDontAccessDirectly"] as ArrayList;
			if (arrayList == null)
			{
				throw new ApplicationException("Failed to get Furblings from JSON");
			}
			int count = arrayList.Count;
			game["m_numFurblingsEverHatched"] = count;
			game["m_numFurblingsWhenGiftingFeatureAdded"] = count;
			return game;
		}

		private Hashtable PopulateFoodQRCodes(Hashtable game)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("SBE", "PANTRY_ITEM_TUNAROLL");
			dictionary.Add("QKB", "PANTRY_ITEM_TATERTOTS");
			dictionary.Add("AHF", "PANTRY_ITEM_FROGURT");
			dictionary.Add("AYH", "PANTRY_ITEM_PEEP");
			dictionary.Add("RNM", "PANTRY_ITEM_CUPCAKE");
			Dictionary<string, string> dictionary2 = dictionary;
			ArrayList arrayList = game["m_purchasedItems"] as ArrayList;
			ArrayList arrayList2 = game["RecognizedQRCodes"] as ArrayList;
			if (arrayList == null)
			{
				return game;
			}
			if (arrayList2 == null)
			{
				return game;
			}
			foreach (string item in arrayList2)
			{
				if (dictionary2.ContainsKey(item))
				{
					string text = "PantryFoodData_" + dictionary2[item];
					if (!arrayList.Contains(text))
					{
						arrayList.Add(text);
					}
				}
			}
			game["m_purchasedItems"] = arrayList;
			return game;
		}

		private Hashtable PopulateQRScanHistory(Hashtable game)
		{
			game["QRItemsUnlocked"] = game["RecognizedQRCodes"];
			return game;
		}
	}
}
