using System;
using Relentless;
using UnityEngine;

public class UnlockLevels : ScriptableObject
{
	[EasyEditArray]
	public int[] Levels;

	public int m_LastIndex_Crystal;

	public int m_LastIndex_Spring;

	public int m_LastIndex_MainTribes;

	public int[] GetUnlockLevels()
	{
		int num = 0;
		num = (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal ? (m_LastIndex_Crystal + 1) : ((!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForSpring) ? (m_LastIndex_MainTribes + 1) : (m_LastIndex_Spring + 1)));
		int[] array = new int[num];
		Array.Copy(Levels, 0, array, 0, num);
		return array;
	}
}
