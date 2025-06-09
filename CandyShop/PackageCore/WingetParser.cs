using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Unicode;

namespace CandyShop.PackageCore
{
    public static class WingetParser
    {
        private static readonly List<char> PROGRESS_CHARS = ['-', '\\', '|', '/'];

        /// <summary>
        /// Trims all chars before the last carriage return '\r' of the first line.
        /// In strict mode, an exception is thrown if the trimmed part contains
        /// anything other than winget progress indicators.
        /// </summary>
        /// <exception cref="PackageManagerException"></exception>
        public static string LeftTrimProgressChars(string value, bool strict = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // find first line (without trailing new line characters)
            int end = value.IndexOf(Environment.NewLine);
            end = end < 0 ? value.Length : end;
            string firstLine = value[..end];

            // perform trim
            int i = firstLine.LastIndexOf('\r');
            string discard;
            string remainder;
            if (i < 0)
            {
                discard = "";
                remainder = value;
            }
            else
            {
                discard = value[..i++];
                remainder = value[i..];
            }

            // validate
            string[] discardedElements = discard.Split('\r');
            bool discardedOnlyWingetProgressElements = discardedElements
                .Select(row => row.Trim())
                .Where(row => !string.IsNullOrEmpty(row))
                .Select(row => row[0])
                .All(c => PROGRESS_CHARS.Contains(c) || UnicodeRanges.BlockElements.Contains(c));

            // notify validation error
            if (!discardedOnlyWingetProgressElements)
            {
                if (strict)
                {
                    Log.Error($"Trimming of winget progress indicators removed too much: trimmed={discard}; remainder={remainder} of \"{value}\"");
                    throw new PackageManagerException($"Trimming of winget progress indicators removed too much.");
                }
                else
                {
                    Log.Warning($"Trimming of winget progress indicators removed too much: trimmed={discard}; remainder={remainder} of \"{value}\"");
                }
            }

            return remainder;
        }

        public static bool Contains(this UnicodeRange range, char value)
        {
            int start = UnicodeRanges.BlockElements.FirstCodePoint;
            int end = start + UnicodeRanges.BlockElements.Length;
            return value >= start && value < end;
        }
    }
}
