using UnityEngine;

public class ToiletRoll : MonoBehaviour
{
	public delegate void ClickHandler();

	public event ClickHandler Clicked;

	public void OnPress(bool down)
	{
		if (down && this.Clicked != null)
		{
			this.Clicked();
		}
	}
}
