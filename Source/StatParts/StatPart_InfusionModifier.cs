using System.Diagnostics.CodeAnalysis;
using System.Text;
using RimWorld;
using Verse;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Infused
{
    //Modifier will change item's stats.
	
    public class StatPart_InfusionModifier : StatPart
    {
		public StatPart_InfusionModifier(StatDef parentStat) {
			this.parentStat = parentStat;
		}

		public override void TransformValue( StatRequest req, ref float val )
		{
			if (!req.HasThing) {
				return;
			}
			if (req.Thing.def.race != null && req.Thing.def.race.Humanlike) {
				TransformValueForPawn (req, ref val);
			} else if (req.Thing.def.HasComp( typeof(CompInfusion) )) {
				TransformValueForThing (req, ref val);
			}
		}

		public void TransformValueForPawn( StatRequest req, ref float val )
		{
			var pawn = req.Thing as Pawn;
			//Just in case
			if ( pawn == null )
			{
				return;
			}

			//Pawn has a primary weapon
			if ( pawn.equipment.Primary != null )
			{
				InfusionSet inf;
				if (pawn.equipment.Primary.TryGetInfusions( out inf ))
				{
					TransformValue ( inf, ref val );
				}
			}

			//Pawn has apparels
			foreach ( var current in pawn.apparel.WornApparel )
			{
				InfusionSet inf;
				if (current.TryGetInfusions( out inf ))
				{
					TransformValue ( inf, ref val );
				}
			}
		}

		private void TransformValueForThing( StatRequest req, ref float val )
		{
			InfusionSet inf;
			if ( !req.Thing.TryGetInfusions( out inf ) )
			{
				return;
			}

			TransformValue (inf, ref val);
		}

		private void TransformValue(InfusionSet inf, ref float val)
		{
			StatMod mod;

			var prefix = inf.prefix;
			var suffix = inf.suffix;

			if (prefix != null && prefix.TryGetStatValue( parentStat, out mod ))
			{
				val += mod.offset;
				val *= mod.multiplier;
			}
			if (suffix != null && suffix.TryGetStatValue( parentStat, out mod ))
			{
				val += mod.offset;
				val *= mod.multiplier;
			}
		}

		public override string ExplanationPart( StatRequest req ) {
			if ( !req.HasThing )
			{
				return null;
			}

			if (req.Thing.def.race != null && req.Thing.def.race.Humanlike) {
				return ExplanationPartForPawn (req);
			} else if (req.Thing.def.HasComp( typeof(CompInfusion) )) {
				return ExplanationPartForThing (req);
			}

			return null;
		}

		private string ExplanationPartForPawn( StatRequest req )
		{
			//Just in case
			var pawn = req.Thing as Pawn;
			if ( pawn == null )
			{
				return null;
			}

			InfusionSet infusions;
			var result = new StringBuilder();

			result.AppendLine(ResourceBank.StringInfusionDescBonus);

			//Pawn has a primary weapon
			if ( pawn.equipment.Primary.TryGetInfusions( out infusions ) )
			{
				result.Append( WriteExplanation( pawn.equipment.Primary, infusions ) );
			}

			//Pawn has apparels
			foreach ( var current in pawn.apparel.WornApparel )
			{
				if ( current.TryGetInfusions( out infusions ) )
				{
					result.Append( WriteExplanation( current, infusions ) );
				}
			}

			return result.ToString();
		}


        private string ExplanationPartForThing( StatRequest req )
        {
            InfusionSet infusions;
            return req.Thing.TryGetInfusions( out infusions )
                ? WriteExplanation( req.Thing, infusions )
                : null;
        }

        private string WriteExplanation( Thing thing, InfusionSet infusions )
        {
            var result = new StringBuilder();

			if ( infusions.prefix != null )
            {
                result.Append( WriteExplanationDetail( thing, infusions.prefix.defName ) );
            }
			if ( infusions.suffix != null )
            {
				result.Append( WriteExplanationDetail( thing, infusions.suffix.defName ) );
            }
            return result.ToString();
        }

        private string WriteExplanationDetail( Thing infusedThing, string val )
        {
			StatMod mod;
			var inf = Def.Named(val);
			var result = new StringBuilder();

			//No mod for this stat
			if ( !inf.TryGetStatValue( parentStat, out mod ) )
			{
				return null;
			}

			if ( mod.offset.FloatEqual( 0 ) && mod.multiplier.FloatEqual( 1 ) )
			{
				return null;
			}

			if ( mod.offset.FloatNotEqual( 0 ) )
			{
				result.Append( "    " + inf.label.CapitalizeFirst() + ": " );
				result.Append( mod.offset > 0 ? "+" : "-" );
				result.AppendLine( parentStat.ValueToString (mod.offset.ToAbs ()) );
			}
			if ( mod.multiplier.FloatNotEqual( 1 ) )
			{
				result.Append( "    " + inf.label.CapitalizeFirst() + ": x" );
				result.AppendLine( mod.multiplier.ToStringPercent() );
			}

            return result.ToString();
        }
    }
}
