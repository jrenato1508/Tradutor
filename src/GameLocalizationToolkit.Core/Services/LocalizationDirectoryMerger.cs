using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;

namespace GameLocalizationToolkit.Core.Services;

public sealed class LocalizationDirectoryMerger : ILocalizationDirectoryMerger
{
    private readonly ILocalizationMerger _merger;

    public LocalizationDirectoryMerger(
        ILocalizationMerger merger)
    {
        ArgumentNullException.ThrowIfNull(merger);

        _merger = merger;
    }

    public LocalizationScanResult Merge(LocalizationScanResult source, LocalizationScanResult target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var result = new LocalizationScanResult();

        var targetFiles = target.Files
            .GroupBy(GetLogicalFileName)
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.OrdinalIgnoreCase);

        var processedFiles = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var sourceFile in source.Files)
        {
            var logicalFileName = GetLogicalFileName(sourceFile);

            if (targetFiles.TryGetValue(
                logicalFileName,
                out var targetFile))
            {
                var mergedFile = _merger.Merge(
                    sourceFile,
                    targetFile);

                result.Files.Add(mergedFile);
            }
            else
            {
                // O arquivo ainda não existe no mod.
                var targetLanguage = target.Files.FirstOrDefault()?.Language ?? sourceFile.Language;
                var convertedFile = ConvertToTargetLanguage( sourceFile,targetLanguage);

                result.Files.Add(convertedFile);
            }

            processedFiles.Add(logicalFileName);
        }

        // Preserva arquivos existentes apenas no mod.
        foreach (var targetFile in target.Files)
        {
            var logicalFileName = GetLogicalFileName(targetFile);

            if (!processedFiles.Contains(logicalFileName))
            {
                result.Files.Add(targetFile);
            }
        }

        result.Errors.AddRange(source.Errors);
        result.Errors.AddRange(target.Errors);

        return result;
    }

    private static string GetLogicalFileName(LocalizationFile file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file.FilePath);

        var index = fileName.LastIndexOf("_l_",
            StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            fileName = fileName[..index];
        }

        return fileName;
    }

    private static LocalizationFile ConvertToTargetLanguage(
    LocalizationFile sourceFile,
    string targetLanguage)
    {
        var sourceLanguageName =
            sourceFile.Language.TrimStart('l', '_');

        var targetLanguageName =
            targetLanguage.TrimStart('l', '_');

        var convertedRelativePath =
            sourceFile.RelativePath.Replace(
                $"_l_{sourceLanguageName}",
                $"_l_{targetLanguageName}",
                StringComparison.OrdinalIgnoreCase);

        var convertedFilePath =
            sourceFile.FilePath.Replace(
                $"_l_{sourceLanguageName}",
                $"_l_{targetLanguageName}",
                StringComparison.OrdinalIgnoreCase);

        var convertedFile = new LocalizationFile
        {
            FilePath = convertedFilePath,
            RelativePath = convertedRelativePath,
            Language = targetLanguage
        };

        foreach (var entry in sourceFile.Entries)
        {
            convertedFile.Entries.Add(entry);
        }

        return convertedFile;
    }
}