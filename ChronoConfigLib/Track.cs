namespace ChronoConfigLib
{

    public class Mix
    {
        public string Bpm { get; set; } = string.Empty;
        public string Fps { get; set; } = string.Empty;
        public string Cadence { get; set; } = string.Empty;
        public string PromptInterval { get; set; } = string.Empty;
        public List<Track> Tracks { get; set ; } = [];
    }

    public class Track
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TrackSection> Sections { get; set; } = [];
        public bool IsLast { get; set; }
    }

    public class TrackSection
    {
        public int Number { get; set; }
        public string Comment { get; set; } = string.Empty;
        public TrackSectionType Type { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public bool IsLast { get; set; }
    }

    public enum TrackSectionType
    {
        NONE,
        START,
        INTRO,
        CHORUS,
        BREAKDOWN,
        BUILDUP,
        OUTRO,
        END
    }
}
