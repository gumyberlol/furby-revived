using UnityEngine;

namespace Relentless
{
	[ExecuteInEditMode]
	[AddComponentMenu("NGUI/UI/Tiled Sprite (Gradient)")]
	public class UIGradientTiledSprite : UITiledSprite
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
