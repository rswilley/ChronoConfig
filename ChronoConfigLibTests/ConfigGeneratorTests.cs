using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class ConfigGeneratorTests
    {
        [Fact]
        public void Create_TrackSectionWithTimeLessThanPromptLengthInSeconds_ReturnsCorrectPromptsWithSubPrompt()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Bpm = "128",
                Fps = "15",
                Cadence = "4",
                PromptInterval = "30",
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                StartTime = "00:00:00", 
                                Comment = "Comment",
                                Type = TrackSectionType.START
                                },
                            new() {
                                StartTime = "00:00:30",
                                Type = TrackSectionType.INTRO
                                },
                                new() {
                                StartTime = "00:00:45",
                                Type = TrackSectionType.CHORUS
                                },
                                new() {
                                StartTime = "00:01:00",
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            });

            Assert.True(result.Prompts.Count == 4);
            Assert.Equal("0", result.Prompts.ElementAt(0).Key);
            Assert.Equal("START [Comment]", result.Prompts.ElementAt(0).Value);
            Assert.Equal("450", result.Prompts.ElementAt(1).Key);
            Assert.Equal("INTRO", result.Prompts.ElementAt(1).Value);
            Assert.Equal("675", result.Prompts.ElementAt(2).Key);
            Assert.Equal("CHORUS", result.Prompts.ElementAt(2).Value);
            Assert.Equal("1125", result.Prompts.ElementAt(3).Key);
            Assert.Equal("CHORUS - sub prompt", result.Prompts.ElementAt(3).Value);
            Assert.Equal(902, result.TotalFrames);
        }

        private static ConfigGenerator GetSubject() => new();
    }
}