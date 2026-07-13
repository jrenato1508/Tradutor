using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models
{
    
    public sealed class LocalizationEntry
    {
        public required string Key { get; init; }

        public required string Value { get; init; }

        public string? Version { get; init; }

        public required string SourceFile { get; init; }

        public int LineNumber { get; init; }
    }
}
