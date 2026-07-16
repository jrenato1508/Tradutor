using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System.Text;

namespace GameLocalizationToolkit.Infrastructure.FileSystem;

public sealed class LocalizationWriter : ILocalizationWriter
{
    public void WriteDirectory(LocalizationScanResult result, string outputDirectoryPath)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectoryPath);

        Directory.CreateDirectory(outputDirectoryPath);

        foreach (var file in result.Files)
        {
            WriteFile(file, outputDirectoryPath);
        }
    }

    private static void WriteFile(LocalizationFile file, string outputDirectoryPath)
    {
        ArgumentNullException.ThrowIfNull(file);

        var outputFilePath = Path.Combine(outputDirectoryPath,file.RelativePath);

        var outputFileDirectory = Path.GetDirectoryName(outputFilePath);

        if (!string.IsNullOrWhiteSpace(outputFileDirectory))
        {
            Directory.CreateDirectory(outputFileDirectory);
        }

        using var writer = new StreamWriter(outputFilePath,false,new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        writer.WriteLine($"{file.Language}:");
        writer.WriteLine();

        foreach (var entry in file.Entries)
        {
            var version = entry.Version ?? string.Empty;

            writer.WriteLine(
                $" {entry.Key}:{version} \"{entry.Value}\"");
        }
    }
}