namespace ChronoConfigLib
{
    public class Track
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public TrackSectionType StartType { get; set; }
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
        INTRO,
        CHORUS,
        BREAKDOWN,
        BUILDUP,
        OUTRO
    }
}
