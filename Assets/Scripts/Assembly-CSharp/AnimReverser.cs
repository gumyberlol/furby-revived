using System;
using UnityEngine;

public class AnimReverser : IDisposable
{
	private Animation anim;

	public AnimReverser(Animation anim)
	{
		this.anim = anim;
		foreach (AnimationState item in anim)
		{
			item.speed = -1f;
			item.time = item.length;
		}
	}

	public void Dispose()
	{
		foreach (AnimationState item in anim)
		{
			item.speed = 1f;
			item.time = 0f;
		}
	}
}
