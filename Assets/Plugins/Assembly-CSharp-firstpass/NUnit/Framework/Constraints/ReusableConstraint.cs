namespace NUnit.Framework.Constraints
{
	public class ReusableConstraint : IResolveConstraint
	{
		private Constraint constraint;

		public ReusableConstraint(IResolveConstraint c)
		{
			constraint = c.Resolve();
		}

		public override string ToString()
		{
			return constraint.ToString();
		}

		public Constraint Resolve()
		{
			return constraint;
		}

		public static implicit operator ReusableConstraint(Constraint c)
		{
			return new ReusableConstraint(c);
		}
	}
}
