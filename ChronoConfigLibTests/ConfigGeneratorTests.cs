using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class ConfigGeneratorTests
    {
        [Fact]
        public void Create_ByDefault_ReturnsCorrectPrompts()
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
                                Type = TrackSectionType.START
                                },
                            new() {
                                StartTime = "00:00:30",
                                Type = TrackSectionType.BUILDUP
                                },
                                new() {
                                StartTime = "00:00:45",
                                Type = TrackSectionType.CHORUS
                                },
                                new() {
                                StartTime = "00:01:45",
                                Type = TrackSectionType.BREAKDOWN
                                },
                                new() {
                                StartTime = "00:02:45",
                                Type = TrackSectionType.BUILDUP
                                },
                                new() {
                                StartTime = "00:03:00",
                                Type = TrackSectionType.CHORUS
                                },
                                new() {
                                StartTime = "00:04:00",
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            });

            Assert.True(result.Prompts.Count == 6);
        }

        [Fact]
        public void Create_TrackStartIsNotZero_ReturnsCorrectPrompts()
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
                                StartTime = "00:02:00",
                                Comment = "Comment",
                                Type = TrackSectionType.START
                                },
                            new() {
                                StartTime = "00:02:30",
                                Type = TrackSectionType.CHORUS
                                },
                                new() {
                                StartTime = "00:03:00",
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            });

            Assert.True(result.Prompts.Count == 2);
            Assert.Equal("0", result.Prompts.ElementAt(0).Key);
            Assert.Equal("START [Comment]", result.Prompts.ElementAt(0).Value);
            Assert.Equal("450", result.Prompts.ElementAt(1).Key);
            Assert.Equal("CHORUS", result.Prompts.ElementAt(1).Value);
            Assert.Equal(902, result.TotalFrames);
        }

        [Fact]
        public void Create_OnlyStartAndEndSections_ReturnsPromptsByInterval()
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
                                Type = TrackSectionType.START
                                },
                                new() {
                                StartTime = "00:04:00",
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            });

            Assert.True(result.Prompts.Count == 8);
        }

        private static ConfigGenerator GetSubject() => new();
    }
}