using UnityEngine;

public class QRCodeDemo : MonoBehaviour
{
	public Texture2D qrCodeImage;

	private WebCamTexture m_camTexture;

	private Texture2D m_luminanceInputTexture;

	private RenderTexture m_renderTexture;

	private Texture2D m_zxingTexture;

	public Material m_greyscale;

	private void Start()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		WebCamDevice[] array = devices;
		foreach (WebCamDevice webCamDevice in array)
		{
			Debug.Log(webCamDevice.name);
		}
		m_camTexture = new WebCamTexture(devices[0].name, 640, 480, 30);
		base.renderer.material.mainTexture = m_camTexture;
		m_camTexture.Play();
		m_renderTexture = new RenderTexture(640, 480, 24);
		m_luminanceInputTexture = new Texture2D(m_camTexture.width, m_camTexture.height, TextureFormat.RGB24, false);
	}

	public void Update()
	{
		Graphics.Blit(m_camTexture, m_renderTexture, m_greyscale);
		RenderTexture renderTexture = RenderTexture.active;
		RenderTexture.active = m_renderTexture;
		m_luminanceInputTexture.ReadPixels(new Rect(0f, 0f, m_camTexture.width, m_camTexture.height), 0, 0, false);
		m_luminanceInputTexture.Apply();
		RenderTexture.active = renderTexture;
		base.transform.rotation *= Quaternion.Euler(0.1f, 0.2f, 0.3f);
		base.renderer.material.mainTexture = qrCodeImage;
	}
}
