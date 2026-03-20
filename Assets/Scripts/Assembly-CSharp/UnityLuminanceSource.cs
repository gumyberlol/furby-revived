using UnityEngine;
using com.google.zxing;

public class UnityLuminanceSource : LuminanceSource
{
	public enum HorizontalLineOrder
	{
		kTopDown = 0,
		kBottomUp = 1
	}

	private sbyte[] luminances;

	private bool isRotated;

	private int __height;

	private int __width;

	public override int Height
	{
		get
		{
			if (!isRotated)
			{
				return __height;
			}
			return __width;
		}
	}

	public override int Width
	{
		get
		{
			if (!isRotated)
			{
				return __width;
			}
			return __height;
		}
	}

	public override sbyte[] Matrix
	{
		get
		{
			return luminances;
		}
	}

	public override bool RotateSupported
	{
		get
		{
			return true;
		}
	}

	public UnityLuminanceSource(int width, int height)
		: base(width, height)
	{
		__width = width;
		__height = height;
		luminances = new sbyte[width * height];
	}

	public void SetLuminances(Color32[] pixels, HorizontalLineOrder lineOrder)
	{
		int num = 0;
		int num2 = 1;
		if (lineOrder == HorizontalLineOrder.kBottomUp)
		{
			num = __height - 1;
			num2 = -1;
		}
		for (int i = 0; i < __height; i++)
		{
			int num3 = num * __width;
			int num4 = i * __width;
			for (int j = 0; j < __width; j++)
			{
				Color32 color = pixels[num4];
				luminances[num3] = (sbyte)((float)(int)color.r * 0.15f + (float)(int)color.g * 0.295f + (float)(int)color.b * 0.055f);
				num3++;
				num4++;
			}
			num += num2;
		}
	}

	public void SetLuminances(Texture2D sourceTexture)
	{
		SetLuminances(sourceTexture.GetPixels32(), HorizontalLineOrder.kBottomUp);
	}

	public void SetLuminances(WebCamTexture sourceTexture)
	{
		SetLuminances(sourceTexture.GetPixels32(), HorizontalLineOrder.kBottomUp);
	}

	public Texture2D GetAsTexture2D()
	{
		Texture2D texture2D = new Texture2D(__width, __height, TextureFormat.RGB24, false);
		Color32[] pixels = texture2D.GetPixels32();
		for (int i = 0; i < __height; i++)
		{
			int num = i * __width;
			for (int j = 0; j < __width; j++)
			{
				byte b = (byte)((byte)luminances[num + j] << 1);
				pixels[num + j].r = b;
				pixels[num + j].g = b;
				pixels[num + j].b = b;
			}
		}
		texture2D.SetPixels32(pixels);
		texture2D.Apply();
		return texture2D;
	}

	public override sbyte[] getRow(int y, sbyte[] row)
	{
		if (!isRotated)
		{
			int num = Width;
			if (row == null || row.Length < num)
			{
				row = new sbyte[num];
			}
			for (int i = 0; i < num; i++)
			{
				row[i] = luminances[y * num + i];
			}
			return row;
		}
		int _width = __width;
		int _height = __height;
		if (row == null || row.Length < _height)
		{
			row = new sbyte[_height];
		}
		for (int j = 0; j < _height; j++)
		{
			row[j] = luminances[j * _width + y];
		}
		return row;
	}

	public override LuminanceSource crop(int left, int top, int width, int height)
	{
		return base.crop(left, top, width, height);
	}

	public override LuminanceSource rotateCounterClockwise()
	{
		isRotated = true;
		return this;
	}
}
