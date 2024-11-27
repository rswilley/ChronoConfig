using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class KeyframesGeneratorTests
    {
        [Fact]
        public void Create_ByDefault_ReturnsCorrectPrompts()
        {
            //var subject = GetSubject();
            var results = KeyframesGenerator.GetKeyframes(new Configuration
            {
                Bpm = "128",
                Fps = "15",
                Cadence = "4",
                PromptInterval = "15",
                VideoLength = "00:01:00"
            });

            Assert.True(results.Count() == 4);
        }

        //private static KeyframesGenerator GetSubject() => new();
    }
}