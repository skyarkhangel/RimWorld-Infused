using Infused;
using RimWorld;
using Verse;

namespace Infused
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
