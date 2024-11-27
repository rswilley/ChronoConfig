using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class SegmentGeneratorTests
    {
        [Fact]
        public void Create_ByDefault_ReturnsCorrectPrompts()
        {
            var subject = GetSubject();
            var result = subject.Create(new Configuration
            {
                Bpm = "128",
                Fps = "15",
                Cadence = "4",
                PromptInterval = "15",
                VideoLength = "00:01:00"
            });

            Assert.True(result.Segments.Count() == 4);
        }

        private static SegmentGenerator GetSubject() => new();
    }
}