using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class ConfigGeneratorTests
    {
        [Fact]
        public void Create_HasNoBpm_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix());

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Bpm is required").Value);
        }

        [Fact]
        public void Create_BpmIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Bpm = "abc"
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Bpm must be a number").Value);
        }

        [Fact]
        public void Create_HasNoFps_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix());

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Fps is required").Value);
        }

        [Fact]
        public void Create_FpsIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Fps = "abc"
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Fps must be a number").Value);
        }

        [Fact]
        public void Create_HasNoCadence_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix());

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Cadence is required").Value);
        }

        [Fact]
        public void Create_CadenceIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Cadence = "abc"
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Cadence must be a number").Value);
        }

        [Fact]
        public void Create_HasNoPromptInterval_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix());

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "PromptInterval is required").Value);
        }

        [Fact]
        public void Create_PromptIntervalIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                PromptInterval = "abc"
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "PromptInterval must be a number").Value);
        }

        [Fact]
        public void Create_NoTracks_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix());

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "At least one track is required").Value);
        }

        [Fact]
        public void Create_SectionWithTypeNotSet_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                Type = TrackSectionType.NONE
                                }
                            ]
                        }
                ]
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "All sections must have their Type set").Value);
        }

        [Fact]
        public void Create_TrackWithNoSections_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Tracks =
                [
                    new()
                ]
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "At least one section is required").Value);
        }

        [Fact]
        public void Create_NoEndingSection_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Tracks =
                [
                    new()
                ]
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Must have an ending section").Value);
        }

        [Fact]
        public void Create_SectionHasInvalidStartTime_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                StartTime = "abc"
                                }
                            ]
                        }
                ]
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Invalid Start Time").Value);
        }

        [Fact]
        public void Create_HasSectionStartTimeLessThanPrevious_ReturnsError()
        {
            var subject = GetSubject();
            var result = subject.Create(new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                StartTime = "00:00:45"
                                },
                                new() {
                                StartTime = "00:00:30"
                                }
                            ]
                        }
                ]
            });

            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessages.FirstOrDefault(e => e.Value == "Start Time is less than previous").Value);
        }

        private static ConfigGenerator GetSubject() => new();
    }
}