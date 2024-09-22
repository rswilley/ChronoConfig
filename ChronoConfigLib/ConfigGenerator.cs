using System.Text;

namespace ChronoConfigLib
{
    public class ConfigGenerator
    {
        public ConfigResult Create(Mix mix)
        {
            var startingSection = mix.Tracks.Select(t => t.Sections.Single(s => s.Type == TrackSectionType.START)).Single();
            var endingSection = mix.Tracks.Select(t => t.Sections.Single(s => s.Type == TrackSectionType.END)).Single();
            
            int totalSeconds = 0;
            if (endingSection != null)
            {
                totalSeconds = (int)(TimeSpan.Parse(endingSection.StartTime) - TimeSpan.Parse(startingSection.StartTime)).TotalSeconds;
            }

            var fps = Convert.ToInt32(mix.Fps);
            var promptLengthInSeconds = Convert.ToInt32(mix.PromptInterval);
            var cadence = Convert.ToInt32(mix.Cadence);

            var prompts = GeneratePrompts(mix.Tracks, fps, promptLengthInSeconds);
            var totalFrames = (totalSeconds * fps) + (cadence / 2);

            return new ConfigResult
            {
                TotalFrames = totalFrames,
                Prompts = prompts
            };
        }

        private Dictionary<string, string> GeneratePrompts(List<Track> tracks, int fps, int promptLengthInSeconds)
        {
            var prompts = new Dictionary<string, string>();
            var startTimeOffset = TimeSpan.Zero;

            for (int trackIndex = 0;  trackIndex < tracks.Count; trackIndex++)
            {
                var trackNumber = trackIndex + 1;

                for (int sectionIndex = 0; sectionIndex < tracks[trackIndex].Sections.Count - 1; sectionIndex++)
                {
                    var sectionNumber = sectionIndex + 1;

                    var currentSection = tracks[trackIndex].Sections[sectionIndex];
                    var currentSectionStartTime = TimeSpan.Parse(currentSection.StartTime);

                    if (trackNumber == 1 && sectionNumber == 1 && currentSectionStartTime != TimeSpan.Zero)
                    {
                        startTimeOffset = currentSectionStartTime;
                        currentSectionStartTime = TimeSpan.Zero;
                    } else
                    {
                        currentSectionStartTime -= startTimeOffset;
                    }

                    var currentSectionFrame = GetFrameFromTime(currentSectionStartTime, fps);

                    prompts.Add(currentSectionFrame.ToString(), GetPromptText(currentSection, false));

                    if (currentSectionStartTime.TotalSeconds > promptLengthInSeconds)
                    {
                        var remainingSeconds = currentSectionStartTime.TotalSeconds - promptLengthInSeconds;
                        var frameIteration = currentSectionFrame;
                        while (remainingSeconds > 0)
                        {
                            var subFrameIndex = frameIteration +
                                                GetFrameFromTime(new TimeSpan(0, 0, promptLengthInSeconds), fps);
                            prompts.Add(subFrameIndex.ToString(), GetPromptText(currentSection, true));
                            remainingSeconds -= promptLengthInSeconds;
                            frameIteration = subFrameIndex;
                        }
                    }
                }
            }

            return prompts;
        }

        private static int GetFrameFromTime(TimeSpan currentSectionStartTime, int fps)
        {
            var frame = (int)Math.Floor(currentSectionStartTime.TotalSeconds * fps);
            return frame;
        }

        private static string GetPromptText(TrackSection section, bool isSubPrompt)
        {
            var sb = new StringBuilder();
            sb.Append(section.Type.ToString());

            if (isSubPrompt)
            {
                sb.Append(" - " + "sub prompt");
            }

            if (!string.IsNullOrEmpty(section.Comment))
            {
                sb.Append($" [{section.Comment}]");
            }

            return sb.ToString();
        }
    }

    public class ConfigResult
    {
        public int TotalFrames { get; set; }
        public Dictionary<string, string> Prompts { get; set; } = [];
    }
}
