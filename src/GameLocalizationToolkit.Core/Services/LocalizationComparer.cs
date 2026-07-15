using GameLocalizationToolkit.Core.Models;


namespace GameLocalizationToolkit.Core.Services
{
    public class LocalizationComparer
    {
        public LocalizationComparisonResult Compare(LocalizationFile source,LocalizationFile target)
        {
            var result = new LocalizationComparisonResult();

            FindAddedEntries(source, target, result);

            FindRemovedEntries(source, target, result);

            FindMatchedEntries(source, target, result);

            //FindModifiedEntries(source, target, result);


            return result;
        }

        #region FindAddedEntries
        /*
         Identifica as chaves que existem no arquivo de origem (Source = Arquivo do jogo (versão nova)),
         mas ainda não existem no arquivo de destino (Target = Arquivo do mod (tradução)).

         Essas chaves representam novas localizações adicionadas pelo jogo
         e que ainda não foram traduzidas no mod.

         Todas as entradas encontradas são adicionadas à coleção AddedEntries.
         */
        #endregion
        private void FindAddedEntries(LocalizationFile source, LocalizationFile target, LocalizationComparisonResult result)
        {
            var targetEntries = target.Entries.ToDictionary(entry => entry.Key);

            foreach (var sourceEntry in source.Entries)
            {
                if (!targetEntries.ContainsKey(sourceEntry.Key))
                {
                    result.AddedEntries.Add(sourceEntry);
                }
            }
        }


        #region FindRemovedEntries
        /*
         Identifica as chaves que existem no arquivo de destino (Target = Arquivo do mod (tradução)),
         mas não existem mais no arquivo de origem (Source = Arquivo do jogo (versão nova)).

         Essas entradas representam traduções que deixaram de existir na
         versão atual do jogo e podem ser removidas do mod.

         Todas as entradas encontradas são adicionadas à coleção RemovedEntries.
         */
        #endregion
        private void FindRemovedEntries(LocalizationFile source, LocalizationFile target, LocalizationComparisonResult result)
        {
            var sourceEntries = source.Entries.ToDictionary(entry => entry.Key);

            foreach (var targetEntry in target.Entries)
            {
                if (!sourceEntries.ContainsKey(targetEntry.Key))
                {
                    result.RemovedEntries.Add(targetEntry);
                }
            }
        }

        #region FindMatchedEntries
        /*
         Identifica as chaves presentes tanto no arquivo de origem
         quanto no arquivo de destino.

         A comparação considera somente a existência da chave.
         Os valores não são comparados porque o jogo e o mod
         normalmente utilizam idiomas diferentes.

         Todas as correspondências encontradas são adicionadas
         à coleção MatchedEntries.
         */
        #endregion
        private void FindMatchedEntries(LocalizationFile source,LocalizationFile target, LocalizationComparisonResult result)
        {
            var targetEntries = target.Entries.ToDictionary(entry => entry.Key);

            foreach (var sourceEntry in source.Entries)
            {
                if (targetEntries.ContainsKey(sourceEntry.Key))
                {
                    result.MatchedEntries.Add(sourceEntry);
                }
            }
        }

        #region FindModifiedEntries
        /*
         Identifica as entradas que existem tanto no arquivo de origem
         quanto no arquivo de destino, mas possuem diferenças no valor
         ou na versão.

         As diferenças encontradas são representadas por objetos
         LocalizationEntryDifference e adicionadas à coleção ModifiedEntries.
         */
        #endregion
        private void FindModifiedEntries(LocalizationFile source,LocalizationFile target, LocalizationComparisonResult result)
        {
            var targetEntries = target.Entries.ToDictionary(entry => entry.Key);

            foreach (var sourceEntry in source.Entries)
            {
                if (!targetEntries.TryGetValue(sourceEntry.Key, out var targetEntry))
                {
                    continue;
                }

                var difference = new LocalizationEntryDifference
                {
                    Source = sourceEntry,
                    Target = targetEntry
                };

                if (difference.HasChanges)
                {
                    result.ModifiedEntries.Add(difference);
                }
            }
        }
    }
}
