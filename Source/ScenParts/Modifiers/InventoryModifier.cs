﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using RString = R.MoreScenarioParts.String;

namespace More_Scenario_Parts.ScenParts
{
    public class InventoryModifier : ScenPartEx_PawnModifier
    {
        private ThingDef thing;
        private ThingDef stuff;
        private ThingKind thingKind;
        private IntRange amount;
        private bool equip;

        public override void ExposeData()
        {

            base.ExposeData();
            Scribe_Defs.Look(ref thing, nameof(thing));
            Scribe_Defs.Look(ref stuff, nameof(stuff));
            Scribe_Values.Look(ref thingKind, nameof(thingKind));
            Scribe_Values.Look(ref amount, nameof(amount), new IntRange(1, 10));
            Scribe_Values.Look(ref equip, nameof(equip));
        }

        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect rect = listing.GetScenPartRect(this, RowHeight * 8 + 31f);
            Rect[] a = rect.SplitRows(5, 4);
            Rect[] rows = a[0].SplitRows(RowHeight, RowHeight, RowHeight, 31f, RowHeight);

            Rect[] r_kind = rows[0].SplitCols(1, 2);
            Rect[] r_thing = rows[1].SplitCols(1, 2);
            Rect[] r_stuff = rows[2].SplitCols(1, 2);
            Rect[] r_amount = rows[3].SplitCols(1, 2);
            Rect r_equip = rows[4]; 

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(r_kind[0], RString.MSP_ThingKind);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(r_kind[1], thingKind.Translate()))
            {
                FloatMenuUtility.MakeMenu(Utilities.GetEnumValues<ThingKind>(), ScenPartExtensions.Translate, (k) => () =>
                {
                    thingKind = k;
                    thing = GetThings().RandomElement();
                    if (thing.MadeFromStuff)
                    {
                        stuff = GetStuffsForThing().RandomElement();
                    }
                });
            }

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(r_thing[0], RString.MSP_Thing);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Widgets.ButtonText(r_thing[1], thing.LabelCap))
            {
                FloatMenuUtility.MakeMenu(GetThings(), (t) => t.LabelCap, (t) => () =>
                {

                    thing = t;
                    if (t.MadeFromStuff)
                        stuff = GenStuff.DefaultStuffFor(thing);
                    else
                        stuff = null;
                });
            }

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(r_stuff[0], RString.MSP_Stuff);
            Text.Anchor = TextAnchor.UpperLeft;
            if (thing.MadeFromStuff && Widgets.ButtonText(r_stuff[1], stuff.LabelCap))
            {
                FloatMenuUtility.MakeMenu(GetStuffsForThing(), (s) => s.LabelCap, (s) => () =>
                {
                    stuff = s;
                });
            }

            if (amount.max > thing.stackLimit)
                amount.max = thing.stackLimit;
            if (amount.min > amount.max)
                amount.min = amount.max;
            if (amount.min != 1 && amount.max != 1)
                equip = false;

            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(r_amount[0], RString.MSP_Amount);
            Text.Anchor = TextAnchor.UpperLeft;

            if (thing.stackLimit == 1)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(r_amount[1], thing.stackLimit.ToStringCached());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Widgets.IntRange(r_amount[1], listing.CurHeight.GetHashCode(), ref amount, 1, thing.stackLimit);
            }

            if (thingKind == ThingKind.Aparrel || thingKind == ThingKind.Weapon)
            {
                bool oldValue = equip;
                Widgets.CheckboxLabeled(r_equip, RString.MSP_Equip, ref equip);
                if (!oldValue && equip)
                    amount.min = amount.max = 1;
            }

            DoContextEditInterface(a[1]);
        }

        private IEnumerable<ThingDef> GetThings()
        {
            IEnumerable<ThingDef> things;
            switch (thingKind)
            {
                case ThingKind.Weapon:
                    things = DefDatabase<ThingDef>.AllDefs.Where(t => t.category == ThingCategory.Item && t.IsWeapon);
                    break;
                case ThingKind.Aparrel:
                    things = DefDatabase<ThingDef>.AllDefs.Where(t => t.category == ThingCategory.Item && t.IsApparel);
                    break;
                case ThingKind.Items:
                    things = DefDatabase<ThingDef>.AllDefs.Where(t => t.category == ThingCategory.Item && !(t.IsWeapon || t.IsApparel));
                    break;
                case ThingKind.Building:
                    things = DefDatabase<ThingDef>.AllDefs.Where(t => t.category == ThingCategory.Building && t.Minifiable);
                    break;
                default:
                    things = Enumerable.Empty<ThingDef>();
                    break;
            }

            things = things.OrderBy(t => t.label).ToList();

            return things;

        }

        private IEnumerable<ThingDef> GetStuffsForThing()
        {
            if (!thing.MadeFromStuff)
                return Enumerable.Empty<ThingDef>();
            return GenStuff.AllowedStuffsFor(thing).OrderBy(t => t.label);
        }

        public override void Randomize()
        {
            base.Randomize();
            this.thingKind = Utilities.GetEnumValues<ThingKind>().RandomElement();
            this.thing = GetThings().RandomElement();
            this.stuff = GetStuffsForThing().RandomElement();
            this.amount = new IntRange(Rand.RangeInclusive(1, 100), Rand.RangeInclusive(1, 100));
            this.equip = Rand.Bool;

            if (amount.max > thing.stackLimit)
                amount.max = thing.stackLimit;
            if (amount.min > amount.max)
                amount.min = amount.max;
        }

        public override bool CanCoexistWith(ScenPart other)
        {
            return true;
        }

        protected override void ModifyGeneratedPawn(Pawn pawn, bool redressed, bool humanLike)
        {
            if (!humanLike)
                return;

            var t = ThingMaker.MakeThing(thing, stuff);
            if (equip && thingKind == ThingKind.Aparrel && pawn.apparel != null)
            {
                pawn.apparel.Wear((Apparel)t, false);

            }
            else if (equip && thingKind == ThingKind.Weapon && pawn.equipment != null)
            {
                pawn.equipment.AddEquipment((ThingWithComps)t);
            }
            else if (pawn.inventory != null)
            {
                t.stackCount = amount.RandomInRange;
                pawn.inventory.TryAddItemNotForSale(t);
            }
        }
    }
}