using GameLocalizationToolkit.Core.Enums;
using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using GameLocalizationToolkit.Core.Services;
using GameLocalizationToolkit.Infrastructure.FileSystem;
using GameLocalizationToolkit.Infrastructure.Parsers;
using GameLocalizationToolkit.Infrastructure.Translation;

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


#region Solicitação do modo de tradução
/*
 Define se o usuário deseja:

 1 - Atualizar uma tradução já existente.
 2 - Criar uma nova tradução a partir dos arquivos originais do jogo.
 */

Console.WriteLine("Como deseja utilizar o programa?");
Console.WriteLine();
Console.WriteLine("1 - Atualizar uma tradução existente");
Console.WriteLine("2 - Criar uma nova tradução");
Console.WriteLine();

Console.Write("Opção: ");
var option = Console.ReadLine();

TranslationMode translationMode;

switch (option)
{
    case "1":
        translationMode = TranslationMode.UpdateExistingTranslation;
        break;

    case "2":
        translationMode = TranslationMode.CreateNewTranslation;
        break;

    default:
        Console.WriteLine();
        Console.WriteLine("Opção inválida.");
        return;
}

Console.WriteLine();

#endregion


#region Leitura dos caminhos
/*
 Solicita ao usuário a pasta original do jogo.

 Caso o modo selecionado seja de atualização de uma tradução existente,
 também solicita a pasta do mod utilizado como base.
 */

Console.Write("Informe o caminho da pasta original do jogo: ");
var sourceDirectoryPath = Console.ReadLine();

if (string.IsNullOrWhiteSpace(sourceDirectoryPath))
{
    Console.WriteLine();
    Console.WriteLine("A pasta original do jogo precisa ser informada.");
    return;
}

sourceDirectoryPath =
    sourceDirectoryPath.Trim().Trim('"');

string? targetDirectoryPath = null;

if (translationMode == TranslationMode.UpdateExistingTranslation)
{
    Console.Write("Informe o caminho da pasta do mod traduzido: ");
    targetDirectoryPath = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(targetDirectoryPath))
    {
        Console.WriteLine();
        Console.WriteLine("A pasta do mod traduzido precisa ser informada.");
        return;
    }

    targetDirectoryPath =
        targetDirectoryPath.Trim().Trim('"');
}

#endregion


