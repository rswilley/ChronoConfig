namespace ChronoConfigLib
{
    public class Settings
    {
        public int Bpm { get; init; }
        public int Fps { get; init; }
        public int PromptLengthInSeconds { get; init; }
        public StrengthSettings Strength { get; init; }
        public double TranslationX { get; init; }
        public double TranslationY { get; init; }
        public double TranslationZ { get; init; }
        public double RotationX { get; init; }
        public double RotationY { get; init; }
        public double RotationZ { get; init; }
    }

    public class StrengthSettings
    {
        public double High { get; init; }
        public double Low { get; init; }
        public double Constant { get; init; }
    }
}
