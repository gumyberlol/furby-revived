using System.Collections;

namespace com.google.zxing
{
	public interface Reader
	{
		Result decode(BinaryBitmap image);

		Result decode(BinaryBitmap image, Hashtable hints);
	}
}
