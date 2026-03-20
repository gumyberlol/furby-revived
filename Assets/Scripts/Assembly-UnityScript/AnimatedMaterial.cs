using System;
using UnityEngine;

[Serializable]
public class AnimatedMaterial : MonoBehaviour
{
	public float scrollSpeed;

	public float offset;

	public AnimatedMaterial()
	{
		scrollSpeed = 0.5f;
	}

	public virtual void Update()
	{
		offset -= Time.deltaTime * scrollSpeed / 10f;
		renderer.material.SetTextureOffset("_Texture", new Vector2(0f, offset));
	}

	public virtual void Main()
	{
	}
}
