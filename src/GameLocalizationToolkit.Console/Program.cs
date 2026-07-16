using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Services;
using GameLocalizationToolkit.Infrastructure.FileSystem;
using GameLocalizationToolkit.Infrastructure.Parsers;

Console.Title = "Game Localization Toolkit";

Console.WriteLine("====================================");
Console.WriteLine("Game Localization Toolkit");
Console.WriteLine("====================================");
Console.WriteLine();

Console.Write("Informe o caminho da pasta original do jogo: ");
var sourceDirectoryPath = Console.ReadLine();

Console.Write("Informe o caminho da pasta do mod traduzido: ");
var targetDirectoryPath = Console.ReadLine();

if (string.IsNullOrWhiteSpace(sourceDirectoryPath) ||
    string.IsNullOrWhiteSpace(targetDirectoryPath))
{
    Console.WriteLine();
    Console.WriteLine("As duas pastas precisam ser informadas.");
    return;
}

sourceDirectoryPath = sourceDirectoryPath.Trim().Trim('"');
targetDirectoryPath = targetDirectoryPath.Trim().Trim('"');

ILocalizationParser parser = new ParadoxLocalizationParser();

ILocalizationFileReader reader =
    new LocalizationFileReader(parser);

ILocalizationDirectoryComparer comparer =
    new LocalizationDirectoryComparer();

try
{
    Console.WriteLine();
    Console.WriteLine("Analisando a pasta original do jogo...");

    var sourceResult =
        reader.ReadDirectory(sourceDirectoryPath);

    Console.WriteLine("Analisando a pasta do mod traduzido...");

    var targetResult =
        reader.ReadDirectory(targetDirectoryPath);

    Console.WriteLine();
    Console.WriteLine("Comparando as localizações...");

    var comparisonResult =
        comparer.Compare(sourceResult, targetResult);

    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine("Resultado da comparação");
    Console.WriteLine("====================================");
    Console.WriteLine();

    Console.WriteLine($"Pasta original: {sourceDirectoryPath}");
    Console.WriteLine($"Arquivos encontrados: {sourceResult.TotalFiles:N0}");
    Console.WriteLine($"Chaves encontradas: {sourceResult.TotalEntries:N0}");
    Console.WriteLine($"Erros encontrados: {sourceResult.Errors.Count:N0}");

    Console.WriteLine();

    Console.WriteLine($"Pasta do mod: {targetDirectoryPath}");
    Console.WriteLine($"Arquivos encontrados: {targetResult.TotalFiles:N0}");
    Console.WriteLine($"Chaves encontradas: {targetResult.TotalEntries:N0}");
    Console.WriteLine($"Erros encontrados: {targetResult.Errors.Count:N0}");

    Console.WriteLine();
    Console.WriteLine("Resumo:");

    Console.WriteLine(
        $"Novas chaves para traduzir: " +
        $"{comparisonResult.AddedEntries.Count:N0}");

    Console.WriteLine(
        $"Chaves removidas do jogo: " +
        $"{comparisonResult.RemovedEntries.Count:N0}");

    Console.WriteLine(
        $"Chaves já existentes no mod: " +
        $"{comparisonResult.MatchedEntries.Count:N0}");

    if (comparisonResult.AddedEntries.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Primeiras novas chaves encontradas:");

        foreach (var entry in comparisonResult.AddedEntries.Take(20))
        {
            Console.WriteLine(
                $"- {entry.Key}: \"{entry.Value}\"");
        }

        if (comparisonResult.AddedEntries.Count > 20)
        {
            Console.WriteLine(
                $"- Outras " +
                $"{comparisonResult.AddedEntries.Count - 20:N0} " +
                "chaves não exibidas.");
        }
    }

    var errors = sourceResult.Errors
        .Concat(targetResult.Errors)
        .ToList();

    if (errors.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Erros encontrados durante a leitura:");

        foreach (var error in errors.Take(10))
        {
            Console.WriteLine($"- {error}");
        }

        if (errors.Count > 10)
        {
            Console.WriteLine(
                $"- Outros {errors.Count - 10:N0} erros não exibidos.");
        }
    }
}
catch (DirectoryNotFoundException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Pasta não encontrada: {exception.Message}");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine();
    Console.WriteLine(
        "O programa não possui permissão para acessar uma das pastas.");
}
catch (ArgumentException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Caminho inválido: {exception.Message}");
}
catch (Exception exception)
{
    Console.WriteLine();
    Console.WriteLine($"Ocorreu um erro inesperado: {exception.Message}");
}

Console.WriteLine();
Console.WriteLine("Pressione qualquer tecla para encerrar...");
Console.ReadKey();