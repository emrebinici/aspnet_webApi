using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FlowDMApi.Core.Extentions
{
    public static class EnumExtensions
    {
        public static T StringToEnum<T>(string enumString) where T : struct
        {
            return string.IsNullOrEmpty(enumString) ? default(T) : (T)Enum.Parse(typeof(T), enumString, true);
        }

        public static int[] ToIntArray<T>(T[] value)
        {
            int[] result = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
                result[i] = Convert.ToInt32(value[i]);
            return result;
        }

        public static T[] FromIntArray<T>(int[] value)
        {
            T[] result = new T[value.Length];
            for (int i = 0; i < value.Length; i++)
                result[i] = (T)Enum.ToObject(typeof(T), value[i]);
            return result;
        }


        internal static T Parse<T>(string value, T defaultValue)
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)Enum.Parse(typeof(T), value);

            int num;
            if (int.TryParse(value, out num))
            {
                if (Enum.IsDefined(typeof(T), num))
                    return (T)Enum.ToObject(typeof(T), num);
            }

            return defaultValue;
        }

        public static IEnumerable<Enum> GetFlags(this System.Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<System.Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(value);
            var results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }

        public static T GetEnumFromString<T>(string value)
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            else
            {
                string[] enumNames = Enum.GetNames(typeof(T));
                foreach (string enumName in enumNames)
                {
                    object e = Enum.Parse(typeof(T), enumName);
                    if (value == GetDescription((Enum)e))
                    {
                        return (T)e;
                    }
                }
            }
            throw new ArgumentException("The value '" + value
                + "' does not match a valid enum name or description.");
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static List<KeyValuePair<string, Enum>> GetEnumList(Type enumType)
        {
            var types = Enum.GetValues(enumType);
            var values = (from object type in types
                let fi = enumType.GetField(type.ToString())
                let attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault()
                let title = attribute == null ? type.ToString() : ((DescriptionAttribute)attribute).Description
                select new KeyValuePair<string, Enum>(title, (Enum)type)).ToList();
            return values;
        }

    }
}