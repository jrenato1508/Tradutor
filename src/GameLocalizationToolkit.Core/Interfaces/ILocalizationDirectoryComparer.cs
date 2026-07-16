using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationDirectoryComparer
    {
        LocalizationDirectoryComparisonResult Compare(LocalizationScanResult source, LocalizationScanResult target);
    }
}
