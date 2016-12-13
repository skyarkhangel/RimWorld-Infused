using System;

using UnityEngine;
using Verse;

namespace Infused
{
	public class MapComponentInjector : MonoBehaviour
	{
		private static Type infusionManager = typeof(MapComponent_InfusionManager);

		public void FixedUpdate()
		{
			if (Current.ProgramState != ProgramState.Playing)
			{
				return;
			}

			if (Find.VisibleMap.components.FindAll(c => c.GetType() == infusionManager).Count == 0)
			{
				Find.VisibleMap.components.Add((MapComponent) Activator.CreateInstance (infusionManager, Find.VisibleMap));

				Log.Message("Infused :: Added an infusion manager to the map.");
			}

			Destroy(this);
		}
	}
}

