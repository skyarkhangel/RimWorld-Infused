using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using RimWorld;
using Verse;

namespace Infused
{
	[StaticConstructorOnStartup]
	public class ModInjector
	{
		public static bool hasRun;

		public static void Inject()
		{
			if (hasRun) {
				return;
			}

			MethodInfo Rimworld_CompQuality_SetQuality = typeof(CompQuality).GetMethod ("SetQuality", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo Infused_CompQuality_SetQuality = typeof(ModDetour).GetMethod ("_SetQuality", BindingFlags.Static | BindingFlags.NonPublic);
			if (!Detours.TryDetourFromTo (Rimworld_CompQuality_SetQuality, Infused_CompQuality_SetQuality)) {
				Log.Error ("Infused :: Failed to Detour");

				hasRun = true;

				return;
			}

			// until CCL for A15 appears...
			GameObject initializer = new GameObject("InfusionMapComponentInjector");
			initializer.AddComponent<MapComponentInjector>();
			UnityEngine.Object.DontDestroyOnLoad(initializer);

			var defs = (
				from def in DefDatabase< ThingDef >.AllDefs
				where (def.IsMeleeWeapon || def.IsRangedWeapon || def.IsApparel)
				&& def.HasComp( typeof(CompQuality) ) && ! def.HasComp( typeof(CompInfusion) )
				select def
			);

			var tabType = typeof(ITab_Infusion);
			var tab = ITabManager.GetSharedInstance ( tabType );
			var compProperties = new CompProperties { compClass = typeof (CompInfusion) };

			foreach(var def in defs) {
				def.comps.Add (compProperties);
#if DEBUG
				Log.Message ("Infused :: Component added to " + def.label);
#endif

				if ( def.inspectorTabs == null || def.inspectorTabs.Count == 0 )
				{
					def.inspectorTabs = new List< Type >();
					def.inspectorTabsResolved = new List< ITab >();
				}

				def.inspectorTabs.Add (tabType);
				def.inspectorTabsResolved.Add (tab);
			}
#if DEBUG
			Log.Message ("Infused :: Injected");
#endif

			hasRun = true;
		}

	}
}

