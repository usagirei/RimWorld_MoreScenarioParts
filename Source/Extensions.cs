using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace More_Scenario_Parts
{
    public static class Utilities
    {
        public  static bool Includes(this IntRange range, int val)
        {
            return val >= range.min && val <= range.max;
        }

        public delegate void SetHandler<T, U>(ref T target, U value);

        private static Dictionary<string, object> _setters = new Dictionary<string, object>();

        public static IEnumerable<T> GetEnumValues<T>() where T : struct, Enum
        {
            foreach (var v in Enum.GetValues(typeof(T)))
                yield return (T)v;
        }

        public static TStruct WithProperty<TStruct, TValue>(this TStruct @struct, string propertyName, TValue value)
            where TStruct : struct
        {
            var setter = CreateSetterForStructProperty<TStruct, TValue>(propertyName);
            setter(ref @struct, value);
            return @struct;
        }

        public static SetHandler<T, U> CreateSetterForStructField<T, U>(string fieldName)
            where T : struct
        {
            var key = $"<{typeof(T).FullName}>_{typeof(U).Name}_f:{fieldName}";
            if (_setters.TryGetValue(key, out var handler))
                return (SetHandler<T, U>)handler;

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
                return (SetHandler<T, U>)handler;

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
    }
}