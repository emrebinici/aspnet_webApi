namespace FlowDMApi.Core.Extentions
{
    public static class isNumericExtension
    {
        public static bool isNumeric(this string value)
        {
            double oReturn = 0; return double.TryParse(value, out oReturn);

        }
    }
}
