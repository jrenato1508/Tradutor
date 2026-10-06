using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public sealed class ProtectedLocalizationText
    {
        public required string Text { get; init; }

        public Dictionary<string, string> Tokens { get; init; } = [];
    }
}
