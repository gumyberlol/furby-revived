using System.Text;
using JsonFx.Serialization;

namespace JsonFx.Model
{
	public abstract class ModelWriterRS : DataWriterModelTokenTypeRS
	{
		public override Encoding ContentEncoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		protected ModelWriterRS(DataWriterSettingsRS settings)
			: base(settings)
		{
		}

		protected override IObjectWalker<ModelTokenType> GetWalker()
		{
			return new ModelWalkerRS(Settings);
		}
	}
}
