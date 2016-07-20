using RimWorld;
using Verse;

namespace Infused
{
	class ThingWithInfusions : ThingWithComps
	{
		public override string LabelNoCount
		{
			get
			{
				QualityCategory qc;
				if (!this.TryGetQuality(out qc) ||
					this.TryGetComp<CompInfusion>() == null ||
					!this.TryGetComp<CompInfusion>().Infused)
					return base.LabelNoCount;

				return this.GetInfusedLabel();
			}
		}
	}
}
