using System;
using System.Collections;
using System.Text;

public class SupportClass
{
	public static byte[] ToByteArray(sbyte[] sbyteArray)
	{
		byte[] array = null;
		if (sbyteArray != null)
		{
			array = new byte[sbyteArray.Length];
			for (int i = 0; i < sbyteArray.Length; i++)
			{
				array[i] = (byte)sbyteArray[i];
			}
		}
		return array;
	}

	public static byte[] ToByteArray(string sourceString)
	{
		return Encoding.UTF8.GetBytes(sourceString);
	}

	public static byte[] ToByteArray(object[] tempObjectArray)
	{
		byte[] array = null;
		if (tempObjectArray != null)
		{
			array = new byte[tempObjectArray.Length];
			for (int i = 0; i < tempObjectArray.Length; i++)
			{
				array[i] = (byte)tempObjectArray[i];
			}
		}
		return array;
	}

	public static int URShift(int number, int bits)
	{
		if (number >= 0)
		{
			return number >> bits;
		}
		return (number >> bits) + (2 << ~bits);
	}

	public static int URShift(int number, long bits)
	{
		return URShift(number, (int)bits);
	}

	public static long URShift(long number, int bits)
	{
		if (number >= 0)
		{
			return number >> bits;
		}
		return (number >> bits) + (2L << ~bits);
	}

	public static long URShift(long number, long bits)
	{
		return URShift(number, (int)bits);
	}

	public static long Identity(long literal)
	{
		return literal;
	}

	public static ulong Identity(ulong literal)
	{
		return literal;
	}

	public static float Identity(float literal)
	{
		return literal;
	}

	public static double Identity(double literal)
	{
		return literal;
	}

	public static void GetCharsFromString(string sourceString, int sourceStart, int sourceEnd, char[] destinationArray, int destinationStart)
	{
		int num = sourceStart;
		int num2 = destinationStart;
		while (num < sourceEnd)
		{
			destinationArray[num2] = sourceString[num];
			num++;
			num2++;
		}
	}

	public static void SetCapacity(ArrayList vector, int newCapacity)
	{
		if (newCapacity > vector.Count)
		{
			vector.AddRange(new Array[newCapacity - vector.Count]);
		}
		else if (newCapacity < vector.Count)
		{
			vector.RemoveRange(newCapacity, vector.Count - newCapacity);
		}
		vector.Capacity = newCapacity;
	}

	public static sbyte[] ToSByteArray(byte[] byteArray)
	{
		sbyte[] array = null;
		if (byteArray != null)
		{
			array = new sbyte[byteArray.Length];
			for (int i = 0; i < byteArray.Length; i++)
			{
				array[i] = (sbyte)byteArray[i];
			}
		}
		return array;
	}
}
