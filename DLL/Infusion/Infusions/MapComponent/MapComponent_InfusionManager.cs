using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Infusion
{
    public class MapComponent_InfusionManager : MapComponent
    {
        private static HashSet<TradeShip> shipDict = new HashSet< TradeShip >(); 
        private bool welfare = true;

        public override void MapComponentTick()
        {
            if ( welfare == false )
                return;

            //Execute every 12 ticks
            if (Find.TickManager.TicksGame%12 != 0)
                return;

            try
            {
                InfuseEquipmentsOnMap();
                InfuseTraderStock();
            }
            catch ( Exception e )
            {
				Log.Error( e.ToString() );
				Log.Warning("Infusion: InfusionManager met an error. Hibernating.");
				welfare = false;
            }
        }
        public override void MapComponentOnGUI()
        {
            Draw();
        }

        //Infuse items of raiders and visitors, etc
        private static void InfuseEquipmentsOnMap()
        {
            foreach (var pawn in Find.MapPawns.AllPawns)
            {
                // No humanlike neither mechanoid, pass
                if (!pawn.def.race.Humanlike && !pawn.def.race.IsMechanoid)
                    continue;

                InfuseWeapon( pawn );
                InfuseApparels( pawn );
            }
        }

        private static void InfuseWeapon(Pawn pawn)
        {
            //Pawn has primary
			if (pawn.equipment.Primary != null) {
				var compInfusion = pawn.equipment.Primary.TryGetComp<CompInfusion> ();
				if (compInfusion != null && ! compInfusion.tried) {
					compInfusion.SetInfusion();
					compInfusion.tried = true;
				}
			}
        }
        private static void InfuseApparels( Pawn pawn )
        {
            //Pawn has apparel
            if ( pawn.apparel.WornApparelCount == 0 )
            {
                return;
            }

            foreach ( var curApparel in pawn.apparel.WornApparel )
            {
                var compInfusion = curApparel.TryGetComp<CompInfusion>();
                if ( compInfusion == null || compInfusion.tried )
                {
                    continue;
                }

                compInfusion.SetInfusion();
                compInfusion.tried = true;
            }
        }
        private static void InfuseTraderStock()
        {
            var ships = Find.PassingShipManager.passingShips;
            if ( !ships.Any() )
                return;

            foreach ( var ship in ships.Cast<TradeShip>() )
            {
                if ( shipDict.Contains( ship ) )
                    continue;

                var field = typeof(TradeShip).GetField( "things", BindingFlags.NonPublic | BindingFlags.Instance);

				if (field != null) {
					var stock = field.GetValue (ship) as List< Thing >;
					if (stock != null) {
						foreach ( var current in stock )
						{
							var compInfusion = current.TryGetComp< CompInfusion >();
							if ( compInfusion == null || compInfusion.tried )
								continue;

							compInfusion.SetInfusion(  );
						}
					} else {
#if DEBUG
						Log.Error("Infusion: Could not get value from field");
#endif
						continue;
					}
				}
 
                if(shipDict.Count > 5)
                    shipDict.Clear();

                shipDict.Add(ship);
            }
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
                var prefix = current.Infusions.Prefix.ToInfusionDef();
                var suffix = current.Infusions.Suffix.ToInfusionDef();

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
