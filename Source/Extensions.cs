using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using More_Scenario_Parts.ScenParts;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts
{
    internal static class Extensions
    {
        public delegate void SetHandler<T, U>(ref T target, U value);

        private static Dictionary<string, object> _setters = new Dictionary<string, object>();

        public static SetHandler<T, U> CreateSetterForStructField<T, U>(string fieldName)
            where T : struct
        {
            var key = $"<{typeof(T).FullName}>_{typeof(U).Name}_f:{fieldName}";
            if (_setters.TryGetValue(key, out var handler))
            {
                return (SetHandler<T, U>)handler;
            }

            var field = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                DynamicMethod dm = new DynamicMethod(key, null, new[] { typeof(T).MakeByRefType(), typeof(U) }, typeof(T));
                var gen = dm.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
                gen.Emit(OpCodes.Ret);

                var del = (SetHandler<T, U>)dm.CreateDelegate(typeof(SetHandler<T, U>));
                _setters[key] = del;
                return del;
            }

            throw new Exception($"Field '{fieldName}' not found on Struct '{typeof(U)}'");
        }

        public static SetHandler<T, U> CreateSetterForStructProperty<T, U>(string propertyName)
            where T : struct
        {
            var key = $"<{typeof(T).FullName}>_{typeof(U).Name}_p:{propertyName}";
            if (_setters.TryGetValue(key, out var handler))
            {
                return (SetHandler<T, U>)handler;
            }

            var prop = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var setter = prop.GetSetMethod(true);
            if (setter != null)
            {
                DynamicMethod dm = new DynamicMethod(key, null, new[] { typeof(T).MakeByRefType(), typeof(U) }, typeof(T));
                var gen = dm.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Call, setter);
                gen.Emit(OpCodes.Ret);

                var del = (SetHandler<T, U>)dm.CreateDelegate(typeof(SetHandler<T, U>));
                _setters[key] = del;
                return del;
            }

            var fieldName = $"<{prop.Name}>k__BackingField";
            return CreateSetterForStructField<T, U>(fieldName);
        }

        public static IEnumerable<T> GetEnumValues<T>() where T : struct, Enum
        {
            foreach (var v in Enum.GetValues(typeof(T)))
            {
                yield return (T)v;
            }
        }

        public static bool Includes(this PawnModifierGender g, Gender other)
        {
            return (g == PawnModifierGender.All || other == Gender.None)
                || (g == PawnModifierGender.Female && other == Gender.Female)
                || (g == PawnModifierGender.Male && other == Gender.Male);
        }

        public static bool Includes(this IntRange range, int val)
        {
            return val >= range.min && val <= range.max;
        }

        public static Rect[] SplitCols(this Rect r, params float[] weights)
        {
            var totalWidth = weights.Sum();
            Rect[] rects = new Rect[weights.Length];
            float xOffset = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                float f = weights[i] / totalWidth;
                rects[i] = new Rect(r.x + xOffset, r.y, r.width * f, r.height);
                xOffset += rects[i].width;
            }
            return rects;
        }

        public static Rect[] SplitRows(this Rect r, params float[] weights)
        {
            var totalWeight = weights.Sum();
            Rect[] rects = new Rect[weights.Length];
            float yOffset = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                float f = weights[i] / totalWeight;
                rects[i] = new Rect(r.x, r.y + yOffset, r.width, r.height * f);
                yOffset += rects[i].height;
            }
            return rects;
        }

        public static string Translate(this ThingKind thing)
        {
            switch (thing)
            {
                case ThingKind.Weapon:
                    return R.String.MSP_ThingKind_Weapon.CapitalizeFirst();

                case ThingKind.Aparrel:
                    return R.String.MSP_ThingKind_Aparrel.CapitalizeFirst();

                case ThingKind.Building:
                    return R.String.MSP_ThingKind_Building.CapitalizeFirst();

                case ThingKind.Items:
                    return R.String.MSP_ThingKind_Items.CapitalizeFirst();

                default:
                    return R.String.MSP_Undefined.CapitalizeFirst();
            }
        }

        public static string Translate(this PawnModifierContext c)
        {
            switch (c)
            {
                case PawnModifierContext.All:
                    return R.String.MSP_PawnContext_All.CapitalizeFirst();

                case PawnModifierContext.PlayerStarter:
                    return R.String.MSP_PawnContext_PlayerStarter.CapitalizeFirst();

                case PawnModifierContext.NonPlayer:
                    return R.String.MSP_PawnContext_NonPlayer.CapitalizeFirst();

                case PawnModifierContext.Faction:
                    return R.String.MSP_PawnContext_Faction.CapitalizeFirst();

                case PawnModifierContext.Player:
                    return R.String.MSP_PawnContext_Player.CapitalizeFirst();

                case PawnModifierContext.PlayerNonStarter:
                    return R.String.MSP_PawnContext_PlayerNonStarter.CapitalizeFirst();

                default:
                    return R.String.MSP_Undefined.CapitalizeFirst();
            }
        }

        public static string Translate(this PawnModifierGender c)
        {
            switch (c)
            {
                case PawnModifierGender.All:
                    return R.String.MSP_Gender_All.CapitalizeFirst();

                case PawnModifierGender.Male:
                    return R.String.MSP_Gender_Male.CapitalizeFirst();

                case PawnModifierGender.Female:
                    return R.String.MSP_Gender_Female.CapitalizeFirst();

                default:
                    return R.String.MSP_Undefined.CapitalizeFirst();
            }
        }

        public static TStruct WithProperty<TStruct, TValue>(this TStruct @struct, string propertyName, TValue value)
            where TStruct : struct
        {
            var setter = CreateSetterForStructProperty<TStruct, TValue>(propertyName);
            setter(ref @struct, value);
            return @struct;
        }
    }
}
