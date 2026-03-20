using System;
using System.Threading;
using Relentless;

namespace Furby
{
	public class ComAirChannel
	{
		public enum TxWaitRxMode
		{
			None = 0,
			Normal = 1,
			Boost = 2
		}

		public struct Tone
		{
			public long Outbound;

			public long Inbound;

			public FurbyReply Callback;

			public TxWaitRxMode WaitMode;

			public bool Acknowledged
			{
				get
				{
					return Valid(Inbound);
				}
			}

			public Tone(long data, FurbyReply callback, TxWaitRxMode mode)
			{
				Outbound = data;
				Inbound = -1L;
				WaitMode = mode;
				Callback = callback;
			}
		}

		public const long kNullCode = -1L;

		public const int kParityShift = 5;

		public const long kParityBit = 32L;

		public const long kParityMask = 31L;

		public const long kBoostBit = long.MinValue;

		private long m_ClockValue = Environment.TickCount;

		private int m_LastTickCount = Environment.TickCount;

		private Tone? m_PendingTone;

		private Thread m_RunThread;

		private ManualResetEvent m_PauseEvent = new ManualResetEvent(false);

		private bool m_AbortFlag;

		public long Ticks
		{
			get
			{
				lock (this)
				{
					int tickCount = Environment.TickCount;
					if (tickCount < m_LastTickCount)
					{
						m_ClockValue += tickCount;
						m_ClockValue += int.MaxValue - m_LastTickCount;
					}
					else
					{
						m_ClockValue += tickCount - m_LastTickCount;
					}
					m_LastTickCount = tickCount;
					return m_ClockValue;
				}
			}
		}

		public bool Active
		{
			get
			{
				return m_RunThread != null;
			}
			set
			{
				if (value != Active)
				{
					if (value)
					{
						Start();
					}
					else
					{
						Stop();
					}
				}
			}
		}

		public bool Paused
		{
			set
			{
				if (value)
				{
					m_PauseEvent.Set();
				}
				else
				{
					m_PauseEvent.Reset();
				}
			}
		}

		public bool IsBusy
		{
			get
			{
				return m_PendingTone.HasValue;
			}
		}

		public static float CommsVolume { get; set; }

		public static event ComAirEvent ComAirTick;

		public static event Func<int> ComAirInitialiseEvent;

		public static event Func<int> ComAirShutdownEvent;

		public static event Func<bool, int> ComAirDualBoostModeEvent;

		public static event Func<long> ComAirReceiveEvent;

		public static event Func<long, float, int> ComAirSendEvent;

		static ComAirChannel()
		{
			ComAirChannel.ComAirTick = delegate
			{
			};
			ComAirChannel.ComAirInitialiseEvent = (Func<int>)Delegate.Combine(ComAirChannel.ComAirInitialiseEvent, new Func<int>(ComAirAndroid.ComAirInitialiseEvent));
			ComAirChannel.ComAirShutdownEvent = (Func<int>)Delegate.Combine(ComAirChannel.ComAirShutdownEvent, new Func<int>(ComAirAndroid.ComAirShutdownEvent));
			ComAirChannel.ComAirDualBoostModeEvent = (Func<bool, int>)Delegate.Combine(ComAirChannel.ComAirDualBoostModeEvent, new Func<bool, int>(ComAirAndroid.ComAirDualBoostModeEvent));
			ComAirChannel.ComAirReceiveEvent = (Func<long>)Delegate.Combine(ComAirChannel.ComAirReceiveEvent, new Func<long>(ComAirAndroid.ComAirReceiveEvent));
			ComAirChannel.ComAirSendEvent = (Func<long, float, int>)Delegate.Combine(ComAirChannel.ComAirSendEvent, new Func<long, float, int>(ComAirAndroid.ComAirSendEvent));
		}

