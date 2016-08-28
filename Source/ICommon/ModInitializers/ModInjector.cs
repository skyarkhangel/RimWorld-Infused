using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RimWorld;
using Verse;
using CommunityCoreLibrary;

namespace Infused
{
	public class ModInjector : SpecialInjector
	{
		public override bool Inject()
		{
			// Detour RimWorld.CompQuality.SetQuality
			MethodInfo Rimworld_CompQuality_SetQuality = typeof(CompQuality).GetMethod ("SetQuality", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo Infused_CompQuality_SetQuality = typeof(ModDetour).GetMethod ("_SetQuality", BindingFlags.Static | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo (Rimworld_CompQuality_SetQuality, Infused_CompQuality_SetQuality))
				return false;

			try {
				InjectVarious ();
			} catch (Exception e) {
				Log.Error ("Infused: Met error while injecting. \n" + e);
				return false;
			}

			Log.Message ("Infused: Injected");

			return true;
		}

		//Inject every prerequisites to defs.
		private void InjectVarious()
		{
			foreach (var thingDef in DefDatabase< ThingDef >.AllDefs.Where ( def => def.IsMeleeWeapon || def.IsRangedWeapon || def.IsApparel )) 
			{
				if ( AddCompInfusion( thingDef ) )
				{
					AddInfusionITab( thingDef );
				}
			}
		}

		//Inject new ThingComp.
		private static bool AddCompInfusion( ThingDef thingDef )
		{
			if ( thingDef.comps.Exists( s => s.compClass == typeof ( CompInfusion ) ) )
			{
				Log.Message ("Infused: Component exists for " + thingDef.label);
				return false;
			}

			if ( !thingDef.comps.Exists( s => s.compClass == typeof ( CompQuality ) ) )
			{
				return false;
			}

			//As we are adding, not replacing, we need a fresh CompProperties.
			//We don't need anything except compClass as CompInfusion does not take anything.
			var compProperties = new CompProperties {compClass = typeof ( CompInfusion )};
			thingDef.comps.Insert( 0, compProperties );
#if DEBUG
			Log.Message ("Infused: Component added to " + def.label);
#endif
			return true;
		}

		//Inject new ITab to given def.
		private static void AddInfusionITab( ThingDef thingDef )
		{
			if ( thingDef.inspectorTabs == null || thingDef.inspectorTabs.Count == 0 )
			{
				thingDef.inspectorTabs = new List< Type >();
				thingDef.inspectorTabsResolved = new List< ITab >();
			}
			if ( thingDef.inspectorTabs.Contains( typeof ( ITab_Infusion ) ) )
			{
				Log.Message ("Infused: Tab exists for " + thingDef.label);
				return;
			}
			thingDef.inspectorTabs.Add( typeof ( ITab_Infusion ) );
			thingDef.inspectorTabsResolved.Add( ITabManager.GetSharedInstance( typeof ( ITab_Infusion ) ) );
		}
	}
}

