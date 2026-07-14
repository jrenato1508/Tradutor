using GameLocalizationToolkit.Infrastructure.Parsers;

namespace GameLocalizationToolkit.Tests
{
    public class ParadoxLocalizationParserTests
    {
        [Fact]
        public void Test1()
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
            Assert.Equal("l_english", result.Language);
        }

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

        [Fact]
        public void Parse_DeveIgnorarComentariosELinhasVazias()
        {
            // Arrange
            var parser = new ParadoxLocalizationParser();

            var lines = new[]
            {
                "l_english:",
                "",
                "# Este é um comentário",
                "",
                "king_feudal_male:0 \"King\""
            };

            // Act
            var result = parser.Parse("titles_l_english.yml", lines);

            // Assert
            Assert.Single(result.Entries);
        }

        [Fact]
        public void Parse_DeveLerMultiplasEntradasValidas()
        {
            // Arrange
            var parser = new ParadoxLocalizationParser();

            var lines = new[]
            {
            "l_english:",
            "king:0 \"King\"",
            "queen:0 \"Queen\""
            };

            // Act
            var result = parser.Parse("titles_l_english.yml", lines);

            // Assert
            Assert.Equal(2, result.Entries.Count);

            Assert.Equal("king", result.Entries[0].Key);
            Assert.Equal("King", result.Entries[0].Value);

            Assert.Equal("queen", result.Entries[1].Key);
            Assert.Equal("Queen", result.Entries[1].Value);
        }
    }
}
