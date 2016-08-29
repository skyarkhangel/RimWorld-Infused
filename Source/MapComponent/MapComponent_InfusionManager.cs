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
        public MapComponent_InfusionManager ()
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
                var inf = current.Infusions;
                var prefix = inf.prefix;
                var suffix = inf.suffix;

                Color color;
                //When there is only suffix
				if (suffix != null)
                {
                    color = suffix.tier.InfusionColor();
                }
                //When there is only prefix
				else if (prefix != null)
                {
                    color = prefix.tier.InfusionColor();
                }
                //When there are both prefix and suffix
                else
                {
                    color = MathUtility.Max(prefix.tier, suffix.tier).InfusionColor();
                }
                var result = new StringBuilder();
				if (prefix != null)
                {
                    result.Append(prefix.labelShort);
					if (suffix != null)
                        result.Append(" ");
                }
				if (suffix != null)
                    result.Append(suffix.labelShort);

                GenWorldUI.DrawThingLabel(
                  GenWorldUI.LabelDrawPosFor(current.parent, -0.66f), result.ToString(), color);
            }
        }
    }
}
