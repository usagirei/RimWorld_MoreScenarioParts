using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using RString = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{

    public class OnPawnDeathExplodesModifier : ScenPartEx_PawnModifier
    {
        private float radius = 5.9f;
        private DamageDef damage;
        private string radiusBuf;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<DamageDef>(ref damage, nameof(damage));
            Scribe_Values.Look<float>(ref radius, nameof(radius), 0, false);
        }

        private IEnumerable<DamageDef> PossibleDamageDefs()
        {
            return DefDatabase<DamageDef>.AllDefs.Where(d => d.explosionCellMote != null);
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, ScenPart.RowHeight * 6);
            Rect[] rows = rect.SplitRows(1, 1, 4);
            Rect[] r_rad = rows[1].SplitCols(1, 2);

            if (Widgets.ButtonText(rows[0], damage.LabelCap, true, false, true))
                FloatMenuUtility.MakeMenu(PossibleDamageDefs(), (DamageDef d) => d.LabelCap, (d) => () => damage = d);

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(r_rad[0], RString.MSP_Radius);
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.TextFieldNumeric(r_rad[1], ref radius, ref radiusBuf, 1);

            DoContextEditInterface(rows[2]);
        }

        public override void Randomize()
        {
            base.Randomize();
            this.damage = PossibleDamageDefs().RandomElement();
            this.radius = Rand.RangeInclusive(3, 8) - 0.1f;
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            return true;
        }

        protected override void ModifyDeadPawn(Corpse corpse, bool humanLike)
        {
            if (corpse.Spawned)
            {
                GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, damage, null, explosionSound: damage.soundExplosion);
            }
        }
    }
}