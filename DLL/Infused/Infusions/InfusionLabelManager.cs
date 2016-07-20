using System.Collections.Generic;
using Infused;

namespace Infused
{
	public static class InfusionLabelManager
	{
		public static List<CompInfusion> Drawee { get; set; }

		static InfusionLabelManager()
		{
			Drawee = new List<CompInfusion>();
		}

		public static void ReInit()
		{
			Drawee.Clear();
		}
		public static void Register(CompInfusion compInfusion)
		{
			Drawee.Add(compInfusion);
		}

		public static void DeRegister(CompInfusion compInfusion)
		{
			Drawee.Remove(compInfusion);
		}
	}
}
