using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Perform a Mouse Pick on the scene and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
	[ActionCategory(ActionCategory.Input)]
	public class MousePick : FsmStateAction
	{
		[RequiredField]
		public FsmFloat rayDistance = 100f;

		[UIHint(UIHint.Variable)]
		public FsmBool storeDidPickObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector3 storePoint;

		[UIHint(UIHint.Variable)]
		public FsmVector3 storeNormal;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeDistance;

		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		public bool everyFrame;

		public override void Reset()
		{
			rayDistance = 100f;
			storeDidPickObject = null;
			storeGameObject = null;
			storePoint = null;
			storeNormal = null;
			storeDistance = null;
			layerMask = new FsmInt[0];
			invertMask = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoMousePick();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMousePick();
		}

		private void DoMousePick()
		{
			RaycastHit raycastHit = ActionHelpers.MousePick(rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			bool flag = raycastHit.collider != null;
			storeDidPickObject.Value = flag;
			if (flag)
			{
				storeGameObject.Value = raycastHit.collider.gameObject;
				storeDistance.Value = raycastHit.distance;
				storePoint.Value = raycastHit.point;
				storeNormal.Value = raycastHit.normal;
			}
			else
			{
				storeGameObject.Value = null;
				storeDistance.Value = float.PositiveInfinity;
				storePoint.Value = Vector3.zero;
				storeNormal.Value = Vector3.zero;
			}
		}
	}
}
