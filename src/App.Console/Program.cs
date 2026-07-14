using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Services;




Console.Title = "Paradox Localization Toolkit";

Console.WriteLine("====================================");
Console.WriteLine(" Paradox Localization Toolkit");
Console.WriteLine("====================================");
Console.WriteLine();

Console.Write("Informe o caminho da pasta de localização: ");

var directoryPath = Console.ReadLine();

if (string.IsNullOrWhiteSpace(directoryPath))
{
    Console.WriteLine();
    Console.WriteLine("Nenhuma pasta foi informada.");
    return;
}

directoryPath = directoryPath.Trim().Trim('"');

ILocalizationFileReader reader = new LocalizationFileReader();

try
{
    Console.WriteLine();
    Console.WriteLine("Analisando arquivos...");
    Console.WriteLine();

    var result = reader.ReadDirectory(directoryPath);

    Console.WriteLine($"Pasta analisada: {directoryPath}");
    Console.WriteLine($"Arquivos encontrados: {result.TotalFiles:N0}");
    Console.WriteLine($"Chaves encontradas: {result.TotalEntries:N0}");
    Console.WriteLine($"Erros encontrados: {result.Errors.Count:N0}");

    var languages = result.Files
        .GroupBy(file => file.Language)
        .OrderBy(group => group.Key);

    Console.WriteLine();
    Console.WriteLine("Idiomas identificados:");

    foreach (var language in languages)
    {
        var entryCount = language.Sum(file => file.Entries.Count);

        Console.WriteLine(
            $"- {language.Key}: {language.Count():N0} arquivos e " +
            $"{entryCount:N0} chaves");
    }

    if (result.Errors.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Erros:");

        foreach (var error in result.Errors.Take(10))
        {
            Console.WriteLine($"- {error}");
        }

        if (result.Errors.Count > 10)
        {
            Console.WriteLine(
                $"- Outros {result.Errors.Count - 10:N0} erros não exibidos.");
        }
    }
}
catch (DirectoryNotFoundException exception)
{
    Console.WriteLine($"Pasta não encontrada: {exception.Message}");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine(
        "O programa não possui permissão para acessar essa pasta.");
}
catch (Exception exception)
{
    Console.WriteLine($"Ocorreu um erro inesperado: {exception.Message}");
}

Console.WriteLine();
Console.WriteLine("Pressione qualquer tecla para encerrar...");
Console.ReadKey();