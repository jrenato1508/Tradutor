using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Services
{
    public sealed class LocalizationTranslationService 
    {
        private readonly ITranslationProvider _translationProvider;
        private readonly ILocalizationTokenProtector _tokenProtector;

        public LocalizationTranslationService( ITranslationProvider translationProvider, ILocalizationTokenProtector tokenProtector)
        {
            ArgumentNullException.ThrowIfNull(translationProvider);
            ArgumentNullException.ThrowIfNull(tokenProtector);

            _translationProvider = translationProvider;
            _tokenProtector = tokenProtector;
        }

        public async Task<LocalizationEntry> TranslateAsync( LocalizationEntry entry, string sourceLanguage, string targetLanguage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);

            var protectedText = _tokenProtector.Protect(entry.Value);

            var translatedText = await _translationProvider.TranslateAsync( protectedText.Text,sourceLanguage, targetLanguage, cancellationToken);

            var restoredText = _tokenProtector.Restore( new ProtectedLocalizationText {Text = translatedText,Tokens = protectedText.Tokens });

            return new LocalizationEntry
            {
                Key = entry.Key,
                Value = restoredText,
                Version = entry.Version,
                SourceFile = entry.SourceFile,
                LineNumber = entry.LineNumber
            };
        }
    }
}
