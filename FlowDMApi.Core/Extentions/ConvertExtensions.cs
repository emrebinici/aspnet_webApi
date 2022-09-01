using System;

namespace FlowDMApi.Core.Extentions
{
    public static class ConvertExtensions
    {
        public static long? ParseLong(object value)
        {
            if (value != null) return long.Parse(value.ToString());            
            return null;
        }

        public static DateTime? ParseDateTime(object value, string format = "")
        {
            if (value != null) return DateTime.Parse(value.ToString());            
            return null;
        }

        public static string ParseString(object value)
        {
            if (value != null) return value.ToString();            
            return null;
        }

        public static T ParseEnum<T>(object value) where T:struct 
        {
            if (value != null) return (T)Enum.Parse(typeof(T), value.ToString());            
            return default(T);
        }

        public static int? ParseInt(object value)
        {
            if (value != null) return int.Parse(value.ToString());
            return null;
        }

        public static long ToLong(this ulong ulongValue)
        {
            return unchecked((long)ulongValue + long.MinValue);
        }

        public static ulong ToUlong(this long longValue)
        {
            return unchecked((ulong)(longValue - long.MinValue));
        }

        public static T To<T>(this IConvertible obj)
        {
            Type t = typeof(T);

            if (t.IsGenericType
                && (t.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null)
                {
                    return (T)(object)null;
                }
                else
                {
                    return (T)Convert.ChangeType(obj, Nullable.GetUnderlyingType(t));
                }
            }
            else
            {
                return (T)Convert.ChangeType(obj, t);
            }
        }

        public static object To(this IConvertible obj, Type type)
        {
            if (type.IsGenericType
                && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null)
                {
                    return (object)null;
                }
                else
                {
                    return Convert.ChangeType(obj, Nullable.GetUnderlyingType(type));
                }
            }
            else
            {
                return Convert.ChangeType(obj, type);
            }
        }
    }
}