using NUnit.Framework;
using Relentless;

public class TestSerializedStringMap : TestScript
{
	private SerializableStringMap m_stringMap;

	private void AddSomeStuff()
	{
		m_stringMap.Add("test1", "fooey");
		m_stringMap.Add("test2", "wooey");
	}

	public TestMethodState TestAdd(TestMethodState testMethod)
	{
		m_stringMap = new SerializableStringMap();
		Assert.IsFalse(m_stringMap.ContainsKey("test1"));
		Assert.IsFalse(m_stringMap.ContainsKey("test2"));
		AddSomeStuff();
		Assert.IsTrue(m_stringMap.ContainsKey("test1"));
		Assert.IsTrue(m_stringMap.ContainsKey("test2"));
		StringAssert.IsMatch(m_stringMap["test1"], "fooey");
		StringAssert.IsMatch(m_stringMap["test2"], "wooey");
		return testMethod.Complete();
	}

	public TestMethodState TestRemove(TestMethodState testMethod)
	{
		m_stringMap = new SerializableStringMap();
		AddSomeStuff();
		Assert.IsTrue(m_stringMap.ContainsKey("test1"));
		Assert.IsTrue(m_stringMap.ContainsKey("test2"));
		m_stringMap.Remove("test1");
		Assert.IsFalse(m_stringMap.ContainsKey("test1"));
		Assert.IsTrue(m_stringMap.ContainsKey("test2"));
		return testMethod.Complete();
	}

	public TestMethodState TestTryGetValue(TestMethodState testMethod)
	{
		m_stringMap = new SerializableStringMap();
		string value;
		Assert.IsFalse(m_stringMap.TryGetValue("test1", out value));
		Assert.IsNull(value);
		AddSomeStuff();
		Assert.IsTrue(m_stringMap.TryGetValue("test1", out value));
		StringAssert.IsMatch(value, "fooey");
		return testMethod.Complete();
	}

	public TestMethodState TestCount(TestMethodState testMethod)
	{
		m_stringMap = new SerializableStringMap();
		Assert.AreEqual(m_stringMap.Count, 0);
		AddSomeStuff();
		Assert.AreEqual(m_stringMap.Count, 2);
		return testMethod.Complete();
	}
}
