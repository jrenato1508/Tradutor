using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;

namespace GameLocalizationToolkit.Core.Services;

public sealed class LocalizationMerger : ILocalizationMerger
{
    public LocalizationFile Merge( LocalizationFile source,LocalizationFile target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        var mergedFile = new LocalizationFile
        {
            FilePath = target.FilePath,
            RelativePath = target.RelativePath,
            Language = target.Language
        };

        var targetEntries = target.Entries
            .GroupBy(entry => entry.Key)
            .ToDictionary(
                group => group.Key,
                group => group.First());

        foreach (var sourceEntry in source.Entries)
        {
            if (targetEntries.TryGetValue(
                sourceEntry.Key,
                out var targetEntry))
            {
                mergedFile.Entries.Add(targetEntry);
            }
            else
            {
                mergedFile.Entries.Add(sourceEntry);
            }
        }

        return mergedFile;
    }
}