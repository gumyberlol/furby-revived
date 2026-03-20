using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Draws a GUI Texture. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	[ActionCategory(ActionCategory.GUI)]
	public class DrawTexture : FsmStateAction
	{
		[RequiredField]
		public FsmTexture texture;

		[Title("Position")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Rectangle on the screen to draw the texture within. Alternatively, set or override individual properties below.")]
		public FsmRect screenRect;

		public FsmFloat left;

		public FsmFloat top;

		public FsmFloat width;

		public FsmFloat height;

		[Tooltip("How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.")]
		public ScaleMode scaleMode;

		[Tooltip("Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.")]
		public FsmBool alphaBlend;

		[Tooltip("Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used. Pass in w/h for the desired aspect ratio. This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.")]
		public FsmFloat imageAspect;

		[Tooltip("Use normalized screen coordinates (0-1)")]
		public FsmBool normalized;

		private Rect rect;

		public override void Reset()
		{
			texture = null;
			screenRect = null;
			left = 0f;
			top = 0f;
			width = 1f;
			height = 1f;
			scaleMode = ScaleMode.StretchToFill;
			alphaBlend = true;
			imageAspect = 0f;
			normalized = true;
		}
	}
}
