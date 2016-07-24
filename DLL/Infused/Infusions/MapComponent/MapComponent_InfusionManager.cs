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
                var prefix = inf.Prefix.ToInfusionDef();
                var suffix = inf.Suffix.ToInfusionDef();

                Color color;
                //When there is only suffix
                if (inf.PassPre)
                {
                    color = suffix.tier.InfusionColor();
                }
                //When there is only prefix
                else if (inf.PassSuf)
                {
                    color = prefix.tier.InfusionColor();
                }
                //When there are both prefix and suffix
                else
                {
                    color = MathUtility.Max(prefix.tier, suffix.tier).InfusionColor();
                }
                var result = new StringBuilder();
                if (!inf.PassPre)
                {
                    result.Append(prefix.labelShort);
                    if (!inf.PassSuf)
                        result.Append(" ");
                }
                if (!inf.PassSuf)
                    result.Append(suffix.labelShort);

                GenWorldUI.DrawThingLabel(
                  GenWorldUI.LabelDrawPosFor(current.parent, -0.66f), result.ToString(), color);
            }
        }
    }
}
