using System;

using Verse;

namespace Infused
{
	public class InfusionSet : IEquatable<InfusionSet>//, IExposable
	{
		public bool Equals(InfusionSet other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(prefix, other.prefix) && string.Equals(suffix, other.suffix);
		}

		public Def prefix;
		public Def suffix;

		public InfusionSet(Def prefix, Def suffix)
		{
			this.prefix = prefix;
			this.suffix = suffix;
		}

		public static InfusionSet Empty = new InfusionSet(null, null);
	}
}