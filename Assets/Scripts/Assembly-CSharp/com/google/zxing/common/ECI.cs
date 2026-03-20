using System;

namespace com.google.zxing.common
{
	public abstract class ECI
	{
		private int value_Renamed;

		public virtual int Value
		{
			get
			{
				return value_Renamed;
			}
		}

		internal ECI(int value_Renamed)
		{
			this.value_Renamed = value_Renamed;
		}

		public static ECI getECIByValue(int value_Renamed)
		{
			if (value_Renamed < 0 || value_Renamed > 999999)
			{
				throw new ArgumentException("Bad ECI value: " + value_Renamed);
			}
			if (value_Renamed < 900)
			{
				return CharacterSetECI.getCharacterSetECIByValue(value_Renamed);
			}
			return null;
		}
	}
}
