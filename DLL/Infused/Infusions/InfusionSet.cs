using System;

namespace Infused
{
	public class InfusionSet : IEquatable<InfusionSet>//, IExposable
	{
		public bool Equals(InfusionSet other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Prefix, other.Prefix) && string.Equals(Suffix, other.Suffix);
		}

		public string Prefix;
		public string Suffix;

		public bool PassPre {
			get {return Prefix == null; }
		}
		public bool PassSuf {
			get {return Suffix == null; }
		}

		public InfusionSet(string pre, string suf)
		{
			Prefix = pre;
			Suffix = suf;
		}

		public static InfusionSet Empty = new InfusionSet(null, null);
	}
}