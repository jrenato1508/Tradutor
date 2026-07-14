using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLocalizationToolkit.Infrastructure.FileSystem
{
    public sealed partial class LocalizationFileReader : ILocalizationFileReader
    {
        private readonly ILocalizationParser _parser;

        public LocalizationFileReader(ILocalizationParser parser)
        {
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

            var files = Directory.EnumerateFiles(directoryPath,"*.yml",SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                try
                {
                    var localizationFile = ReadFile(filePath);
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

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "O arquivo de localização não foi encontrado.",
                    filePath);
            }

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            return _parser.Parse(filePath, lines);                      
        }

        
    }
}