#region Criação dos serviços
/*
 Cria os componentes responsáveis por:

 - interpretar os arquivos de localização;
 - ler arquivos e diretórios;
 - comparar os conteúdos das duas pastas;
 - realizar o merge entre arquivos;
 - coordenar o merge completo dos diretórios;
 - gravar o resultado em disco;
 - determinar quais entradas precisam ser traduzidas.
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

ILocalizationTranslationPlanner translationPlanner =
    new LocalizationTranslationPlanner();

ILocalizationTokenProtector tokenProtector =
    new LocalizationTokenProtector();

ITranslationExporter translationExporter =
    new TranslationExporter(tokenProtector);

#endregion


try
{
    #region Leitura da pasta original
    /*
     Lê recursivamente todos os arquivos .yml encontrados
     na pasta original do jogo.
     */

    Console.WriteLine();
    Console.WriteLine("Analisando a pasta original do jogo...");

    var sourceResult =
        reader.ReadDirectory(sourceDirectoryPath);

    Console.WriteLine();
    Console.WriteLine($"Arquivos encontrados: {sourceResult.TotalFiles:N0}");
    Console.WriteLine($"Chaves encontradas: {sourceResult.TotalEntries:N0}");
    Console.WriteLine($"Erros encontrados: {sourceResult.Errors.Count:N0}");

    #endregion


    #region Leitura e comparação com mod existente
    /*
     Esta etapa é executada apenas quando o usuário possui
     uma tradução existente para utilizar como base.

     Nesse cenário, as duas pastas são comparadas para identificar:

     - novas chaves;
     - chaves removidas;
     - chaves já existentes no mod.
     */

    LocalizationScanResult? targetResult = null;
    LocalizationDirectoryComparisonResult? comparisonResult = null;

    if (translationMode == TranslationMode.UpdateExistingTranslation)
    {
        Console.WriteLine();
        Console.WriteLine("Analisando a pasta do mod traduzido...");

        targetResult =
            reader.ReadDirectory(targetDirectoryPath!);

        Console.WriteLine();
        Console.WriteLine("Comparando as localizações...");

        comparisonResult =
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
    }

    #endregion


    #region Planejamento da tradução
    /*
     Define quais entradas precisam ser traduzidas.

     Atualização de mod:
     - somente as novas chaves encontradas durante a comparação.

     Nova tradução:
     - todas as chaves existentes na pasta original do jogo.
     */

    var entriesToTranslate =
        translationPlanner
            .GetEntriesToTranslate(
                translationMode,
                sourceResult,
                comparisonResult)
            .ToList();


    #region Validação da proteção dos tokens;

    if (entriesToTranslate.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("====================================");
        Console.WriteLine("Validação da proteção dos tokens");
        Console.WriteLine("====================================");

        var tokenValidationErrors = new List<LocalizationEntry>();

        foreach (var entry in entriesToTranslate)
        {
            var protectedText =
                tokenProtector.Protect(entry.Value);

            var restoredText =
                tokenProtector.Restore(protectedText);

            if (!string.Equals(
                entry.Value,
                restoredText,
                StringComparison.Ordinal))
            {
                tokenValidationErrors.Add(entry);
            }
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Entradas verificadas: {entriesToTranslate.Count:N0}");

        Console.WriteLine(
            $"Falhas na restauração: {tokenValidationErrors.Count:N0}");

        if (tokenValidationErrors.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Primeiras entradas com problema:");

            foreach (var entry in tokenValidationErrors.Take(20))
            {
                Console.WriteLine(
                    $"- {entry.Key}: \"{entry.Value}\"");
            }
        }
    }

    #endregion




    Console.WriteLine();
    Console.WriteLine("====================================");
    Console.WriteLine("Plano de tradução");
    Console.WriteLine("====================================");
    Console.WriteLine();

    var translationModeDescription =
        translationMode == TranslationMode.UpdateExistingTranslation
            ? "Atualizar tradução existente"
            : "Criar nova tradução";

    Console.WriteLine(
        $"Modo selecionado: {translationModeDescription}");

    Console.WriteLine(
        $"Entradas que precisam ser traduzidas: " +
        $"{entriesToTranslate.Count:N0}");

    #endregion


    #region Resumo da comparação
    /*
     Exibe informações específicas da comparação somente quando
     existe um mod utilizado como base.
     */

    if (comparisonResult is not null)
    {
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
    }

    #endregion


    #region Exibição das entradas para tradução
    /*
     Exibe uma pequena amostra das entradas que fazem parte
     da fila de tradução.
     */

    if (entriesToTranslate.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Primeiras entradas que precisam ser traduzidas:");

        foreach (var entry in entriesToTranslate.Take(20))
        {
            Console.WriteLine(
                $"- {entry.Key}: \"{entry.Value}\"");
        }

        if (entriesToTranslate.Count > 20)
        {
            Console.WriteLine(
                $"- Outras " +
                $"{entriesToTranslate.Count - 20:N0} " +
                "entradas não exibidas.");
        }
    }

    #endregion

    #region Exportação para tradução

    if (entriesToTranslate.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Deseja exportar as entradas para tradução? (S/N)");
        Console.Write("Opção: ");

        var exportOption = Console.ReadLine()?.Trim();

        if (string.Equals(exportOption, "S", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine();
            Console.Write("Informe a pasta onde deseja salvar os arquivos de tradução: ");

            var translationOutputPath = Console.ReadLine()?.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(translationOutputPath))
            {
                Console.WriteLine();
                Console.WriteLine("Nenhuma pasta de saída foi informada.");
            }
            else
            {
                const int entriesPerFile = 500;

                var exportResult = translationExporter.Export(entriesToTranslate, translationOutputPath, entriesPerFile);

                Console.WriteLine();
                Console.WriteLine("Arquivos para tradução gerados com sucesso.");

                Console.WriteLine($"Entradas pendentes: {entriesToTranslate.Count:N0}");

                Console.WriteLine($"Entradas exportadas: {exportResult.ExportedEntries:N0}");

                Console.WriteLine($"Entradas sem conteúdo traduzível: " + $"{entriesToTranslate.Count - exportResult.ExportedEntries:N0}");

                Console.WriteLine($"Arquivos gerados: {exportResult.GeneratedFiles:N0}");

                Console.WriteLine($"Pasta de saída: {translationOutputPath}");

                Console.WriteLine();
                Console.WriteLine("As entradas foram exportadas para tradução.");
                Console.WriteLine("Após traduzir os arquivos, execute novamente o programa para importar as traduções.");

                return;
            }
        }
    }

    #endregion

    #region Atualização de tradução existente
    /*
     O merge e a geração dos arquivos atuais somente fazem sentido
     quando existe um mod utilizado como base.

     O modo de criação de uma tradução do zero ainda será implementado
     junto ao pipeline de tradução automática.
     */

    if (translationMode == TranslationMode.UpdateExistingTranslation)
    {
        if (targetResult is null)
        {
            throw new InvalidOperationException(
                "O resultado da leitura do mod não está disponível.");
        }

        #region Merge completo

        Console.WriteLine();
        Console.WriteLine("Gerando merge completo em memória...");

        var mergedResult =
            directoryMerger.Merge(
                sourceResult,
                targetResult);

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
    }
    else
    {
        Console.WriteLine();
        Console.WriteLine(
            "A geração de uma tradução completa será realizada " +
            "na próxima etapa do pipeline de tradução.");
    }

    #endregion


    #region Exibição dos erros
    /*
     Exibe os erros encontrados durante a leitura.

     Caso exista um mod base, os erros das duas leituras são reunidos.
     */

    var errors = new List<string>();

    errors.AddRange(sourceResult.Errors);

    if (targetResult is not null)
    {
        errors.AddRange(targetResult.Errors);
    }

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
 comparação, planejamento, merge e gravação dos arquivos.
 */

catch (DirectoryNotFoundException exception)
{
    Console.WriteLine();
    Console.WriteLine(
        $"Pasta não encontrada: {exception.Message}");
}
catch (FileNotFoundException exception)
{
    Console.WriteLine();
    Console.WriteLine(
        $"Arquivo não encontrado: {exception.FileName}");
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
    Console.WriteLine(
        $"Dados inválidos: {exception.Message}");
}
catch (IOException exception)
{
    Console.WriteLine();
    Console.WriteLine(
        $"Erro ao acessar ou gravar arquivos: {exception.Message}");
}
catch (Exception exception)
{
    Console.WriteLine();
    Console.WriteLine(
        $"Ocorreu um erro inesperado: {exception.Message}");
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