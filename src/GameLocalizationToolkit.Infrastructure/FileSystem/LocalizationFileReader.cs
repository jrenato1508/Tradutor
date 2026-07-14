using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLocalizationToolkit.Core.Services
{
    public sealed partial class LocalizationFileReader : ILocalizationFileReader
    {
        public LocalizationScanResult ReadDirectory(string directoryPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException(
                    $"A pasta informada não foi encontrada: {directoryPath}");
            }

            var result = new LocalizationScanResult();

            var files = Directory.EnumerateFiles(
                directoryPath,
                "*.yml",
                SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                try
                {
                    var localizationFile = ReadFile(filePath);
                    result.Files.Add(localizationFile);
                }
                catch (Exception exception)
                {
                    result.Errors.Add(
                        $"Erro ao ler '{filePath}': {exception.Message}");
                }
            }

            return result;
        }

        public LocalizationFile ReadFile(string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "O arquivo de localização não foi encontrado.",
                    filePath);
            }

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            var language = IdentifyLanguage(lines);

            var localizationFile = new LocalizationFile
            {
                FilePath = filePath,
                Language = language
            };

            for (var index = 0; index < lines.Length; index++)
            {
                var line = lines[index];

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

        private static LocalizationEntry? ParseLine( string line, string filePath, int lineNumber)
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
