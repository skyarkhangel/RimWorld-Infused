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
			//Access ThingDef database with each def's defName.
			String fieldName = "defsByName";
			FieldInfo field = typeof(DefDatabase< ThingDef >).GetField (fieldName, BindingFlags.NonPublic | BindingFlags.Static);
			if (field == null)
				throw new ArgumentOutOfRangeException("fieldName", string.Format("Field {0} was not found in Type {1}", fieldName, typeof(DefDatabase< ThingDef >).FullName));
			var defsByName = field.GetValue( null ) as Dictionary< string, ThingDef >;
			if ( defsByName == null )
				throw new Exception( "Could not access private members" );
			foreach (var current in defsByName.Values.Where(current => current.IsMeleeWeapon || current.IsRangedWeapon || current.IsApparel)) 
			{
				if ( AddCompInfusion( current ) )
				{
					AddInfusionITab( current );
				}
			}
		}

		//Inject new ThingComp.
		private static bool AddCompInfusion( ThingDef def )
		{
			if ( def.comps.Exists( s => s.compClass == typeof ( CompInfusion ) ) )
			{
				Log.Message ("Infused: Component exists for " + def.label);
				return false;
			}

			if ( !def.comps.Exists( s => s.compClass == typeof ( CompQuality ) ) )
			{
				return false;
			}

			//As we are adding, not replacing, we need a fresh CompProperties.
			//We don't need anything except compClass as CompInfusion does not take anything.
			var compProperties = new CompProperties {compClass = typeof ( CompInfusion )};
			def.comps.Insert( 0, compProperties );
#if DEBUG
			Log.Message ("Infused: Component added to " + def.label);
#endif
			return true;
		}

		//Inject new ITab to given def.
		private static void AddInfusionITab( ThingDef def )
		{
			if ( def.inspectorTabs == null || def.inspectorTabs.Count == 0 )
			{
				def.inspectorTabs = new List< Type >();
				def.inspectorTabsResolved = new List< ITab >();
			}
			if ( def.inspectorTabs.Contains( typeof ( ITab_Infusion ) ) )
			{
				Log.Message ("Infused: Tab exists for " + def.label);
				return;
			}
			def.inspectorTabs.Add( typeof ( ITab_Infusion ) );
			def.inspectorTabsResolved.Add( ITabManager.GetSharedInstance( typeof ( ITab_Infusion ) ) );
		}
	}
}

