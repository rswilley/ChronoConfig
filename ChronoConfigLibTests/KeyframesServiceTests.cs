using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class KeyframesServiceTests
    {
        [Fact]
        public void GetKeyframes_ByDefault_ReturnsCorrectPrompts()
        {
            var subject = GetSubject();
            var results = subject.GetKeyframes(new Configuration
            {
                Bpm = "128",
                Fps = "15",
                Cadence = "4",
                PromptInterval = "15",
                VideoLength = "00:01:00"
            });

            Assert.True(results.Count() == 4);
        }

        [Fact]
        public void GetSchedules_StrengthUnchanged_ReturnsStrength()
        {
            var subject = GetSubject();
            var result = subject.GetSchedules(
            [
                new()
                {
                    FrameIndex = "0",
                    Strength = "0.65",
                },
                new()
                {
                    FrameIndex = "225",
                    Strength = "",
                }
            ]);

            Assert.True(result.Strength.All(s => s.Value == "0.65"));
        }

        [Fact]
        public void GetSchedules_ZoomUnchanged_ReturnsZoom()
        {
            var subject = GetSubject();
            var result = subject.GetSchedules(
            [
                new()
                {
                    FrameIndex = "0",
                    Zoom = "0.5",
                },
                new()
                {
                    FrameIndex = "225",
                    Zoom = "",
                }
            ]);

            Assert.True(result.Zoom.All(s => s.Value == "0.5"));
        }

        [Fact]
        public void GetSchedules_StrengthChanged_ReturnsStrength()
        {
            var subject = GetSubject();
            var result = subject.GetSchedules(
            [
                new()
                {
                    FrameIndex = "0",
                    Strength = "0.65",
                },
                new()
                {
                    FrameIndex = "225",
                    Strength = "0.55",
                }
            ]);

            Assert.Equal("0.65", result.Strength.ElementAt(0).Value);
            Assert.Equal("0.65", result.Strength.ElementAt(1).Value);
            Assert.Equal("0.55", result.Strength.ElementAt(2).Value);
            Assert.Equal("0.65", result.Strength.ElementAt(3).Value);
        }

        [Fact]
        public void GetSchedules_ZoomChanged_ReturnsZoom()
        {
            var subject = GetSubject();
            var result = subject.GetSchedules(
            [
                new()
                {
                    FrameIndex = "0",
                    Zoom = "0.5",
                },
                new()
                {
                    FrameIndex = "225",
                    Zoom = "-0.5",
                }
            ]);

            Assert.Equal("0.5", result.Zoom.ElementAt(0).Value);
            Assert.Equal("-0.5", result.Zoom.ElementAt(1).Value);
        }

        private KeyframesService GetSubject() => new();
    }
}