using NUnit.Framework;
using UnityEngine;

public class SubsceneLoaderTest : TestScript
{
	public TestMethodState CheckSomeObjectsLoadedAfterPause(TestMethodState testMethod)
	{
		testMethod.WaitForSeconds(1f);
		int arg = Object.FindObjectsOfType(typeof(GameObject)).Length;
		Assert.Greater(arg, 6);
		return testMethod.Complete();
	}
}
