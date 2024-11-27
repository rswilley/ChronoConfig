namespace ChronoConfigLib
{
    public static class KeyframesGenerator
    {
        public static int GetTotalFrames(Configuration config)
        {
            var totalSeconds = (int)TimeSpan.Parse(config.VideoLength).TotalSeconds;
            var totalFrames = totalSeconds * Convert.ToInt32(config.Fps);
            return totalFrames + 2;
        }

        public static IEnumerable<Keyframe> GetKeyframes(Configuration config)
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

        private static TimeSpan FrameToTime(int frameIndex, int fps)
        {
            return new TimeSpan(0, 0, frameIndex / fps);
        }
    }
}
