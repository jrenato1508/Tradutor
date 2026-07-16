using GameLocalizationToolkit.Core.Models;

namespace GameLocalizationToolkit.Core.Interfaces;

public interface ILocalizationWriter
{
    void WriteDirectory(LocalizationScanResult result, string outputDirectoryPath);
}