using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public sealed class LocalizationFile
    {
        public required string FilePath { get; init; }

        public required string Language { get; init; }

        public List<LocalizationEntry> Entries { get; init; } = [];
    }
}
