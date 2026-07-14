using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Services
{
    public class LocalizationComparer
    {
        public LocalizationComparisonResult Compare(LocalizationFile source,LocalizationFile target)
        {
            var result = new LocalizationComparisonResult();

            FindAddedEntries(source, target, result);

            FindRemovedEntries(source, target, result);

            FindUnchangedEntries(source, target, result);

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



        #region FindUnchangedEntries
        /*
         Identifica as chaves presentes tanto no arquivo de origem (Source = Arquivo do jogo (versão nova))
         quanto no arquivo de destino (Target = Arquivo do mod (tradução)).

         Essas entradas representam traduções que continuam válidas após a
         atualização do jogo e, portanto, não necessitam de nenhuma alteração.

         Todas as entradas encontradas são adicionadas à coleção UnchangedEntries.
         */
        #endregion
        private void FindUnchangedEntries(LocalizationFile source,LocalizationFile target, LocalizationComparisonResult result)
        {
            var targetEntries = target.Entries.ToDictionary(entry => entry.Key);

            foreach (var sourceEntry in source.Entries)
            {
                if (targetEntries.ContainsKey(sourceEntry.Key))
                {
                    result.UnchangedEntries.Add(sourceEntry);
                }
            }
        }
    }
}
