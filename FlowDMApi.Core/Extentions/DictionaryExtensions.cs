using System.Collections;

namespace FlowDMApi.Core.Extentions
{
    public static class DictionaryExtensions
    {
        public static object Get(this IDictionary dictionary, object key, object defaultValue = null)
        {
            return dictionary.Contains(key) ? dictionary[key] : defaultValue;
        }

        public static T Get<T>(this IDictionary dictionary, object key, T defaultValue = default(T))
        {
            return (T)(dictionary.Contains(key) ? dictionary[key] : defaultValue);
        }

        public static void Set<T>(this IDictionary dictionary, object key, T value = default(T))
        {
            if (dictionary.Contains(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void Set(this IDictionary dictionary, object key, object value = null)
        {
            if (dictionary.Contains(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }

}