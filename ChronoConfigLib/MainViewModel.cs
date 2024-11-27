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
        public Step2Model? Step2Model { get; set; }
        public Step3Model? Step3Model { get; set; }

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

    public class Step2Model
    {
        public Step2Model(Configuration config)
        {
            Keyframes = KeyframesGenerator.GetKeyframes(config).ToList();
        }

        public List<Keyframe> Keyframes { get; set; } = [];
    }

    public class Step3Model
    {
        public Step3Model(Configuration config, Dictionary<string, string> prompts)
        {
            TotalFrames = KeyframesGenerator.GetTotalFrames(config).ToString();
            PromptsJson = JsonSerializer.Serialize(prompts, options: new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

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
