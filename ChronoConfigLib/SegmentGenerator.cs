namespace ChronoConfigLib
{
    public class SegmentGenerator
    {
        public SegmentResult Create(Configuration config)
        {
            var fps = Convert.ToInt32(config.Fps);
            var promptLengthInSeconds = Convert.ToInt32(config.PromptInterval);
            var cadence = Convert.ToInt32(config.Cadence);

            var settings = new Settings
            {
                Bpm = Convert.ToInt32(config.Bpm),
                Fps = fps,
                PromptLengthInSeconds = promptLengthInSeconds,
                Strength = new StrengthSettings
                {
                    Constant = .65,
                    Low = .25,
                    High = .7
                },
                TranslationX = 0,
                TranslationY = 0,
                TranslationZ = .5,
                RotationX = 0.04,
                RotationY = 0.06,
                RotationZ = -0.03
            };

            var totalSeconds = (int)TimeSpan.Parse(config.VideoLength).TotalSeconds;
            var totalFrames = (totalSeconds * fps) + 2;
            var segments = GenerateSegments(totalSeconds, promptLengthInSeconds, fps);

            return new SegmentResult
            {
                TotalFrames = totalFrames,
                Segments = segments
            };
        }

        private IEnumerable<Segment> GenerateSegments(int totalSeconds, int segmentLengthSeconds, int fps)
        {
            var segments = new List<Segment>();
            var totalFrames = totalSeconds * fps;
            var segmentFrames = segmentLengthSeconds * fps;

            for (int frameIndex = 0; frameIndex < totalFrames; frameIndex += segmentFrames)
            {
                segments.Add(new Segment
                {
                    Time = FrameToTime(frameIndex, fps).ToString(),
                    FrameIndex = frameIndex.ToString(),
                    Prompt = "",
                    Strength = "",
                    Zoom = "",
                    X = "",
                    Y = "",
                    Z = ""
                });
            }

            return segments;
        }

        private TimeSpan FrameToTime(int frameIndex, int fps)
        {
            return new TimeSpan(0, 0, frameIndex / fps);
        }

        private static FrameSetting NegateFrameValue(int frameStart, List<FrameSetting> frames, Settings settings)
        {
            var previousFrame = frames.LastOrDefault(r => r.FrameNumber < frameStart);
            return new FrameSetting
            {
                FrameNumber = frameStart,
                FrameValue = previousFrame == null
                    ? settings.RotationX
                    : previousFrame.FrameValue * -1
            };
        }
    }

    public class SegmentResult
    {
        public int TotalFrames { get; set; }
        public IEnumerable<Segment> Segments { get; set; } = [];
    }
}
