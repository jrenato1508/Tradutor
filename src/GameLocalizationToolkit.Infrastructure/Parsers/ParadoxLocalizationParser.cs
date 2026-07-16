using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLocalizationToolkit.Infrastructure.Parsers
{
    public sealed partial class ParadoxLocalizationParser : ILocalizationParser
    {
        public LocalizationFile Parse(string filePath,string relativePath,string[] lines)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
            ArgumentNullException.ThrowIfNull(lines);

            var fileLines = lines.ToArray();

            var language = IdentifyLanguage(fileLines);

            var localizationFile = new LocalizationFile
            {
                FilePath = filePath,
                RelativePath = relativePath,
                Language = language
            };

            for (var index = 0; index < fileLines.Length; index++)
            {
                var line = fileLines[index];

                if (ShouldIgnoreLine(line))
                {
                    continue;
                }

                var entry = ParseLine(
                    line,
                    filePath,
                    index + 1);

                if (entry is not null)
                {
                    localizationFile.Entries.Add(entry);
                }
            }

            return localizationFile;
        }

        private static string IdentifyLanguage(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim()
                    .TrimStart('\uFEFF');

                if (trimmedLine.StartsWith("l_", StringComparison.OrdinalIgnoreCase) &&
                    trimmedLine.EndsWith(':'))
                {
                    return trimmedLine.TrimEnd(':');
                }
            }

            return "unknown";
        }

        private static bool ShouldIgnoreLine(string line)
        {
            var trimmedLine = line.Trim();

            return string.IsNullOrWhiteSpace(trimmedLine) ||
                   trimmedLine.StartsWith('#') ||
                   trimmedLine.StartsWith("l_");
        }

        private static LocalizationEntry? ParseLine(string line, string filePath, int lineNumber)
        {
            var match = LocalizationLineRegex().Match(line);

            if (!match.Success)
            {
                return null;
            }

            return new LocalizationEntry
            {
                Key = match.Groups["key"].Value,
                Version = match.Groups["version"].Value,
                Value = match.Groups["value"].Value,
                SourceFile = filePath,
                LineNumber = lineNumber
            };
        }

        [GeneratedRegex("""^\s*(?<key>[^\s:#]+):(?<version>\d*)\s+"(?<value>.*)"\s*$""", RegexOptions.Compiled)]
        private static partial Regex LocalizationLineRegex();

    }
}
