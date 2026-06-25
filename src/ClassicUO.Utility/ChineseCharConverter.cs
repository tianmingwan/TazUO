using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ClassicUO.Utility
{
    using ClassicUO.Utility.Logging;

    public static class ChineseCharConverter
    {
        // Maps a traditional char to its full simplified form. Most entries are a single
        // char, but CJK Extension B / rare characters are encoded as a UTF-16 surrogate
        // PAIR (two char code units). Storing the whole string — not just values[0][0] —
        // is critical: taking only the first code unit splits the pair and produces a
        // lone high surrogate, which crashes FontStashSharp's ConvertToUtf32 later.
        private static readonly Dictionary<char, string> _traditionalToSimplified = new();

        static ChineseCharConverter()
        {
            try
            {
                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClassicUO.Utility.Resources.TSCharacters.txt");
                if (stream == null)
                {
                    Log.Error("ChineseCharConverter: embedded resource TSCharacters.txt not found");
                    return;
                }

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                        continue;

                    string[] parts = line.Split('\t');
                    if (parts.Length < 2)
                        continue;

                    string traditional = parts[0];
                    string[] values = parts[1].Split(' ');
                    if (traditional.Length > 0 && values.Length > 0 && values[0].Length > 0)
                    {
                        // Keep the entire simplified value (values[0]). For surrogate-pair
                        // targets this is 2 chars; for BMP targets it is 1 char.
                        _traditionalToSimplified[traditional[0]] = values[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ChineseCharConverter: failed to load dictionary - {ex.Message}");
            }
        }

        public static string TraditionalToSimplified(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (_traditionalToSimplified.TryGetValue(c, out string simplified))
                    sb.Append(simplified);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }

}