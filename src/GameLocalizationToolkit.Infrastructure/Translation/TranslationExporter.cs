using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLocalizationToolkit.Infrastructure.Translation;

public sealed class TranslationExporter : ITranslationExporter
{
    private readonly ILocalizationTokenProtector _tokenProtector;

    public TranslationExporter(ILocalizationTokenProtector tokenProtector)
    {
        ArgumentNullException.ThrowIfNull(tokenProtector);

        _tokenProtector = tokenProtector;
    }
   

    public TranslationExportResult Export(IEnumerable<LocalizationEntry> entries, string outputDirectoryPath, int entriesPerFile = 500)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectoryPath);

        if (entriesPerFile <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(entriesPerFile), "A quantidade de entradas por arquivo deve ser maior que zero.");
        }

        var entriesList = entries
            .Where(entry =>
            {
                var protectedText =
                    _tokenProtector.Protect(entry.Value);

                return HasTranslatableContent(
                    protectedText.Text);
            })
            .ToList();

        if (entriesList.Count == 0)
        {
            return new TranslationExportResult
            {
                ExportedEntries = 0,
                GeneratedFiles = 0
            };
        }

        Directory.CreateDirectory(outputDirectoryPath);

        var totalFiles =
            (int)Math.Ceiling(
                entriesList.Count /
                (double)entriesPerFile);

        for (var fileIndex = 0;
             fileIndex < totalFiles;
             fileIndex++)
        {
            var entriesBatch = entriesList
                .Skip(fileIndex * entriesPerFile)
                .Take(entriesPerFile)
                .ToList();

            var fileName =
                $"translation_part_{fileIndex + 1:D3}.txt";

            var filePath =
                Path.Combine(
                    outputDirectoryPath,
                    fileName);

            WriteTranslationFile(
                entriesBatch,
                filePath);
        }

        return new TranslationExportResult
        {
            ExportedEntries = entriesList.Count,
            GeneratedFiles = totalFiles
        };

    }

    private void WriteTranslationFile(IEnumerable<LocalizationEntry> entries, string filePath)
    {
        using var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        foreach (var entry in entries)
        {
            var protectedText = _tokenProtector.Protect(entry.Value);
                        
            writer.WriteLine($"@@KEY={entry.Key}");

            writer.WriteLine($"ORIGINAL={protectedText.Text}");

            writer.WriteLine("TRANSLATION=");

            writer.WriteLine();
        }
    }


    private static bool HasTranslatableContent(string protectedText)
    {
        if (string.IsNullOrWhiteSpace(protectedText))
        {
            return false;
        }

        var textWithoutTokens = Regex.Replace(
            protectedText,
            @"__GLT_TOKEN_\d+__",
            string.Empty);

        return !string.IsNullOrWhiteSpace(textWithoutTokens);
    }

    
}