namespace FlowDMApi.Core.Extentions
{
    public static class ObjectExtensions
    {
        public static long ParseLong(object obj)
        {
            var defaultValue = default(long);
            if (obj != null && long.TryParse(obj.ToString(), out defaultValue))
            {
                
            }
            return defaultValue;
        }

        public static double ParseDouble(object obj)
        {
            var defaultValue = default(double);
            if (obj != null && double.TryParse(obj.ToString(), out defaultValue))
            {
                
            }
            return defaultValue;
        }

        public static int ParseInt(object obj)
        {
            var defaultValue = default(int);
            if (obj != null && int.TryParse(obj.ToString(), out defaultValue))
            {

            }
            return defaultValue;
        }

        public static bool IsInt(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }
    }
}