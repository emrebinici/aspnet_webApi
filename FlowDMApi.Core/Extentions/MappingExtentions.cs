using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlowDMApi.Core.Extentions
{
    public class Mapping
    {
        public static void To<T>(ref T target, T source)
        {
            Type t = typeof(T);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            var target1 = target;
            Parallel.ForEach(properties, prop =>
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target1, value, null);
            });

        }
        public static void DifrentTo<T, TU>(ref T target, TU source)
        {
            if (source == null)
            {
                target = default(T);
                return;
            }
            Type t = typeof(T);
            Type tu = typeof(TU);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            var propertiestu = tu.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            var target1 = target;
            Parallel.ForEach(properties, prop =>
            {
                var propertyInfos = propertiestu as PropertyInfo[] ?? propertiestu.ToArray();
                if (propertyInfos.FirstOrDefault(x => x.Name == prop.Name) != null)
                {
                    var value = propertyInfos.FirstOrDefault(x => x.Name == prop.Name)?.GetValue(source, null);
                    if (value != null)
                        prop.SetValue(target1, value, null);
                };

            });
        }
        public static void SameListTo<T>(ref List<T> source, string propname, object propvalue)
        {
            if (source == null) return;
            Parallel.ForEach(source, item =>
            {
                var properties = item.GetType().GetProperties().FirstOrDefault(prop => prop.Name.Trim() == propname);
                if (properties == null)
                {
                    throw new Exception("Parametre Adını Düzgün Yazınız");
                }
                properties.SetValue(item, propvalue, null);

            });
        }
    }
}
