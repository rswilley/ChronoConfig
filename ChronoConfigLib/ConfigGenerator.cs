using System.Text;

namespace ChronoConfigLib
{
    public class ConfigGenerator
    {
        public ConfigResult Create(Mix mix)
        {
            var startingSection = mix.Tracks.Select(t => t.Sections.Single(s => s.Type == TrackSectionType.START)).Single();
            var endingSection = mix.Tracks.Select(t => t.Sections.Single(s => s.Type == TrackSectionType.END)).Single();
            var sectionCount = mix.Tracks.SelectMany(t => t.Sections).Count();
            
            int totalSeconds = 0;
            if (endingSection != null)
            {
                totalSeconds = (int)(TimeSpan.Parse(endingSection.StartTime) - TimeSpan.Parse(startingSection.StartTime)).TotalSeconds;
            }

            var fps = Convert.ToInt32(mix.Fps);
            var promptLengthInSeconds = Convert.ToInt32(mix.PromptInterval);
            var cadence = Convert.ToInt32(mix.Cadence);

            var settings = new Settings
            {
                Bpm = Convert.ToInt32(mix.Bpm),
                Fps = fps,
                PromptLengthInSeconds = promptLengthInSeconds,
                Strength = new StrengthSettings
                {
                    Constant = .55,
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

            var totalFrames = (totalSeconds * fps) + (cadence / 2);

            if (sectionCount == 2)
            {
                var prompts = GeneratePromptsByLength(startingSection, endingSection!, settings);
                return new ConfigResult
                {
                    TotalFrames = totalFrames,
                    Prompts = prompts
                };

            } else
            {
                var (prompts, movementSchedule) = GeneratePromptsBySection(mix.Tracks, settings);
                return new ConfigResult
                {
                    TotalFrames = totalFrames,
                    Prompts = prompts,
                    MovementSchedule = movementSchedule
                };
            }
        }

        private Dictionary<string, string> GeneratePromptsByLength(TrackSection starting, TrackSection ending, Settings settings)
        {
            var prompts = new Dictionary<string, string>();
            var startTime = TimeSpan.Parse(starting.StartTime);
            var endTime = TimeSpan.Parse(ending.StartTime);

            var totalLengthInSeconds = (int)(endTime - startTime).TotalSeconds;
            var sectionCount = totalLengthInSeconds / settings.PromptLengthInSeconds;

            for (int sectionIndex = 0; sectionIndex < sectionCount; sectionIndex++)
            {
                var frameIndex = sectionIndex * 30 * settings.Fps;
                prompts.Add(frameIndex.ToString(), "");
            }

            return prompts;
        }

        private (Dictionary<string, string> prompts, MovementSchedule movementSchedule) GeneratePromptsBySection(List<Track> tracks, Settings settings)
        {
            var prompts = new Dictionary<string, string>();
            var movementSchedule = new MovementSchedule();
            var startTimeOffset = TimeSpan.Zero;

            for (int trackIndex = 0;  trackIndex < tracks.Count; trackIndex++)
            {
                var trackNumber = trackIndex + 1;

                for (int sectionIndex = 0; sectionIndex < tracks[trackIndex].Sections.Count - 1; sectionIndex++)
                {
                    var sectionNumber = sectionIndex + 1;

                    var currentSection = tracks[trackIndex].Sections[sectionIndex];
                    var currentSectionStartTime = TimeSpan.Parse(currentSection.StartTime);
                    var currentSectionFrame = GetFrameFromTime(currentSectionStartTime, settings.Fps);

                    HandleSectionType(sectionIndex, tracks[trackIndex].Sections, movementSchedule, currentSectionFrame, settings);

                    if (trackNumber == 1 && sectionNumber == 1 && currentSectionStartTime != TimeSpan.Zero)
                    {
                        startTimeOffset = currentSectionStartTime;
                        currentSectionStartTime = TimeSpan.Zero;
                    } else
                    {
                        currentSectionStartTime -= startTimeOffset;
                    }

                    prompts.Add(currentSectionFrame.ToString(), GetPromptText(currentSection, false));
                }
            }

            return (prompts, movementSchedule);
        }

        private static void HandleSectionType(int currentSectionIndex, List<TrackSection> sections, MovementSchedule movementSchedule, int frameStart, Settings settings)
        {
            var currentSection = sections[currentSectionIndex];

            switch (currentSection.Type)
            {
                case TrackSectionType.START:
                case TrackSectionType.INTRO:
                case TrackSectionType.OUTRO:
                    movementSchedule.TranslationZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.TranslationZ
                    });
                    movementSchedule.Rotation3DX.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.RotationX
                    });
                    movementSchedule.Rotation3DY.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.RotationY
                    });
                    movementSchedule.Rotation3DZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.RotationZ
                    });
                    movementSchedule.StrengthSchedule.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.Strength.Constant
                    });
                    break;
                case TrackSectionType.BUILDUP:
                    movementSchedule.TranslationZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.TranslationZ
                    });
                    movementSchedule.Rotation3DX.AddRange(GetBuildupRotationX(currentSectionIndex, sections, frameStart, settings));
                    movementSchedule.Rotation3DY.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.RotationY
                    });
                    movementSchedule.Rotation3DZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.RotationZ
                    });
                    movementSchedule.StrengthSchedule.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.Strength.Constant
                    });
                    break;
                case TrackSectionType.BREAKDOWN:
                    movementSchedule.TranslationZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = 0
                    });
                    movementSchedule.Rotation3DX.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = 0
                    });
                    movementSchedule.Rotation3DY.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = 0
                    });
                    movementSchedule.Rotation3DZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = 0
                    });
                    movementSchedule.StrengthSchedule.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.Strength.Constant
                    });
                    break;
                case TrackSectionType.CHORUS:
                    movementSchedule.TranslationZ.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = 0,
                        UseFormula = true,
                        Formula = $"(-(2 * 1 / 3.141) * arctan((1 * 1 + 1) / tan(((t  + 0) * 3.141 * {settings.Bpm} / 60 / {settings.Fps}))) + 1.50)"
                    });
                    movementSchedule.Rotation3DX.Add(NegateFrameValue(frameStart, movementSchedule.Rotation3DX, settings));
                    movementSchedule.Rotation3DY.Add(NegateFrameValue(frameStart, movementSchedule.Rotation3DY, settings));
                    movementSchedule.Rotation3DZ.Add(NegateFrameValue(frameStart, movementSchedule.Rotation3DZ, settings));
                    movementSchedule.StrengthSchedule.Add(new FrameSetting
                    {
                        FrameNumber = frameStart,
                        FrameValue = settings.Strength.Constant
                    });
                    break;
            }
        }

        /// <summary>
        /// Calculate a sawtooth wave for buildup
        /// </summary>
        /// <param name="currentSection"></param>
        /// <param name="frameStart"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static IEnumerable<FrameSetting> GetBuildupRotationX(int currentSectionIndex, List<TrackSection> sections, int frameStart, Settings settings)
        {
            var currentSection = sections[currentSectionIndex];
            var nextSection = sections[currentSectionIndex + 1];

            var durationInSeconds = (TimeSpan.Parse(nextSection.StartTime) - TimeSpan.Parse(currentSection.StartTime)).TotalSeconds;
            var durationInFrames = (int)Math.Floor(durationInSeconds * settings.Fps);

            var results = new List<FrameSetting>();
            for (var f = 0; f < durationInFrames; f++)
            {
                var frameNumber = frameStart + f;
                var value = Sawtooth(f + 1, durationInFrames, settings.RotationX, 1);
                var roundedValue = Math.Round(value / 2, 2);

                results.Add(new FrameSetting
                {
                    FrameNumber = frameNumber,
                    FrameValue = roundedValue
                });
            }

            return results;

            static double Sawtooth(double t, double period, double min, double max)
            {
                return min + (max - min) * Fract(t / period);
            }

            static double Fract(double t)
            {
                return t % 1.0;
            }
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

    public class ConfigResult
    {
        public int TotalFrames { get; set; }
        public Dictionary<string, string> Prompts { get; set; } = [];
        public MovementSchedule? MovementSchedule { get; set; }
    }
}
