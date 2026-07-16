using GameLocalizationToolkit.Core.Interfaces;
using GameLocalizationToolkit.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Core.Services
{
    public sealed class LocalizationDirectoryComparer : ILocalizationDirectoryComparer
    {
        public LocalizationDirectoryComparisonResult Compare(LocalizationScanResult source, LocalizationScanResult target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            var result = new LocalizationDirectoryComparisonResult();

            var sourceEntries = source.Files
                .SelectMany(file => file.Entries)
                .GroupBy(entry => entry.Key)
                .ToDictionary(
                    group => group.Key,
                    group => group.First());

            var targetEntries = target.Files
                .SelectMany(file => file.Entries)
                .GroupBy(entry => entry.Key)
                .ToDictionary(
                    group => group.Key,
                    group => group.First());

            foreach (var sourceEntry in sourceEntries.Values)
            {
                if (targetEntries.ContainsKey(sourceEntry.Key))
                {
                    result.MatchedEntries.Add(sourceEntry);
                }
                else
                {
                    result.AddedEntries.Add(sourceEntry);
                }
            }

            foreach (var targetEntry in targetEntries.Values)
            {
                if (!sourceEntries.ContainsKey(targetEntry.Key))
                {
                    result.RemovedEntries.Add(targetEntry);
                }
            }

            return result;



        }
    }
}
