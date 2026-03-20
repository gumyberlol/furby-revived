using System;

namespace NUnit.Framework
{
	public interface ITestCaseData
	{
		object[] Arguments { get; }

		object Result { get; }

		Type ExpectedException { get; }

		string ExpectedExceptionName { get; }

		string TestName { get; }

		string Description { get; }

		bool Ignored { get; }

		string IgnoreReason { get; }
	}
}
