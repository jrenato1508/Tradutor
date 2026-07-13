using App.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interfaces
{
    public interface ILocalizationFileReader
    {
        LocalizationScanResult ReadDirectory(string directoryPath);

        LocalizationFile ReadFile(string filePath);
    }
}
