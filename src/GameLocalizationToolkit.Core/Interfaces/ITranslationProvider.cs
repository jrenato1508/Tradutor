using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Interfaces
{
    public interface ITranslationProvider
    {
        Task<string> TranslateAsync(string text, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken = default);
    }
}
