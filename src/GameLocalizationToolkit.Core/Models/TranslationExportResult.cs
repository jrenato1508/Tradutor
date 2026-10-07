using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public sealed class TranslationExportResult
    {
        public int ExportedEntries { get; init; }

        public int GeneratedFiles { get; init; }
    }
}
