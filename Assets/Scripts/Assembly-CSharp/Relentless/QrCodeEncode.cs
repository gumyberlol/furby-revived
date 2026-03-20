using UnityEngine;
using com.google.zxing.qrcode.decoder;
using com.google.zxing.qrcode.encoder;

namespace Relentless
{
	public class QrCodeEncode
	{
		private Encoder m_qrEncoder;

		public Texture2D Encode(string input, int blockSize)
		{
			QRCode qRCode = new QRCode();
			Encoder.encode(input, ErrorCorrectionLevel.Q, qRCode);
			int num = qRCode.Matrix.Width + 2;
			Texture2D texture2D = new Texture2D(num * blockSize, num * blockSize, TextureFormat.RGB24, false);
			Color32[] array = new Color32[num * num * blockSize * blockSize];
			int num2 = num - 1;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Color32 color = Color.white;
					if (i > 0 && j > 0 && i < num2 && j < num2)
					{
						color = ((qRCode.Matrix.Array[i - 1][j - 1] != 0) ? Color.black : Color.white);
					}
					for (int k = 0; k < blockSize; k++)
					{
						for (int l = 0; l < blockSize; l++)
						{
							array[i * blockSize + l + (j * blockSize + k) * num * blockSize] = color;
						}
					}
				}
			}
			texture2D.SetPixels32(array);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			return texture2D;
		}
	}
}
