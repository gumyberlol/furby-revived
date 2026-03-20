using Relentless;
using UnityEngine;

namespace Furby
{
	public class OneTimeAppInitialisation : Singleton<OneTimeAppInitialisation>
	{
		private void Awake()
		{
			Application.targetFrameRate = 60;
		}
	}
}
