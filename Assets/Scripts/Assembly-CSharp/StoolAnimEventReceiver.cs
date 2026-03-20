using UnityEngine;

public class StoolAnimEventReceiver : MonoBehaviour
{
	public delegate void Handler();

	public event Handler Boing;

	public void OnBoingEvent()
	{
		if (this.Boing != null)
		{
			this.Boing();
		}
	}
}
