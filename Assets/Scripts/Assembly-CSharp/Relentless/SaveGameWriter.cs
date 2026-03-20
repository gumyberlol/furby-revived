using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Relentless
{
	public class SaveGameWriter
	{
		private MemoryStream m_memoryStream;

		private BinaryWriter m_writer;

		public SaveGameWriter()
		{
			m_memoryStream = new MemoryStream();
			m_writer = new BinaryWriter(m_memoryStream);
		}

		public string GetWrittenString()
		{
			m_writer.Close();
			m_memoryStream.Close();
			return Convert.ToBase64String(m_memoryStream.GetBuffer());
		}

		public void WriteFloat(float val)
		{
			m_writer.Write(val);
		}

		public void WriteVector3(Vector3 val)
		{
			m_writer.Write(val.x);
			m_writer.Write(val.y);
			m_writer.Write(val.z);
		}

		public void WriteInt(int val)
		{
			m_writer.Write(val);
		}

		public void WriteString(string val)
		{
			m_writer.Write(val);
		}

		public void WriteBool(bool val)
		{
			m_writer.Write(val);
		}

		public void WriteDictionary<A, B>(Dictionary<A, B> dictionary, Func<A, SaveGameWriter, bool> writeFunctionA, Func<B, SaveGameWriter, bool> writeFunctionB)
		{
			WriteInt(dictionary.Count);
			foreach (KeyValuePair<A, B> item in dictionary)
			{
				writeFunctionA(item.Key, this);
				writeFunctionB(item.Value, this);
			}
		}
	}
}
