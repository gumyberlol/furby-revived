using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JsonFx.Model;

namespace JsonFx.Serialization
{
	public abstract class DataWriterModelTokenTypeRS : IDataWriterRS
	{
		private readonly DataWriterSettingsRS settings;

		public abstract Encoding ContentEncoding { get; }

		public abstract IEnumerable<string> ContentType { get; }

		public abstract IEnumerable<string> FileExtension { get; }

		public DataWriterSettingsRS Settings
		{
			get
			{
				return settings;
			}
		}

		protected DataWriterModelTokenTypeRS(DataWriterSettingsRS settings)
		{
			if (settings == null)
			{
				throw new NullReferenceException("settings");
			}
			this.settings = settings;
		}

		public virtual void Write(object data, TextWriter output)
		{
			IObjectWalker<ModelTokenType> walker = GetWalker();
			if (walker == null)
			{
				throw new ArgumentNullException("walker");
			}
			ITextFormatter<ModelTokenType> formatter = GetFormatter();
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
			IObjectWalker<ModelTokenType> walker = GetWalker();
			if (walker == null)
			{
				throw new ArgumentNullException("walker");
			}
			ITextFormatter<ModelTokenType> formatter = GetFormatter();
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

		protected abstract IObjectWalker<ModelTokenType> GetWalker();

		protected abstract ITextFormatter<ModelTokenType> GetFormatter();
	}
}
