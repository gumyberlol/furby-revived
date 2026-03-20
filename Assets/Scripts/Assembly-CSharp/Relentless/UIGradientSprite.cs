using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("NGUI/UI/Sprite (Gradient)")]
	[ExecuteInEditMode]
	public class UIGradientSprite : UISprite
	{
		[HideInInspector]
		[SerializeField]
		public UIGradientAddin mGradient;

		public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
		{
			base.OnFill(verts, uvs, cols);
			mGradient.ApplyGradient(verts, uvs, cols, base.color, base.transform);
		}
	}
}
