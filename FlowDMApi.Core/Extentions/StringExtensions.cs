using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlowDMApi.Core.Extentions
{
    public static class StringExtensions
    {
        public static bool Match(this string str, string pattern)
        {
            return new Regex(pattern).IsMatch(str);
        }

        /// <summary>
        /// #1#3#5#2#  şekindeki ifadeleri List<int> tipine çevirir.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<int> SeparateToList(this string input, char separator)
        {
            var list = new List<int>();

            if (String.IsNullOrEmpty(input))
                return list;

            input = input.Trim();
            // İlk karakter ayraçsa onu kaldır
            if (input.Length > 0 && input[0] == separator)
            {
                input = input.Remove(0, 1);
            }
            // Son karakter ayraçsa onu kaldır
            if (input.Length > 0 && input[input.Length - 1] == separator)
            {
                input = input.Remove(input.Length - 1, 1);
            }

            var tokens = input.Split(separator);

            foreach (var token in tokens)
            {
                list.Add(Convert.ToInt32(token.Trim()));
            }
            return list;
        }

        public static List<T> SeparateToList<T>(this string input, char separator)
        {
            return new List<T>(input.SeparateToList(separator).Cast<T>());
        }

        public static bool Match(this string str, params string[] patterns)
        {
            return str.Match(Operator.And, patterns);
        }
        public static List<string> EnumToList<T>(this List<T> input)
        {
            return input.Select(arg => Convert.ToString(arg)).ToList();
        }
        public static T[] ListToEnums<T>(this List<string> item)
        {
            return item.Select(x => Enum.Parse(typeof(T), x.ToString().ToUpper())).Cast<T>().ToArray();
        }
        public static T StringToEnums<T>(this string item)
        {
           return (T) Enum.Parse(typeof(T), item, true);
        }
        public static bool Match(this string str, Operator oOperator, params string[] patterns)
        {
            var returnValue = false;
            foreach (var pattern in patterns)
            {
                var result = Regex.IsMatch(str, pattern);

                switch (oOperator)
                {
                    case Operator.And:
                        if (!result)
                        {
                            return false;
                        }
                        else
                        {
                            returnValue = true;
                        }
                        break;
                    case Operator.Or:
                        if (result)
                        {
                            return true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("oOperator");
                }
            }
            return returnValue;
        }


        public static string GetFullFnString(string fn, params string[] parameters)
        {
            var func = "";
            var match = Regex.Match(fn, @"^(?<fn>.[^(]+)\((?<params>.[^)]+)?\)$");
            if (match.Success)
            {
                var liste = parameters.ToList();
                var prevParams = match.Groups["params"].Value.Split(',');
                for (var i = 0; i < prevParams.Length; i++)
                {
                    liste.Insert(i, prevParams[i]);
                }
                func = match.Groups["fn"].Value + "(" + string.Join(",", liste) + ")";
            }
            else
            {
                if (parameters == null || parameters.Length == 0)
                {
                    func = fn + (fn.Contains("(") ? "" : "()");
                }
                else
                {
                    func = fn + "(" + string.Join(",", parameters) + ")";
                }
            }
            return func;
        }

        public static string ToEnglishString(this string str)
        {
            //str = str.Replace(" ", "_");
            str = str.Replace("Ş", "S");
            str = str.Replace("ş", "s");
            str = str.Replace("Ç", "C");
            str = str.Replace("ç", "c");
            str = str.Replace("Ö", "O");
            str = str.Replace("ö", "o");
            str = str.Replace("Ü", "U");
            str = str.Replace("ü", "u");
            str = str.Replace("Ğ", "G");
            str = str.Replace("İ", "I");
            str = str.Replace("ı", "i");
            str = str.Replace("ğ", "g");
            return str;
        }

        public static bool Contains(this string input, string findMe, StringComparison comparisonType)
        {
            return !string.IsNullOrWhiteSpace(input) && input.IndexOf(findMe, comparisonType) > -1;
        }

        public static string ToStringNullSafe(this object value)
        {
            return (value ?? string.Empty).ToString();
        }

        public static string WinsoftSubstr(string text, int maxKarekter = 31, int minKarakter = 0)
        {
            if (string.IsNullOrEmpty(text) == false && text.Length > maxKarekter)
            {
                text = text.Substring(minKarakter, maxKarekter);
            }
            else if (string.IsNullOrEmpty(text) == false && text.Length > minKarakter && minKarakter > 0)
            {
                text = text.Remove(0, minKarakter);
            }

            return text;
        }

        public static string Sansurle(string text)
        {
            var isim = "";
            for (int j = 0; j < text.Split(' ').Length; j++)
            {
                for (var i = 0; i < text.Split(' ')[j].Length; i++)
                {
                    if (i > 1 && text.Split(' ')[j][i] != ' ')
                    {
                        isim = isim + '*';
                    }
                    else
                    {
                        isim = isim + text.Split(' ')[j][i];
                    }
                }
                isim = isim + " ";
            }
            return isim;
        }

        public static string Reverse(string str)
        {
            string result = string.Empty;
            char[] charsOfStr = str.ToCharArray();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                result += charsOfStr[i];
            }
            return result;
        }

    }

    public enum Operator
    {
        And,
        Or
    }

    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
       
      
  


