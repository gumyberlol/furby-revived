using System.Collections;
using UnityEngine;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode.decoder;
using com.google.zxing.qrcode.detector;

namespace Relentless
{
	public class QrCodeDecode
	{
		public enum DecodeStage
		{
			kNoCodesDetected = 0,
			kPossibleCodesDetected = 1,
			kCodeDecoded = 2
		}

		public class DetectedQrCode
		{
			public Vector2[] CornerPoints;

			public DecodeStage DecodeStage;

			public string Text;
		}

		public UnityLuminanceSource m_luminanceSource;

		private BinaryBitmap m_bitmapSource;

		private Decoder m_decoder;

		private Detector m_detector;

		private HybridBinarizer m_binarizer;

		private Hashtable m_hints;

		public QrCodeDecode()
		{
			m_hints = new Hashtable();
			m_hints.Add(DecodeHintType.TRY_HARDER, true);
			m_decoder = new Decoder();
			m_detector = new Detector();
		}

		public void SetSource(Texture2D source)
		{
			m_luminanceSource = new UnityLuminanceSource(source.width, source.height);
			m_binarizer = new HybridBinarizer(m_luminanceSource);
			m_bitmapSource = new BinaryBitmap(m_binarizer);
		}

		public void SetWebcamSource(WebCamTexture source)
		{
			Logging.Log(string.Format("Camera source width: {0}   height: {1}", source.width, source.height));
			m_luminanceSource = new UnityLuminanceSource(source.width, source.height);
			m_binarizer = new HybridBinarizer(m_luminanceSource);
			m_bitmapSource = new BinaryBitmap(m_binarizer);
		}

		public void SetLuminanceSource(UnityLuminanceSource source)
		{
			m_luminanceSource = source;
			m_binarizer = new HybridBinarizer(m_luminanceSource);
			m_bitmapSource = new BinaryBitmap(m_binarizer);
		}

		public DetectedQrCode ExtractQrCode()
		{
			DetectedQrCode detectedQrCode = new DetectedQrCode();
			detectedQrCode.DecodeStage = DecodeStage.kNoCodesDetected;
			if (!PrepareImageForDetection())
			{
				return detectedQrCode;
			}
			DetectorResult detectorResult = null;
			DecoderResult decoderResult = null;
			detectorResult = m_detector.detect(m_hints);
			if (detectorResult != null)
			{
				decoderResult = m_decoder.decode(detectorResult.Bits);
			}
			if (detectorResult != null)
			{
				detectedQrCode.CornerPoints = new Vector2[detectorResult.Points.Length];
				for (int i = 0; i < detectorResult.Points.Length; i++)
				{
					detectedQrCode.CornerPoints[i].x = detectorResult.Points[i].X;
					detectedQrCode.CornerPoints[i].y = detectorResult.Points[i].Y;
				}
				if (detectorResult.Points.Length > 0)
				{
					detectedQrCode.DecodeStage = DecodeStage.kPossibleCodesDetected;
				}
			}
			else
			{
				detectedQrCode.CornerPoints = new Vector2[0];
			}
			if (decoderResult != null)
			{
				detectedQrCode.Text = decoderResult.Text;
				if (detectedQrCode.Text != string.Empty)
				{
					detectedQrCode.DecodeStage = DecodeStage.kCodeDecoded;
				}
			}
			return detectedQrCode;
		}

		private bool PrepareImageForDetection()
		{
			m_bitmapSource.Reset();
			m_binarizer.Reset();
			m_detector.SetImage(m_bitmapSource.BlackMatrix);
			return true;
		}
	}
}
