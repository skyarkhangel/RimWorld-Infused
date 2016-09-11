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
			if (Current.ProgramState != ProgramState.MapPlaying)
			{
				return;
			}

			if (Find.Map.components.FindAll(c => c.GetType() == infusionManager).Count == 0)
			{
				Find.Map.components.Add((MapComponent) Activator.CreateInstance (infusionManager));

				Log.Message("Infused :: Added an infusion manager to the map.");
			}

			Destroy(this);
		}
	}
}

