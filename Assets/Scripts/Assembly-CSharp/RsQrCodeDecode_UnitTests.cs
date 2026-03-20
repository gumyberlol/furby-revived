using NUnit.Framework;
using Relentless;
using UnityEngine;

public class RsQrCodeDecode_UnitTests : TestScript
{
	protected QrCodeDecode.DetectedQrCode ScanQrCodeOnGhost(string ghost)
	{
		Texture texture = GameObject.Find(ghost).guiTexture.texture;
		RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
		Graphics.Blit(texture, renderTexture);
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0, false);
		texture2D.Apply();
		QrCodeDecode qrCodeDecode = new QrCodeDecode();
		qrCodeDecode.SetSource(texture2D);
		return qrCodeDecode.ExtractQrCode();
	}

	public TestMethodState CheckWikipediaQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("Wikipedia");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(4));
		Assert.That(detectedQrCode.Text, Is.EqualTo("http://en.m.wikipedia.org"));
		return testMethod.Complete();
	}

	public TestMethodState CheckFurbyQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("Furby");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(0));
		Assert.That(detectedQrCode.Text, Is.EqualTo(null));
		return testMethod.Complete();
	}

	public TestMethodState CheckSmallFurbyQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("SmallFurby");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(3));
		Assert.That(detectedQrCode.Text, Is.EqualTo("012345"));
		return testMethod.Complete();
	}

	public TestMethodState CheckPartial1QrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("Partial");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(0));
		Assert.That(detectedQrCode.Text, Is.EqualTo(null));
		return testMethod.Complete();
	}

	public TestMethodState CheckPartial2oQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("Partial2");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(0));
		Assert.That(detectedQrCode.Text, Is.EqualTo(null));
		return testMethod.Complete();
	}

	public TestMethodState CheckTooCloseQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("TooClose");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(0));
		Assert.That(detectedQrCode.Text, Is.EqualTo(null));
		return testMethod.Complete();
	}

	public TestMethodState CheckNoQrCodeQrCode(TestMethodState testMethod)
	{
		QrCodeDecode.DetectedQrCode detectedQrCode = ScanQrCodeOnGhost("NoQrCode");
		Assert.That(detectedQrCode.CornerPoints.Length, Is.EqualTo(0));
		Assert.That(detectedQrCode.Text, Is.EqualTo(null));
		return testMethod.Complete();
	}
}
