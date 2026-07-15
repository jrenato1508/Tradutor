namespace GameLocalizationToolkit.Core.Models
{
    public sealed class LocalizationComparisonResult
    {
        //existem no jogo, mas não no mod;
        public List<LocalizationEntry> AddedEntries { get; } = [];
        
        //RemovedEntries: existem no mod, mas não no jogo;
        public List<LocalizationEntry> RemovedEntries { get; } = [];

        public List<LocalizationEntry> MatchedEntries { get; } = [];

        #region ModifiedEntries
        /*
         * ModifiedEntries: existem nos dois, mas o valor ou a versão mudou.
         Representa entradas existentes tanto no arquivo de origem quanto
         no arquivo de destino que possuem diferenças identificadas durante
         a comparação.

         Atualmente esta coleção está preparada para suportar comparações
         de conteúdo (valor e versão), porém essa funcionalidade depende da
         comparação entre diferentes versões do arquivo de origem (ex. jogo
         antigo x jogo atualizado).

         Na comparação atual entre Jogo x Mod, os valores das traduções
         naturalmente serão diferentes por estarem em idiomas distintos.
         Portanto, esta coleção permanece preparada para uma evolução futura
         da ferramenta.
        */
        #endregion
        public List<LocalizationEntryDifference> ModifiedEntries { get; } = [];
    }
}
