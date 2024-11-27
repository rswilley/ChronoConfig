namespace ChronoConfigLib
{
    public class Configuration
    {
        public required string Bpm { get; set; }
        public required string Fps { get; set; }
        public required string Cadence { get; set; }
        public required string PromptInterval { get; set; }
        public required string VideoLength { get; set; }
    }
}
