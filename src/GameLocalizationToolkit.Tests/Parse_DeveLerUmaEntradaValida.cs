using GameLocalizationToolkit.Infrastructure.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLocalizationToolkit.Tests
{
    internal class Parse_DeveLerUmaEntradaValida
    {
        [Fact]
        public void Parse_DeveLerUmaEntradaValida()
        {
            // Arrange
            var parser = new ParadoxLocalizationParser();

            var lines = new[]
            {
        "l_english:",
        "king_feudal_male:0 \"King\""
    };

            // Act
            var result = parser.Parse("titles_l_english.yml", lines);

            // Assert
            Assert.Single(result.Entries);

            var entry = result.Entries.First();

            Assert.Equal("king_feudal_male", entry.Key);
            Assert.Equal("King", entry.Value);
            Assert.Equal("0", entry.Version);
        }
    }
}
