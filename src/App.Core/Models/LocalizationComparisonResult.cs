using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Models
{
    public class LocalizationComparisonResult
    {
        public List<LocalizationEntry> AddedEntries { get; } = [];

        public List<LocalizationEntry> RemovedEntries { get; } = [];

        public List<LocalizationEntry> UnchangedEntries { get; } = [];
    }
}
