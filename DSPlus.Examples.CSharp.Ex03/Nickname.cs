using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DSPlus.Examples
{
    class Nickname
    {
        public static string FixNickname(string source, string discriminator)
        {
            var acceptablePartOfNickname = FindAcceptablePathOfNickname(source);
            return acceptablePartOfNickname != "" ? acceptablePartOfNickname : $"D.{discriminator}";
        }

        public static string FindAcceptablePathOfNickname(string source)
        {
            Regex regex = new Regex(@"([a-zA-Zа-яА-ЯёЁ'].*[a-zA-Zа-яА-ЯёЁ\d\)\]-])");
            MatchCollection matches = regex.Matches(source);
            if (matches.Count > 0)
            {
                return RemoveDiacritics(matches[0].Value);
            }

            return "";
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (c == 'ё' || c == 'й')
                {
                    stringBuilder.Append(c);
                }

                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);


            char[] acceptedSymbols = { 'й', 'ё' };

            StringBuilder sb = new StringBuilder(result);


            foreach (var symbol in acceptedSymbols)
            {
                int index = text.IndexOf(symbol);
                if (index != -1)
                    sb[index] = symbol;
            }

            return sb.ToString();
        }
    }
}
