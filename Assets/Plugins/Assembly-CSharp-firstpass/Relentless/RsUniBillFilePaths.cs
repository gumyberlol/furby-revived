using System.IO;
using UnityEngine;

namespace Relentless
{
	public class RsUniBillFilePaths : MonoBehaviour
	{
		public enum FileEnum
		{
			IOSStoreKit = 0,
			AndroidGooglePlay = 1,
			AmazonJSON = 2,
			InventoryFile = 3,
			InventoryFileAndPath = 4
		}

		public static string s_IOSStorekitPath = "Assets/Plugins/unibill/generated/storekit";

		public static string s_GooglePlayPath = "Assets/Plugins/unibill/generated/googleplay";

		public static string s_InventoryPath = "Assets/Plugins/unibill/resources/";

		public static string s_IOSStorekitFile = "Import_StoreKit.txt";

		public static string s_GooglePlayFile = "Import_GooglePlay.txt";

		public static string s_AmazonJSONFile = "Import_Amazon.json.txt";

		public static string s_InventoryFile = "unibillInventory";

		public static string HELPER_GetTarget(FileEnum fileType)
		{
			string result = string.Empty;
			switch (fileType)
			{
			case FileEnum.IOSStoreKit:
				result = Path.Combine(s_IOSStorekitPath, s_IOSStorekitFile);
				result = new FileInfo(result).FullName;
				break;
			case FileEnum.AndroidGooglePlay:
				result = Path.Combine(s_GooglePlayPath, s_GooglePlayFile);
				result = new FileInfo(result).FullName;
				break;
			case FileEnum.AmazonJSON:
				result = s_AmazonJSONFile;
				break;
			case FileEnum.InventoryFile:
				result = s_InventoryFile;
				break;
			case FileEnum.InventoryFileAndPath:
				result = s_InventoryPath + s_InventoryFile;
				break;
			}
			return result;
		}
	}
}
