using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Guppy
{
    public static class StringExtensions
    {
        /// <summary>
        /// Answers true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue (this string s)
        {
            return !string.IsNullOrWhiteSpace (s);
        }

        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrEmpty (this string s)
        {
            return string.IsNullOrEmpty (s);
        }

        /// <summary>
        /// Answers true if this String is either null or empty.
        /// </summary>
        /// <remarks>I'm so tired of typing String.IsNullOrEmpty(s)</remarks>
        public static bool IsNullOrWhitespace (this string s)
        {
            return string.IsNullOrWhiteSpace (s);
        }

        public static bool IsNumeric (this string text)
        {
            foreach (var c in text)
                if (!char.IsDigit (c))
                    return false;

            return true;
        }

        // Converts nulls to string.empty
        public static string OrEmpty (this string str)
        {
            return str ?? string.Empty;
        }

        // Truncates a string to a max length
        public static string Truncate (this string str, int length)
        {
            if (string.IsNullOrWhiteSpace (str))
                return str;

            if (str.Length <= length)
                return str;

            return str.Remove (length);
        }

        public static string TruncateWithEllipsis (this string s, int maxLength)
        {
            if (s.IsNullOrEmpty ()) return s;
            if (s.Length <= maxLength) return s;

            return string.Format ("{0}...", Truncate (s, maxLength - 3));
        }

        // Tries to format a string as Yes/No
        public static string ToYesNo (this string str)
        {
            if (string.IsNullOrWhiteSpace (str))
                return str;

            switch (str.ToLowerInvariant ()) {
                case "y":
                case "yes":
                    return "Yes";
                case "n":
                case "no":
                    return "No";
            }

            return str.OrEmpty ();
        }

        // Grabs a single value out of a string using a Regex capture
        // Ex: var value = "Test [String]".QuickCapture ("\[(?<value>.+)\]");
        public static string QuickCapture (this string s, string regex, string groupName = "value")
        {
            var r = Regex.Match (s, regex);

            if (r.Success && r.Groups.Count > 0)
                return r.Groups[groupName].Value;

            return null;
        }

        // Grabs a single value out of a string using a Regex capture
        // Ex: var value = "Test [String]".QuickCapture ("\[(?<value>.+)\]");
        public static List<string> QuickCaptures (this string s, string regex, string groupName = "value")
        {
            var r = Regex.Match (s, regex);
            var results = new List<string> ();

            if (r.Success && r.Groups.Count > 0)
                foreach (Group g in r.Groups)
                    results.Add (g.Value.Trim ());

            return results;
        }

        public static string ToBase64 (this string str)
        {
            var bytes = UTF8Encoding.UTF8.GetBytes (str);
            return Convert.ToBase64String (bytes);
        }

        public static bool IsAnyNullOrWhitespace (params string[] strings)
        {
            return strings.Any (p => string.IsNullOrWhiteSpace (p));
        }
    }
}
