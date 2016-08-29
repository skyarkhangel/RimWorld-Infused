using System;
using System.Collections.Generic;
using System.Reflection;

using RimWorld;
using Verse;

namespace Infused
{
	internal static class ModDetour
	{
		#region Method Detours

		internal static void _SetQuality(this CompQuality comp, QualityCategory qc, ArtGenerationContext source)
		{
			// Need to set quality properly and initialite art if there is any since we are detouring.
			String fieldName = "qualityInt";
			Type type = comp.GetType();
			FieldInfo field = type.GetField (fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
				throw new ArgumentOutOfRangeException("fieldName", string.Format("Field {0} was not found in Type {1}", fieldName, comp.GetType().FullName));
			field.SetValue (comp, qc);

			CompArt compArt = comp.parent.TryGetComp<CompArt> ();
			if (compArt != null) {
				compArt.InitializeArt (source);
			}

			// Can we be infused?
			CompInfusion compInfusion = comp.parent.TryGetComp<CompInfusion> ();
			if (compInfusion != null) {
				var thing = comp.parent;
				var def = comp.parent.def;
				// Get those Infusions rolling
				var prefix = roll (thing, qc);
				var suffix = roll (thing, qc);

				var tierMult = def.techLevel < TechLevel.Industrial ? 3 : 1;

				if (prefix)
					compInfusion.InitializeInfusionPrefix (GenInfusion.GetTier( qc, tierMult ));
				if (suffix)
					compInfusion.InitializeInfusionSuffix (GenInfusion.GetTier( qc, tierMult ));
				if (prefix || suffix) {
					//For additional hit points
					comp.parent.HitPoints = comp.parent.MaxHitPoints;
				}
			}
		}

		#endregion

		//Rolling for chance TODO: make it configurable
		private static bool roll(Thing thing, QualityCategory qc)
		{
			var chance = GenInfusion.GetInfusionChance( thing, qc );
			var rand = Rand.Value;
#if DEBUG
			Log.Message ("Infused :: Rolled " + ((rand < chance) ? "success" : "failure") + " " + rand + " < " + chance + " for " + thing + " and " + qc);
#endif
			return rand < chance;
		}
	}
}

