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
				var def = comp.parent.def;
				// Get those Infusions rolling
				var prefix = roll (qc, def.techLevel);
				var suffix = roll (qc, def.techLevel);

				if (prefix)
					compInfusion.InitializeInfusionPrefix (qc, def.techLevel);
				if (suffix)
					compInfusion.InitializeInfusionSuffix (qc, def.techLevel);
				if (prefix || suffix) {
					//For additional hit points
					comp.parent.HitPoints = comp.parent.MaxHitPoints;
				}
			}
		}

		#endregion

		//Rolling for chance TODO: make it configurable
		private static bool roll(QualityCategory qc, TechLevel tech)
		{
			var lowTech = tech < TechLevel.Industrial;

			var chance = GenInfusion.GetInfusionChance( qc );
			var rand = Rand.Value;
			if ( lowTech )
			{
				rand /= 3;
			}
#if DEBUG
			Log.Message ("Infused: Rolled " + ((rand < chance) ? "success" : "failure") + " " + rand + " < " + chance + " for " + qc +  " under " + tech);
#endif
			return rand < chance;
		}
	}
}

