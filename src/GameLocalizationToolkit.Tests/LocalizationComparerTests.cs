using GameLocalizationToolkit.Core.Models;
using GameLocalizationToolkit.Core.Services;

namespace GameLocalizationToolkit.Tests
{
    public class LocalizationComparerTests
    {
        [Fact]
        public void Compare_DeveIdentificarEntradasAdicionadas()
        {
            // Arrange
            var comparer = new LocalizationComparer();

            var source = CreateLocalizationFile(
                CreateEntry("king", "King"),
                CreateEntry("queen", "Queen"),
                CreateEntry("duke", "Duke"));

            var target = CreateLocalizationFile(
                CreateEntry("king", "Rei"),
                CreateEntry("queen", "Rainha"));

            // Act
            var result = comparer.Compare(source, target);

            // Assert
            Assert.Single(result.AddedEntries);
            Assert.Equal("duke", result.AddedEntries[0].Key);
        }

        [Fact]
        public void Compare_DeveIdentificarEntradasRemovidas()
        {
            // Arrange
            var comparer = new LocalizationComparer();

            var source = CreateLocalizationFile(
                CreateEntry("king", "King"),
                CreateEntry("queen", "Queen"));

            var target = CreateLocalizationFile(
                CreateEntry("king", "Rei"),
                CreateEntry("queen", "Rainha"),
                CreateEntry("duke", "Duque"));

            // Act
            var result = comparer.Compare(source, target);

            // Assert
            Assert.Single(result.RemovedEntries);
            Assert.Equal("duke", result.RemovedEntries[0].Key);
        }

        [Fact]
        public void Compare_DeveIdentificarEntradasNaoAlteradas()
        {
            // Arrange
            var comparer = new LocalizationComparer();

            var source = CreateLocalizationFile(
                CreateEntry("king", "King"),
                CreateEntry("queen", "Queen"),
                CreateEntry("duke", "Duke"));

            var target = CreateLocalizationFile(
                CreateEntry("king", "Rei"),
                CreateEntry("queen", "Rainha"));

            // Act
            var result = comparer.Compare(source, target);

            // Assert
            Assert.Equal(2, result.UnchangedEntries.Count);

            Assert.Contains(
                result.UnchangedEntries,
                entry => entry.Key == "king");

            Assert.Contains(
                result.UnchangedEntries,
                entry => entry.Key == "queen");
        }

        private static LocalizationFile CreateLocalizationFile(
            params LocalizationEntry[] entries)
        {
            var file = new LocalizationFile
            {
                FilePath = "localization.yml",
                Language = "l_english"
            };

            file.Entries.AddRange(entries);

            return file;
        }

        private static LocalizationEntry CreateEntry(
            string key,
            string value)
        {
            return new LocalizationEntry
            {
                Key = key,
                Value = value,
                Version = "0",
                SourceFile = "localization.yml",
                LineNumber = 1
            };
        }
    }
}