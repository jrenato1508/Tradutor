using GameLocalizationToolkit.Core.Enums;
using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;

namespace GameLocalizationToolkit.Core.Services;

public sealed class LocalizationTranslationPlanner : ILocalizationTranslationPlanner
{
    public IEnumerable<LocalizationEntry> GetEntriesToTranslate( TranslationMode mode, LocalizationScanResult source, LocalizationDirectoryComparisonResult? comparisonResult = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        return mode switch
        {
            TranslationMode.UpdateExistingTranslation =>
                GetEntriesForExistingTranslation(comparisonResult),

            TranslationMode.CreateNewTranslation =>
                GetAllEntries(source),

            _ => throw new ArgumentOutOfRangeException(
                nameof(mode),
                mode,
                "Modo de tradução não suportado.")
        };
    }

    private static IEnumerable<LocalizationEntry>
        GetEntriesForExistingTranslation(
            LocalizationDirectoryComparisonResult? comparisonResult)
    {
        ArgumentNullException.ThrowIfNull(comparisonResult);

        return comparisonResult.AddedEntries;
    }

    private static IEnumerable<LocalizationEntry>
        GetAllEntries(LocalizationScanResult source)
    {
        return source.Files
            .SelectMany(file => file.Entries);
    }
}