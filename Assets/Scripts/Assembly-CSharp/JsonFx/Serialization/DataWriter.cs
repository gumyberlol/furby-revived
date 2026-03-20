using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JsonFx.Serialization
{
	public abstract class DataWriter<T> : IDataWriter
	{
		private readonly DataWriterSettings settings;

		public abstract Encoding ContentEncoding { get; }

		public abstract IEnumerable<string> ContentType { get; }

		public abstract IEnumerable<string> FileExtension { get; }

		public DataWriterSettings Settings
		{
			get
			{
				return settings;
			}
		}

		protected DataWriter(DataWriterSettings settings)
		{
			if (settings == null)
			{
				throw new NullReferenceException("settings");
			}
			this.settings = settings;
		}

		public virtual void Write(object data, TextWriter output)
		{
			IObjectWalker<T> walker = GetWalker();
			if (walker == null)
			{
				throw new ArgumentNullException("walker");
			}
			ITextFormatter<T> formatter = GetFormatter();
			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}
			try
			{
				formatter.Format(walker.GetTokens(data), output);
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new SerializationException(ex2.Message, ex2);
			}
		}

		public virtual string Write(object data)
		{
			IObjectWalker<T> walker = GetWalker();
			if (walker == null)
			{
				throw new ArgumentNullException("walker");
			}
			ITextFormatter<T> formatter = GetFormatter();
			if (formatter == null)
			{
				throw new ArgumentNullException("formatter");
			}
			try
			{
				return formatter.Format(walker.GetTokens(data));
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new SerializationException(ex2.Message, ex2);
			}
		}

		protected abstract IObjectWalker<T> GetWalker();

		protected abstract ITextFormatter<T> GetFormatter();
	}
}
