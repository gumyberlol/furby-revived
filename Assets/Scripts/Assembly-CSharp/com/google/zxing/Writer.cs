using System.Collections;
using com.google.zxing.common;

namespace com.google.zxing
{
	public interface Writer
	{
		ByteMatrix encode(string contents, BarcodeFormat format, int width, int height);

		ByteMatrix encode(string contents, BarcodeFormat format, int width, int height, Hashtable hints);
	}
}
