using System;
using UnityEngine;

namespace Relentless
{
	public class RelentlessException : UnityException
	{
		public RelentlessException()
		{
		}

		public RelentlessException(string message)
			: base(message)
		{
		}

		public RelentlessException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
