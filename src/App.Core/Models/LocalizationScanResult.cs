using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public sealed class LocalizationScanResult
    {
        public List<LocalizationFile> Files { get; init; } = [];

        public List<string> Errors { get; init; } = [];

        public int TotalFiles => Files.Count;

        public int TotalEntries => Files.Sum(file => file.Entries.Count);
    }
}
