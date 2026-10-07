using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameLocalizationToolkit.Core.Services
{
    public class LocalizationTokenProtector : ILocalizationTokenProtector
    {
        private static readonly Regex TokenRegex = new(@"(\[[^\]]+\]|\$[^$]+\$|@[A-Za-z0-9_]+!|#[A-Za-z0-9_]+|#!)", RegexOptions.Compiled);

        public ProtectedLocalizationText Protect(string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            var tokens = new Dictionary<string, string>();

            var index = 0;

            var protectedText = TokenRegex.Replace(text,match =>{ var placeholder = $"__GLT_TOKEN_{index}__";
                                tokens.Add( placeholder, match.Value); index++; return placeholder; });

            return new ProtectedLocalizationText
            {
                Text = protectedText,
                Tokens = tokens
            };
        }

        public string Restore(ProtectedLocalizationText protectedText)
        {
            ArgumentNullException.ThrowIfNull(protectedText);

            var restoredText = protectedText.Text;

            foreach (var token in protectedText.Tokens)
            {
                restoredText = restoredText.Replace(
                    token.Key,
                    token.Value,
                    StringComparison.Ordinal);
            }

            return restoredText;
        }
    }
}
