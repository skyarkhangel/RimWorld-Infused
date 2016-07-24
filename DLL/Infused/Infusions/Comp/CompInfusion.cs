using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Infused
{
	public class CompInfusion : ThingComp
	{
		public bool Infused {
			get { return prefix != null || suffix != null; }
		}

		public InfusionSet Infusions {
			get { return new InfusionSet (prefix, suffix); }
		}

		// Is our infusion newly created? Only for notifications.
		private bool isNew;

		private string prefix, suffix;

		private static readonly SoundDef InfusionSound = SoundDef.Named( "Infusion_Infused" );

		public void InitializeInfusionPrefix(QualityCategory qc, TechLevel tech)
		{
			var tierMult = tech < TechLevel.Industrial ? 3 : 1;

			InfusionDef preTemp;
			var tier = GenInfusion.GetTier (qc, tierMult);
			if (
				!(
				    from t in DefDatabase< InfusionDef >.AllDefsListForReading
				 where
				     t.tier == tier &&
				     t.type == InfusionType.Prefix &&
				     t.MatchItemType (parent.def)
				 select t
				).TryRandomElement (out preTemp)) {
				//No infusion available from defs
				Log.Warning ("Infused: Couldn't find any prefixed InfusionDef! Tier: " + tier);
				prefix = null;
			} else {
				prefix = preTemp.defName;
			}

			isNew = true;
		}
		public void InitializeInfusionSuffix(QualityCategory qc, TechLevel tech)
		{
			var tierMult = tech < TechLevel.Industrial ? 3 : 1;

			InfusionDef preTemp;
			var tier = GenInfusion.GetTier( qc, tierMult );
			if ( !
				(from t in DefDatabase< InfusionDef >.AllDefs.ToList()
				 where
					 t.tier == tier &&
					 t.type == InfusionType.Suffix &&
					 t.MatchItemType( parent.def )
				 select t
					).TryRandomElement( out preTemp ) )
			{
				//No infusion available from defs
				Log.Warning( "Infused: Couldn't find any suffixed InfusionDef! Tier: " + tier );
				suffix = null;
			}
			else
			{
				suffix = preTemp.defName;
			}

			isNew = true;
		}

		private void throwMote()
		{
			CompQuality compQuality = parent.TryGetComp<CompQuality> ();
			if (compQuality == null) {
				return;
			}
			string qualityLabel = compQuality.Quality.GetLabel();

			var msg = new StringBuilder();
			msg.Append( qualityLabel + " " );
			if ( parent.Stuff != null )
			{
				msg.Append( parent.Stuff.LabelAsStuff + " " );
			}
			msg.Append( parent.def.label );
			Messages.Message( ResourceBank.StringInfusionMessage.Translate( msg ), MessageSound.Silent );
			InfusionSound.PlayOneShotOnCamera();
			MoteThrower.ThrowText( parent.Position.ToVector3Shifted(), ResourceBank.StringInfused,
			                       GenInfusionColor.Legendary );

			isNew = false;
		}

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();

			if (Infused) {
				InfusionLabelManager.Register (this);

				// We only throw notifications for newly spawned items.
				if (isNew)
					throwMote ();
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();

			Scribe_Values.LookValue( ref prefix, "prefix", null );
			Scribe_Values.LookValue( ref suffix, "suffix", null );

#if DEBUG
			if ( (prefix != null && prefix.ToInfusionDef() == null) || (suffix != null && suffix.ToInfusionDef() == null) )
			{

				Log.Warning( "Infused: Could not find some of InfusionDef." + prefix + "/" + suffix );
			}
#endif
		}

		public override void PostDeSpawn()
		{
			base.PostDeSpawn();

			if ( Infused )
			{
				InfusionLabelManager.DeRegister( this );
			}
		}

		public override bool AllowStackWith( Thing other )
		{
			return false;
		}

		public override string GetDescriptionPart()
		{
			return base.GetDescriptionPart() + "\n" + parent.GetInfusionDesc();
		}

		public override string TransformLabel (string label)
		{
			// When this function is called, our infusion is no longer new.
			isNew = false;

			if (Infused) {
				return parent.GetInfusedLabel ();
			} else {
				return base.TransformLabel (label);
			}
		}
	}
}
