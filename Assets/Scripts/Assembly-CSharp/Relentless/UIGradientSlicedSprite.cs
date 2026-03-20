using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("NGUI/UI/Sliced Sprite (Gradient)")]
	[ExecuteInEditMode]
	public class UIGradientSlicedSprite : UISlicedSprite
	{
		[SerializeField]
		[HideInInspector]
		public UIGradientAddin mGradient;

		public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
		{
			base.OnFill(verts, uvs, cols);
			mGradient.ApplyGradient(verts, uvs, cols, base.color, base.transform);
		}
	}
}
