using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public sealed class LocalizationDirectoryComparisonResult
    {
        public List<LocalizationEntry> AddedEntries { get; } = [];

        public List<LocalizationEntry> RemovedEntries { get; } = [];

        public List<LocalizationEntry> MatchedEntries { get; } = [];
    }
}
