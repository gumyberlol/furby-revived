using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FurBallFootBall
{
	public bool ShowInInspector;

	public float MaxSpeed = -10.8f;

	public float MinSpeed = -10f;

	public float MinDelay;

	public float MaxDelay = 1f;

	public bool[] BallStartPositions;

	public bool[] BallTargetPositions;

	public float Speed
	{
		get
		{
			return UnityEngine.Random.Range(MinSpeed, MaxSpeed);
		}
	}

	public float Delay
	{
		get
		{
			return UnityEngine.Random.Range(MinDelay, MaxDelay);
		}
	}

	public int GetRandomStartPosition
	{
		get
		{
			List<int> list = new List<int>();
			for (int i = 0; i < BallStartPositions.Length; i++)
			{
				if (BallStartPositions[i])
				{
					list.Add(i);
				}
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
	}

	public int GetRandomTargetPosition
	{
		get
		{
			List<int> list = new List<int>();
			for (int i = 0; i < BallTargetPositions.Length; i++)
			{
				if (BallTargetPositions[i])
				{
					list.Add(i);
				}
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
	}
}
