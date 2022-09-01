using System;

namespace FlowDMApi.Core.Extentions
{
    public static class NullControl
    {
        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }

        public static bool IsNull<T>(this T obj, Action action)
        {
            bool ret = obj == null;

            if (ret) action();

            return ret;
        }

        public static bool IsNotNull<T>(this T obj)
        {
            return obj != null;
        }

        public static bool IfNotNull<T>(this T obj, Action action)
        {
            bool ret = obj != null;
            if (ret) action();
            return ret;
        }
    
    }
}
