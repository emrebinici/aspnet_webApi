using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowDMApi.Core.Extentions
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static Y SelectFirstIfExists<T, Y>(this IEnumerable<T> source, Func<T, Y> selector,
            Y defaultValue = default(Y))
        {
            return
                source != null &&
                source.Any()
                    ? source.Select(selector).FirstOrDefault()
                    : defaultValue;
        }

        public static Y SelectIfExist<T, Y>(this T source, Func<T, Y> selector, Y defaultValue = default(Y))
        {
            return source != null ? selector(source) : defaultValue;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    IEqualityComparer<TKey> comparer)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        // string parametre'ye göre index bulan metot - eğer değere uygun kayıt yoksa -1 döner
        public static int IndexControl<T>(this IEnumerable<T> veriler, string parametre)
        {
            var sonuc = -1;
            Parallel.ForEach(veriler, (i, state) =>
            {
                if (i.ToString() != parametre) return;
                sonuc = 1;
                state.Stop();

            });
          
            return sonuc;
        }

        // Lambda Expressions ile sorgulama yapılabilen ve sonuçta veriler elde edilen metot
        public static IEnumerable<T> WhereSearch<T>(this IEnumerable<T> veriler, Func<T, bool> expressions)
        {
            List<T> sonuc = new List<T>();
            foreach (T veri in veriler)
            {
                if (expressions(veri))
                {
                    sonuc.Add(veri);
                }
            }
            return sonuc;
        }

    }
}