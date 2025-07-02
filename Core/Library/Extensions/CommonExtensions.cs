using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Library.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        ///     Compares an item to a list of items for inclusion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsOneOf<T>(this T item, params T[] items) where T : IConvertible //enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            if (items == null)
                return false;

            return items.Any(i => i.Equals(item));
        }

        /// <summary>
        ///     Negative of IsOneOf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsNotOneOf<T>(this T item, params T[] items) where T : IConvertible //enum
        {
            return !item.IsOneOf(items);
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static string SanitizeFileName(this string str, int limit = -1)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            str = r.Replace(str, "");

            if (limit > -1)
                str = new string(str.Take(limit).ToArray());
            return str;
        }

        public static string GetTestName(this string testname)
        {
            var pattern = @"\*.*\*";
            var regex = new Regex(pattern);
            if (regex.IsMatch(testname)) return regex.Match(testname).Groups[0].Value;

            return testname;
        }

        //public static TAttribute GetEnumAttribute<TAttribute>(this Enum value)
        //    where TAttribute : Attribute
        //{
        //    var enumType = value.GetType();
        //    var name = Enum.GetName(enumType, value);
        //    return enumType.GetField(name)
        //        .GetCustomAttributes(false)
        //        .OfType<TAttribute>()
        //        .SingleOrDefault();
        //}

        public static T GetEnumAttribute<T>(this Enum enumValue) where T : Attribute
        {
            T attribute;

            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString())
                .FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T) memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                return attribute;
            }

            return null;
        }

        /// <summary>
        ///     Tries to detect similar in a lenient way
        /// </summary>
        /// <param name="str"></param>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsSimilarTo(this string str, string testStr)
        {
            //simplest case
            if (str == testStr) return true;

            //either is null
            if (str == null && testStr != null ||
                testStr == null && str != null)
                return false;

            //safe to perform operations
            var cleanStr = str.Trim().ToLower();
            var cleanTestStr = testStr.Trim().ToLower();

            return cleanStr.Equals(cleanTestStr);
        }
    }
}