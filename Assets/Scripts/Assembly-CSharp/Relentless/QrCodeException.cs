using System;

namespace Relentless
{
	public class QrCodeException : Exception
	{
		public QrCodeException(string message)
			: base(message)
		{
		}
	}
}
