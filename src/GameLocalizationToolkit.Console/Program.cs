using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Services;
using GameLocalizationToolkit.Infrastructure.FileSystem;
using GameLocalizationToolkit.Infrastructure.Parsers;

#region Configuração inicial
/*
 Configura o título e exibe o cabeçalho inicial da aplicação.
 */
Console.Title = "Game Localization Toolkit";

Console.WriteLine("====================================");
Console.WriteLine("Game Localization Toolkit");
Console.WriteLine("====================================");
Console.WriteLine();
#endregion

#region Leitura dos caminhos
/*
 Solicita ao usuário os caminhos da pasta original do jogo
 e da pasta que contém os arquivos traduzidos do mod.

 Os caminhos são validados e normalizados antes do processamento.
 */
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
#endregion

#region Criação dos serviços
/*
 Cria os componentes responsáveis por:

 - interpretar os arquivos de localização;
 - ler arquivos e diretórios;
 - comparar os conteúdos das duas pastas;
 - realizar o merge entre arquivos;
 - coordenar o merge completo dos diretórios;
 - gravar o resultado em disco.
 */
ILocalizationParser parser =
    new ParadoxLocalizationParser();

ILocalizationFileReader reader =
    new LocalizationFileReader(parser);

ILocalizationDirectoryComparer comparer =
    new LocalizationDirectoryComparer();

ILocalizationMerger merger =
    new LocalizationMerger();

ILocalizationDirectoryMerger directoryMerger =
    new LocalizationDirectoryMerger(merger);

ILocalizationWriter writer =
    new LocalizationWriter();
#endregion

try
{
    #region Leitura dos diretórios
    /*
     Lê recursivamente todos os arquivos .yml encontrados nas pastas
     informadas e transforma seus conteúdos em objetos de localização.
     */
    Console.WriteLine();
    Console.WriteLine("Analisando a pasta original do jogo...");

    var sourceResult =
        reader.ReadDirectory(sourceDirectoryPath);

    Console.WriteLine("Analisando a pasta do mod traduzido...");

    var targetResult =
        reader.ReadDirectory(targetDirectoryPath);
    #endregion

    #region Comparação dos diretórios
    /*
     Compara todas as chaves encontradas nas duas pastas para identificar:

     - novas chaves existentes apenas no jogo;
     - chaves existentes apenas no mod;
     - chaves correspondentes presentes nos dois lados.
     */
    Console.WriteLine();
    Console.WriteLine("Comparando as localizações...");

    var comparisonResult =
        comparer.Compare(sourceResult, targetResult);
    #endregion

    #region Resultado da leitura
    /*
     Exibe as informações gerais das duas pastas analisadas.
     */
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
    #endregion

    #region Resumo da comparação
    /*
     Exibe a quantidade de chaves adicionadas, removidas
     e correspondentes encontradas durante a comparação.
     */
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
    #endregion

    #region Exibição das novas chaves
    /*
     Exibe uma amostra das primeiras novas chaves encontradas,
     limitando a saída para evitar sobrecarregar o Console.
     */
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
    #endregion

    #region Merge completo
    /*
     Realiza o merge completo entre os arquivos da pasta original
     e os arquivos traduzidos do mod.

     O resultado é inicialmente gerado em memória e depois pode ser
     gravado em uma pasta de saída informada pelo usuário.
     */
    Console.WriteLine();
    Console.WriteLine("Gerando merge completo em memória...");

    var mergedResult =
        directoryMerger.Merge(sourceResult, targetResult);

    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine("Resultado do merge completo");
    Console.WriteLine("====================================");
    Console.WriteLine();

    Console.WriteLine(
        $"Arquivos gerados: {mergedResult.TotalFiles:N0}");

    Console.WriteLine(
        $"Chaves geradas: {mergedResult.TotalEntries:N0}");

    Console.WriteLine(
        $"Erros acumulados: {mergedResult.Errors.Count:N0}");
    #endregion

    #region Escrita dos arquivos
    /*
     Solicita uma pasta de saída e grava nela o resultado do merge.

     Os arquivos originais do jogo e do mod não são alterados.
     */
    Console.WriteLine();
    Console.Write("Informe a pasta onde deseja salvar o resultado: ");

    var outputDirectoryPath =
        Console.ReadLine()?.Trim().Trim('"');

    if (string.IsNullOrWhiteSpace(outputDirectoryPath))
    {
        Console.WriteLine();
        Console.WriteLine("Nenhuma pasta de saída foi informada.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Gravando arquivos...");

    writer.WriteDirectory(
        mergedResult,
        outputDirectoryPath);

    Console.WriteLine();
    Console.WriteLine("Arquivos gravados com sucesso.");

    Console.WriteLine(
        $"Pasta de saída: {outputDirectoryPath}");

    Console.WriteLine(
        $"Arquivos gravados: {mergedResult.TotalFiles:N0}");

    Console.WriteLine(
        $"Chaves gravadas: {mergedResult.TotalEntries:N0}");
    #endregion

    #region Exibição dos erros
    /*
     Reúne e exibe os erros encontrados durante a leitura
     das duas pastas, limitando a saída aos primeiros dez registros.
     */
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
    #endregion
}
#region Tratamento de erros
/*
 Trata os principais erros que podem ocorrer durante a leitura,
 comparação, merge e gravação dos arquivos de localização.
 */
catch (DirectoryNotFoundException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Pasta não encontrada: {exception.Message}");
}
catch (FileNotFoundException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Arquivo não encontrado: {exception.FileName}");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine();
    Console.WriteLine(
        "O programa não possui permissão para acessar uma das pastas ou arquivos.");
}
catch (ArgumentException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Dados inválidos: {exception.Message}");
}
catch (IOException exception)
{
    Console.WriteLine();
    Console.WriteLine($"Erro ao acessar ou gravar arquivos: {exception.Message}");
}
catch (Exception exception)
{
    Console.WriteLine();
    Console.WriteLine($"Ocorreu um erro inesperado: {exception.Message}");
}
#endregion

#region Encerramento
/*
 Mantém o Console aberto para que o usuário possa visualizar
 os resultados antes de encerrar a aplicação.
 */
Console.WriteLine();
Console.WriteLine("Pressione qualquer tecla para encerrar...");
Console.ReadKey();
#endregion