using System.Text.Json;

namespace ChronoConfigLib
{
    public class MainViewModel
    {
        public ViewModes ViewMode { get; set; } = ViewModes.Step1;
        public Configuration Configuration { get; set; } = new Configuration { 
            Bpm = "", 
            Cadence = "", 
            Fps = "", 
            PromptInterval = "", 
            VideoLength = "00:00:00"
        };
        public string TotalFrames { get; set; } = string.Empty;
        public List<Segment> Prompts { get; set; } = [];
        public Step3Model Step3Model { get; set; } = new();

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            ValidateIsNumber(nameof(Configuration.Bpm), Configuration.Bpm, errors);
            ValidateIsNumber(nameof(Configuration.Fps), Configuration.Fps, errors);
            ValidateIsNumber(nameof(Configuration.Cadence), Configuration.Cadence, errors);
            ValidateIsNumber(nameof(Configuration.PromptInterval), Configuration.PromptInterval, errors);
            ValidateIsTimeSpan(nameof(Configuration.VideoLength), Configuration.VideoLength, errors);

            return errors;
        }

        public void SetPromptJson(Dictionary<string, string> prompts)
        {
            Step3Model.TotalFrames = TotalFrames;
            Step3Model.PromptsJson = JsonSerializer.Serialize(prompts, options: new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private static void ValidateIsNumber(string errorKey, string value, Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add(errorKey, $"{errorKey} is required");
            }
            else if (!int.TryParse(value, out _))
            {
                errors.Add(errorKey, $"{errorKey} must be a number");
            }
        }

        private static void ValidateIsTimeSpan(string errorKey, string value, Dictionary<string, string> errors)
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add(errorKey, $"{errorKey} is required");
            }
            else if (!TimeSpan.TryParse(value, out _))
            {
                errors.Add(errorKey, $"{errorKey} must be a timespan");
            }
        }
    }

    public class Step3Model
    {
        public string TotalFrames { get; set; } = string.Empty;
        public string PromptsJson { get; set; } = string.Empty;
        public string StrengthSchedule { get; set; } = string.Empty;
        public string TranslationZ { get; set; } = string.Empty;
        public string Rotation3DX { get; set; } = string.Empty;
        public string Rotation3DY { get; set; } = string.Empty;
        public string Rotation3DZ { get; set; } = string.Empty;
    }

    public enum ViewModes
    {
        Step1,
        Step2,
        Step3
    }
}
