using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System.Text;

namespace GameLocalizationToolkit.Infrastructure.FileSystem;

public sealed class LocalizationFileReader : ILocalizationFileReader
{
    private readonly ILocalizationParser _parser;

    public LocalizationFileReader(ILocalizationParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        _parser = parser;
    }

    public LocalizationScanResult ReadDirectory(string directoryPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException(
                $"A pasta informada não foi encontrada: {directoryPath}");
        }

        var result = new LocalizationScanResult();

        var files = Directory.EnumerateFiles(
            directoryPath,
            "*.yml",
            SearchOption.AllDirectories);

        foreach (var filePath in files)
        {
            try
            {
                var relativePath = Path.GetRelativePath(directoryPath,filePath);

                var localizationFile = ReadFile(filePath,relativePath);

                result.Files.Add(localizationFile);
            }
            catch (Exception exception)
            {
                result.Errors.Add(
                    $"Erro ao ler '{filePath}': {exception.Message}");
            }
        }

        return result;
    }

    public LocalizationFile ReadFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var relativePath = Path.GetFileName(filePath);

        return ReadFile(filePath, relativePath);
    }

    private LocalizationFile ReadFile( string filePath,string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("O arquivo de localização não foi encontrado.",filePath);
        }

        var lines = File.ReadAllLines(filePath,Encoding.UTF8);

        return _parser.Parse(filePath,relativePath,lines);
    }
}