
namespace GameLocalizationToolkit.Core.Models
{
    public sealed class LocalizationEntryDifference
    {
        public required LocalizationEntry Source { get; init; }

        public required LocalizationEntry Target { get; init; }

        public bool HasValueChanged =>
         Source.Value != Target.Value;

        public bool HasVersionChanged =>
            Source.Version != Target.Version;

        public bool HasChanges =>
            HasValueChanged || HasVersionChanged;
    }
}
