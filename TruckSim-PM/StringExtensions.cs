using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckSim_PM
{
    static class StringExtensions
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            ArgumentNullException.ThrowIfNull(s);
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            for (int i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
            }
        }

        public static string DirectoryToScsUsername(this string directorystring)
        {
            DirectoryInfo di = new(directorystring);
            IEnumerable<string> parts = di.Name.SplitInParts(2);
            string hexstring = String.Join(" ", parts);
            string[] hexValuesSplit = hexstring.Split(' ');
            string username = string.Empty;
            foreach (string hex in hexValuesSplit)
            {
                int value = Convert.ToInt32(hex, 16);
                string stringValue = Char.ConvertFromUtf32(value);
                char charValue = (char)value;
                username += charValue;
            }
            return username;
        }

        public static string ScsUsernameToDirectory(this string username)
        {
            char[] values = username.ToCharArray();
            string ScSDirectoryname = string.Empty;
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hex = $"{value:X}";
                ScSDirectoryname += hex;
            }
            return ScSDirectoryname;
        }
    }
}
