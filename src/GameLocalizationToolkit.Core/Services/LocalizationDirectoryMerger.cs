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
                result.Files.Add(sourceFile);
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
}