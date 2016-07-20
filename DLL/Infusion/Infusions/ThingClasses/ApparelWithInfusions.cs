using Infusion;
using RimWorld;
using Verse;

namespace Infusion
{
    internal class ApparelWithInfusions : Apparel
    {
		public override string LabelNoCount
        {
            get
            {
                QualityCategory qc;
                if ( !this.TryGetQuality( out qc ) ||
                     this.TryGetComp< CompInfusion >() == null ||
                     !this.TryGetComp< CompInfusion >().Infused )
                {
					return base.LabelNoCount;
                }

                return this.GetInfusedLabel();
            }
        }
    }
}
