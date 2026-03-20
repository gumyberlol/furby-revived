using System.Text;
using JsonFx.Serialization;

namespace JsonFx.Model
{
	public abstract class ModelWriter : DataWriter<ModelTokenType>
	{
		public override Encoding ContentEncoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		protected ModelWriter(DataWriterSettings settings)
			: base(settings)
		{
		}

		protected override IObjectWalker<ModelTokenType> GetWalker()
		{
			return new ModelWalker(Settings);
		}
	}
}
