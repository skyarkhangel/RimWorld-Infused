﻿using Verse;

namespace Infused
{
	public static class ResourceBank
	{
		public static string Translate( this InfusionTier it )
		{
			return ("Infusion" + it).Translate();
		}

		//Mote
		public static readonly string StringInfused = "Infused".Translate();

		//Your weapon, {0}, is infused!
		public static readonly string StringInfusionMessage = "InfusionMessage";

		//{1: golden sword} of {2: stream}
		public static readonly string StringInfusionOf = "InfusionOf";

        //SK TRANSLATION FIX
        public static readonly string StringTranslationFix = "TranslationFix";

        //Infusion bonuses
        public static readonly string StringInfusionDescBonus = "InfusionDescBonus".Translate();
		public static readonly string StringInfusionDescFrom = "InfusionDescFrom";

		public static readonly string StringQuality = "InfusionQuality".Translate();
	}
}
