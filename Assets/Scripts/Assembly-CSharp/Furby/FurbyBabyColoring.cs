using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class FurbyBabyColoring : ScriptableObject
	{
		public Texture FurTexture;

		public Texture EggTexture;

		public Texture BitsTexture;

		public float tilingX = 1f;

		public float tilingY = 1f;
	}
}
