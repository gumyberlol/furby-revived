using JsonFx.Serialization;

namespace JsonFx.Model
{
	public abstract class ModelReaderRS : DataReaderModelTokenTypeRS
	{
		protected ModelReaderRS(DataReaderSettingsRS settings)
			: base(settings)
		{
		}

		protected override ITokenAnalyzerRS<ModelTokenType> GetAnalyzer()
		{
			return new ModelAnalyzerRS(Settings);
		}
	}
}
