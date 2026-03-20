using Eye3.TestStar;
using NUnit.Framework;

public class TestMethodState
{
	public readonly float firstCallTime;

	public readonly float firstCallFixedTime;

	public readonly int firstCallFrameCount;

	public int timesCalled;

	protected TestState _state = TestState.running;

	public float waitForSeconds = -1f;

	public int waitForFrames = -1;

	public bool waitForFixedUpdate;

	public string message;

	public TestState state
	{
		get
		{
			return _state;
		}
		set
		{
			if (state == TestState.waiting || state == TestState.noState || state == TestState.skipped || state == TestState.passed)
			{
				throw new InvalidTestState(string.Format("State '{0}' cannot be set from a test method", value.ToString()));
			}
			_state = value;
		}
	}

	public TestMethodState(float firstCalled, float firstCallFixedTime, int firstCallFrameCount)
	{
		firstCallTime = firstCalled;
		this.firstCallFrameCount = firstCallFrameCount;
		this.firstCallFixedTime = firstCallFixedTime;
	}

	public TestMethodState Complete()
	{
		state = TestState.completed;
		return this;
	}

	public TestMethodState Disabled(string message)
	{
		state = TestState.disabled;
		this.message = message;
		return this;
	}

	public TestMethodState Failed(string message)
	{
		state = TestState.failed;
		throw new AssertionException(message);
	}

	public TestMethodState WaitForFixedUpdate()
	{
		waitForFixedUpdate = true;
		return this;
	}

	public TestMethodState WaitForFrames(int frames)
	{
		waitForFrames = frames;
		return this;
	}

	public TestMethodState WaitForSeconds(float seconds)
	{
		waitForSeconds = seconds;
		return this;
	}
}
