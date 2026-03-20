using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Relentless
{
	public class SaveGameReader
	{
		private BinaryReader m_reader;

		public SaveGameReader(string dataIn)
		{
			byte[] buffer = Convert.FromBase64String(dataIn);
			m_reader = new BinaryReader(new MemoryStream(buffer));
		}

		public float ReadFloat()
		{
			return m_reader.ReadSingle();
		}

		public Vector3 ReadVector3()
		{
			return new Vector3(m_reader.ReadSingle(), m_reader.ReadSingle(), m_reader.ReadSingle());
		}

		public int ReadInt()
		{
			return m_reader.ReadInt32();
		}

		public string ReadString()
		{
			return m_reader.ReadString();
		}

		public bool ReadBool()
		{
			return m_reader.ReadBoolean();
		}

		public void ReadDictionary<A, B>(Dictionary<A, B> dictionary, Func<SaveGameReader, A> readFunctionA, Func<SaveGameReader, B> readFunctionB)
		{
			int num = ReadInt();
			dictionary.Clear();
			for (int i = 0; i < num; i++)
			{
				A key = readFunctionA(this);
				B value = readFunctionB(this);
				dictionary[key] = value;
			}
		}
	}
}
