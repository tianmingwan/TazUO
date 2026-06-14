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
        private static readonly Dictionary<char, char> _traditionalToSimplified = new();

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
                        _traditionalToSimplified[traditional[0]] = values[0][0];
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
                sb.Append(_traditionalToSimplified.TryGetValue(c, out char simplified) ? simplified : c);
            }
            return sb.ToString();
        }
    }
}