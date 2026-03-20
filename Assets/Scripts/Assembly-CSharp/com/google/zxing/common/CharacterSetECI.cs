using System;
using System.Collections;

namespace com.google.zxing.common
{
	public sealed class CharacterSetECI : ECI
	{
		private static Hashtable VALUE_TO_ECI;

		private static Hashtable NAME_TO_ECI;

		private string encodingName;

		public string EncodingName
		{
			get
			{
				return encodingName;
			}
		}

		private CharacterSetECI(int value_Renamed, string encodingName)
			: base(value_Renamed)
		{
			this.encodingName = encodingName;
		}

		private static void initialize()
		{
			VALUE_TO_ECI = Hashtable.Synchronized(new Hashtable(29));
			NAME_TO_ECI = Hashtable.Synchronized(new Hashtable(29));
			addCharacterSet(0, "Cp437");
			addCharacterSet(1, new string[2] { "ISO8859_1", "ISO-8859-1" });
			addCharacterSet(2, "Cp437");
			addCharacterSet(3, new string[2] { "ISO8859_1", "ISO-8859-1" });
			addCharacterSet(4, "ISO8859_2");
			addCharacterSet(5, "ISO8859_3");
			addCharacterSet(6, "ISO8859_4");
			addCharacterSet(7, "ISO8859_5");
			addCharacterSet(8, "ISO8859_6");
			addCharacterSet(9, "ISO8859_7");
			addCharacterSet(10, "ISO8859_8");
			addCharacterSet(11, "ISO8859_9");
			addCharacterSet(12, "ISO8859_10");
			addCharacterSet(13, "ISO8859_11");
			addCharacterSet(15, "ISO8859_13");
			addCharacterSet(16, "ISO8859_14");
			addCharacterSet(17, "ISO8859_15");
			addCharacterSet(18, "ISO8859_16");
			addCharacterSet(20, new string[2] { "SJIS", "Shift_JIS" });
		}

		private static void addCharacterSet(int value_Renamed, string encodingName)
		{
			CharacterSetECI value = new CharacterSetECI(value_Renamed, encodingName);
			VALUE_TO_ECI[value_Renamed] = value;
			NAME_TO_ECI[encodingName] = value;
		}

		private static void addCharacterSet(int value_Renamed, string[] encodingNames)
		{
			CharacterSetECI value = new CharacterSetECI(value_Renamed, encodingNames[0]);
			VALUE_TO_ECI[value_Renamed] = value;
			for (int i = 0; i < encodingNames.Length; i++)
			{
				NAME_TO_ECI[encodingNames[i]] = value;
			}
		}

		public static CharacterSetECI getCharacterSetECIByValue(int value_Renamed)
		{
			if (VALUE_TO_ECI == null)
			{
				initialize();
			}
			if (value_Renamed < 0 || value_Renamed >= 900)
			{
				throw new ArgumentException("Bad ECI value: " + value_Renamed);
			}
			return (CharacterSetECI)VALUE_TO_ECI[value_Renamed];
		}

		public static CharacterSetECI getCharacterSetECIByName(string name)
		{
			if (NAME_TO_ECI == null)
			{
				initialize();
			}
			return (CharacterSetECI)NAME_TO_ECI[name];
		}
	}
}
