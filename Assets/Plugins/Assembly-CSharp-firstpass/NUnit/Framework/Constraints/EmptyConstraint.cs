using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EmptyConstraint : Constraint
	{
		private Constraint RealConstraint
		{
			get
			{
				if (actual is string)
				{
					return new EmptyStringConstraint();
				}
				if (actual is DirectoryInfo)
				{
					return new EmptyDirectoryContraint();
				}
				return new EmptyCollectionConstraint();
			}
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return RealConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			RealConstraint.WriteDescriptionTo(writer);
		}
	}
}
