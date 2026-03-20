using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("NGUI/Interaction/SnapPanelWhenTriggered")]
	public class UISnapPanelWhenTriggered : RelentlessMonoBehaviour
	{
		public UIWrappedGrid UIWrappedGrid;

		public string TagToLookFor;

		public Snap SnapCommandToSend;

		public void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag(TagToLookFor) && UIWrappedGrid != null)
			{
				UIWrappedGrid.Snap(SnapCommandToSend);
			}
		}
	}
}
