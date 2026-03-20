using System;
using System.Collections.Generic;
using System.Threading;

namespace Relentless
{
	public class ProcessOnThread<IN, OUT> : IDisposable
	{
		public delegate OUT ProcessFunction(IN data);

		private ProcessFunction m_function;

		private Queue<IN> m_inputQueue = new Queue<IN>();

		private Mutex m_inputMutex = new Mutex();

		private Queue<OUT> m_outputQueue = new Queue<OUT>();

		private Mutex m_outputMutex = new Mutex();

		private ManualResetEvent m_dataInInputQueue;

		private Thread m_workerThread;

		private int m_waitingCount;

		public int WaitingCount
		{
			get
			{
				return m_waitingCount;
			}
		}

		public ProcessOnThread(ProcessFunction function)
		{
			m_function = function;
			m_dataInInputQueue = new ManualResetEvent(false);
			m_workerThread = new Thread(WorkerThread);
			m_waitingCount = 0;
		}

		~ProcessOnThread()
		{
			Stop();
		}

		public void Dispose()
		{
			Stop();
		}

		public void Start()
		{
			m_workerThread.Start();
		}

		public void Stop()
		{
			if (m_workerThread != null)
			{
				m_workerThread.Abort();
				m_workerThread = null;
			}
		}

		public void PushJob(IN data)
		{
			m_inputMutex.WaitOne();
			m_inputQueue.Enqueue(data);
			m_inputMutex.ReleaseMutex();
			m_waitingCount++;
			m_dataInInputQueue.Set();
		}

		public bool GetResult(out OUT output)
		{
			m_outputMutex.WaitOne();
			bool flag = m_outputQueue.Count != 0;
			if (flag)
			{
				output = m_outputQueue.Dequeue();
			}
			else
			{
				output = default(OUT);
			}
			m_outputMutex.ReleaseMutex();
			return flag;
		}

		private void WorkerThread()
		{
			while (true)
			{
				m_dataInInputQueue.WaitOne();
				m_inputMutex.WaitOne();
				IN data = m_inputQueue.Dequeue();
				if (m_inputQueue.Count == 0)
				{
					m_dataInInputQueue.Reset();
				}
				m_inputMutex.ReleaseMutex();
				OUT item = m_function(data);
				m_outputMutex.WaitOne();
				m_waitingCount--;
				m_outputQueue.Enqueue(item);
				m_outputMutex.ReleaseMutex();
			}
		}
	}
}
