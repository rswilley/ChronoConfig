using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class MainViewModelTests
    {
        [Fact]
        public void Validate_HasNoBpm_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration 
            { 
                Bpm = "", 
                Cadence = "4", 
                Fps = "15", 
                PromptInterval = "30", 
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Bpm is required").Value);
        }

        [Fact]
        public void Validate_BpmIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "abc",
                Cadence = "4",
                Fps = "15",
                PromptInterval = "30",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Bpm must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoFps_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "",
                PromptInterval = "30",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Fps is required").Value);
        }

        [Fact]
        public void Validate_FpsIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "abc",
                PromptInterval = "30",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Fps must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoCadence_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "0",
                Fps = "15",
                PromptInterval = "30",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Cadence is required").Value);
        }

        [Fact]
        public void Validate_CadenceIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "abc",
                Fps = "15",
                PromptInterval = "30",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Cadence must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoPromptInterval_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "15",
                PromptInterval = "",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "PromptInterval is required").Value);
        }

        [Fact]
        public void Validate_PromptIntervalIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "15",
                PromptInterval = "abc",
                VideoLength = "00:01:00"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "PromptInterval must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoVideoLength_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "15",
                PromptInterval = "30",
                VideoLength = ""
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "VideoLength is required").Value);
        }

        [Fact]
        public void Validate_VideoLengthIsNotATimespan_ReturnsError()
        {
            var subject = GetSubject();
            subject.Configuration = new Configuration
            {
                Bpm = "128",
                Cadence = "4",
                Fps = "15",
                PromptInterval = "30",
                VideoLength = "abc"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "VideoLength must be a timespan").Value);
        }

        private static MainViewModel GetSubject() => new();
    }
}
