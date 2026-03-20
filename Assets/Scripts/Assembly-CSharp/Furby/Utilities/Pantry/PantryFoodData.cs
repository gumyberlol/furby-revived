using System;

namespace Furby.Utilities.Pantry
{
	[Serializable]
	public class PantryFoodData
	{
		public string Name;

		public UIAtlas GraphicAtlas;

		public string GraphicName;

		public int[] Watermarks = new int[6];

		public string[] Returns = new string[6];

		public bool UnlockedByQRCode;

		public string QRUnlockCode = string.Empty;

		public string DisplayName = string.Empty;

		public int m_FurbuckCost;

		public static int GetPersonalityIndex(int personalityCode)
		{
			int num = 0;
			switch (personalityCode)
			{
			case 900:
			case 901:
			case 902:
			case 903:
			case 904:
			case 905:
				return personalityCode - 900;
			default:
				return 0;
			case 920:
				return 1;
			case 919:
				return 2;
			case 917:
				return 3;
			case 921:
				return 4;
			case 918:
				return 5;
			}
		}

		public static string GetPersonality(int index)
		{
			int num = -1;
			switch (index)
			{
			default:
				return "DEFAULT";
			case 1:
				num = 920;
				break;
			case 2:
				num = 919;
				break;
			case 3:
				num = 917;
				break;
			case 4:
				num = 921;
				break;
			case 5:
				num = 918;
				break;
			}
			return ((FurbyPersonality)num).ToString();
		}

		public int WatermarkForPersonality(int personality)
		{
			return Convert.ToInt32(Watermarks[GetPersonalityIndex(personality)]);
		}

		public string ReturnForPersonality(int personality)
		{
			return Returns[GetPersonalityIndex(personality)];
		}
	}
}