		public void Start()
		{
			m_RunThread = new Thread(Run);
			m_AbortFlag = false;
			m_RunThread.Priority = ThreadPriority.Highest;
			m_RunThread.Name = "ComAirTick";
			ComAirChannel.ComAirInitialiseEvent();
			m_RunThread.Start();
			Logging.Log("STARTING COMAIR");
		}

		public void Stop()
		{
			m_AbortFlag = true;
			m_PauseEvent.Reset();
			m_RunThread.Join();
			ComAirChannel.ComAirShutdownEvent();
			m_RunThread = null;
		}

		private void Run()
		{
			Tone tone = new Tone(-1L, null, TxWaitRxMode.None);
			while (!m_AbortFlag)
			{
				if (!m_PauseEvent.WaitOne(0))
				{
					if (!InternalReceive(tone, 1350, false) && !InternalSend())
					{
						Thread.Sleep(125);
					}
					ComAirChannel.ComAirTick(null);
				}
				else
				{
					Thread.Sleep(125);
				}
			}
			ComAirChannel.ComAirTick(m_PendingTone);
			m_PendingTone = null;
		}

		private bool InternalReceive(Tone tone, int delay, bool retry)
		{
			long num = ComAirChannel.ComAirReceiveEvent();
			long num2 = -1L;
			while (retry | ParityEven(num))
			{
				m_PauseEvent.WaitOne(delay);
				num2 = ComAirChannel.ComAirReceiveEvent();
				if (ParityEven(num) & ParityOdd(num2))
				{
					tone.Inbound = num << 5;
					tone.Inbound |= num2 & 0x1F;
					ComAirChannel.ComAirTick(tone);
					return true;
				}
				num = num2;
				retry = false;
			}
			return false;
		}

		private bool InternalSend()
		{
			if (m_PendingTone.HasValue)
			{
				Tone value = m_PendingTone.Value;
				long num = value.Outbound >> 5;
				long num2 = value.Outbound & 0x1F;
				switch (value.WaitMode)
				{
				case TxWaitRxMode.Boost:
					ComAirChannel.ComAirDualBoostModeEvent(true);
					ComAirChannel.ComAirSendEvent(num & 0x1F, CommsVolume);
					m_PauseEvent.WaitOne(1000);
					ComAirChannel.ComAirSendEvent(num2 | 0x20, CommsVolume);
					m_PauseEvent.WaitOne(2500);
					num = ComAirChannel.ComAirReceiveEvent();
					ComAirChannel.ComAirDualBoostModeEvent(false);
					if ((num & 0x3000000000L) == 0L)
					{
						value.Inbound = num;
					}
					ComAirChannel.ComAirTick(value);
					break;
				case TxWaitRxMode.Normal:
					ComAirChannel.ComAirSendEvent(num & 0x1F, CommsVolume);
					m_PauseEvent.WaitOne(1000);
					ComAirChannel.ComAirSendEvent(num2 | 0x20, CommsVolume);
					m_PauseEvent.WaitOne(1750);
					if (!InternalReceive(value, 1000, true))
					{
						ComAirChannel.ComAirTick(value);
					}
					break;
				default:
					ComAirChannel.ComAirSendEvent(num & 0x1F, CommsVolume);
					m_PauseEvent.WaitOne(1000);
					ComAirChannel.ComAirSendEvent(num2 | 0x20, CommsVolume);
					m_PauseEvent.WaitOne(1000);
					break;
				}
				m_PendingTone = null;
				return true;
			}
			return false;
		}

		public bool Transmit(Tone tone)
		{
			lock (this)
			{
				if (!IsBusy)
				{
					m_PendingTone = tone;
					return true;
				}
				return false;
			}
		}

		public static bool Boost(long data)
		{
			return Convert.ToBoolean(data & long.MinValue);
		}

		public static bool ParityEven(long data)
		{
			return Valid(data) & !Convert.ToBoolean(data & 0x20);
		}

		public static bool ParityOdd(long data)
		{
			return Valid(data) & Convert.ToBoolean(data & 0x20);
		}

		public static bool Valid(long data)
		{
			return data != -1;
		}
	}
}
