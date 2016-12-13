using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infused
{
    public class MapComponent_InfusionManager : MapComponent
    {
		public MapComponent_InfusionManager (Map map) : base(map)
        {
            // Clear old labels
            InfusionLabelManager.ReInit();
        }

        public override void MapComponentOnGUI()
        {
            Draw();
        }

        //Draw infusion label on map
        private static void Draw()
        {
            if (Find.CameraDriver.CurrentZoom != CameraZoomRange.Closest) return;
            if (InfusionLabelManager.Drawee.Count == 0)
                return;

            foreach (var current in InfusionLabelManager.Drawee)
            {
				// skip fogged
				if (Find.VisibleMap.fogGrid.IsFogged (current.parent.Position)) {
					continue;
				}

				GenMapUI.DrawThingLabel( GenMapUI.LabelDrawPosFor(current.parent, -0.66f), current.InfusedLabel, current.InfusedLabelColor );
            }
        }
    }
}
