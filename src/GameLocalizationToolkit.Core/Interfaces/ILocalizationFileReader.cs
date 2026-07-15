using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ILocalizationFileReader
    {
        LocalizationScanResult ReadDirectory(string directoryPath);

        LocalizationFile ReadFile(string filePath);
    }
}
