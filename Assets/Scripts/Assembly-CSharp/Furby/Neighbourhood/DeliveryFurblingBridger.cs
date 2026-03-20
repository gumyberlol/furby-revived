using UnityEngine;

namespace Furby.Neighbourhood
{
	public class DeliveryFurblingBridger : MonoBehaviour
	{
		public void TellTheRemovalVanControllerToDeliverFurbling()
		{
			RemovalVanController component = GameObject.Find("Ufo_Gold_DoNotRename").GetComponent<RemovalVanController>();
			component.DeliverFurbling();
		}
	}
}
