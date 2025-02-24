namespace ChronoConfigLib;

public interface IKeyframesService
{
    int GetTotalFrames(Configuration config);
    IEnumerable<Keyframe> GetKeyframes(Configuration config);
    KeyframeSchedule GetSchedules(IEnumerable<Keyframe> keyframes);
}

public class KeyframesService : IKeyframesService
{
    public int GetTotalFrames(Configuration config)
    {
        var totalSeconds = (int)TimeSpan.Parse(config.VideoLength).TotalSeconds;
        var totalFrames = totalSeconds * Convert.ToInt32(config.Fps);
        return totalFrames + 2;
    }

    public IEnumerable<Keyframe> GetKeyframes(Configuration config)
    {
        var fps = Convert.ToInt32(config.Fps);
        var totalFrames = GetTotalFrames(config);
        var segmentFrames = Convert.ToInt32(config.PromptInterval) * fps;

        var keyframes = new List<Keyframe>();
        for (int frameIndex = 0; frameIndex < totalFrames; frameIndex += segmentFrames)
        {
            keyframes.Add(new Keyframe
            {
                Time = FrameToTime(frameIndex, fps).ToString(),
                FrameIndex = frameIndex.ToString(),
                Strength = "",
                Zoom = "",
                X = "",
                Y = "",
                Z = "",
                Prompt = ""
            });
        }

        return keyframes;
    }

    private TimeSpan FrameToTime(int frameIndex, int fps)
    {
        return new TimeSpan(0, 0, frameIndex / fps);
    }

    public KeyframeSchedule GetSchedules(IEnumerable<Keyframe> keyframes)
    {
        var prompts = new Dictionary<string, string>();
        var strengthSchedule = new Dictionary<int, string>();
        var zoomSchedule = new Dictionary<int, string>();

        var strength = string.Empty;
        var zoom = string.Empty;

        for (var i = 0; i < keyframes.Count(); i++)
        {
            var keyframe = keyframes.ElementAt(i);
            var frameIndex = Convert.ToInt32(keyframe.FrameIndex);

            bool strengthChanged = false;
            bool zoomChanged = false;

            if (i == 0)
            {
                strength = keyframe.Strength;
                zoom = keyframe.Zoom;
            }
            else
            {
                strengthChanged = !strength.Equals(keyframe.Strength) && !string.IsNullOrEmpty(keyframe.Strength);
                zoomChanged = !zoom.Equals(keyframe.Zoom) && !string.IsNullOrEmpty(keyframe.Zoom);
            }

            if (strengthChanged)
            {
                var newStrength = keyframe.Strength;

                strengthSchedule.Add(frameIndex - 1, strength);
                strengthSchedule.Add(frameIndex, newStrength);
                strengthSchedule.Add(frameIndex + 1, strength);
            } else
            {
                strengthSchedule.Add(frameIndex, strength);
            }

            if (zoomChanged)
            {
                var newZoom = keyframe.Zoom;
                zoomSchedule.Add(frameIndex, newZoom);

                zoom = newZoom;
            } else
            {
                zoomSchedule.Add(frameIndex, zoom);
            }

            prompts.Add(frameIndex.ToString(), keyframe.Prompt);
        }

        return new KeyframeSchedule
        {
            Prompts = prompts,
            Strength = strengthSchedule,
            Zoom = zoomSchedule
        };
    }
}

public class KeyframeSchedule
{
    public Dictionary<string, string> Prompts { get; init; } = [];
    public Dictionary<int, string> Strength { get; init; } = [];
    public Dictionary<int, string> Zoom { get; init; } = [];
}