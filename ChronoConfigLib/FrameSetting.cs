namespace ChronoConfigLib
{
    public class FrameSetting
    {
        public int FrameNumber { get; init; }
        public double FrameValue { get; init; }
        public bool UseFormula { get; init; }
        public string Formula { get; init; } = string.Empty;
    }

    public class MovementSchedule
    {
        public List<FrameSetting> TranslationZ = [];
        public List<FrameSetting> Rotation3DX = [];
        public List<FrameSetting> Rotation3DY = [];
        public List<FrameSetting> Rotation3DZ = [];
        public List<FrameSetting> StrengthSchedule = [];
    }
}
