using GameLocalizationToolkit.Core.Enums;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationTranslationPlanner
    {
       IEnumerable<LocalizationEntry> GetEntriesToTranslate(TranslationMode mode,LocalizationScanResult source,LocalizationDirectoryComparisonResult? comparisonResult = null);
    }
}
