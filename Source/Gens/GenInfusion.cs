using System;
using System.Linq;

using RimWorld;
using Verse;

namespace Infused
{
	public static class GenInfusion
	{
		public static float GetInfusionChance( Thing thing, QualityCategory qc ) {
			var infunsionChanceDef = (
				from chance in DefDatabase< ChanceDef >.AllDefs.Reverse()
				where chance.Allows(thing)
				select chance
			).FirstOrDefault();

			if (infunsionChanceDef == null) {
				return 0f;
			}

			return infunsionChanceDef.Chance (qc);
		}

		public static InfusionTier GetTier( QualityCategory qc, float multiplier )
		{
			var rand = Rand.Value;
			if ( rand < 0.02*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Artifact;
			}
			if ( rand < 0.045*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Legendary;
			}
			if ( rand < 0.09*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Epic;
			}
			if ( rand < 0.18*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Rare;
			}
			if ( rand < 0.5*QualityMultiplier( qc )*multiplier )
			{
				return InfusionTier.Uncommon;
			}
			return InfusionTier.Common;
		}

		private static float QualityMultiplier( QualityCategory qc )
		{
			return (int) qc/3f;
		}

		/// <summary>
		/// Set parameter targInf to thing's CompInfusion's infusions. Set targInf to null when there is no CompInfusion, or the comp is not infused.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="targInf"></param>
		/// <returns></returns>
		public static bool TryGetInfusions( this Thing thing, out InfusionSet targInf )
		{
			var comp = thing.TryGetComp< CompInfusion >();
			if ( comp == null )
			{
				targInf = InfusionSet.Empty;
				return false;
			}
			targInf = comp.Infusions;
			return comp.Infused;
		}


	}
}
