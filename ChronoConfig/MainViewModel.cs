using ChronoConfigLib;
using ChronoConfigLib.Extensions;
using Microsoft.Maui.Controls;
using System.Text.Json;

namespace ChronoConfig;

public interface IMainViewModel
{
    ViewModes ViewMode { get; set; }
    Configuration Configuration { get; }

    Dictionary<string, string> Validate();
    void SavePreferences();
    List<Keyframe> LoadKeyframes();
    Output LoadOutput(List<Keyframe> keyframes);
}

public class MainViewModel : IMainViewModel
{
    private readonly IPlatformAdapter _platformAdapter;
    private readonly IKeyframesService _keyframesService;
    private readonly Configuration _configuration;

    public MainViewModel(
        IPlatformAdapter platformAdapter,
        IKeyframesService keyframesService)
    {
        _platformAdapter = platformAdapter;
        _keyframesService = keyframesService;

        _configuration = new Configuration
        {
            Bpm = _platformAdapter.GetPreference("bpm"),
            Fps = _platformAdapter.GetPreference("fps"),
            Cadence = _platformAdapter.GetPreference("cadence"),
            PromptInterval = _platformAdapter.GetPreference("prompt-interval"),
            VideoLength = _platformAdapter.GetPreference("video-length")
        };
    }

    public ViewModes ViewMode { get; set; } = ViewModes.Step1;
    public Configuration Configuration => _configuration;

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

    public void SavePreferences()
    {
        _platformAdapter.SetPreference("bpm", _configuration.Bpm);
        _platformAdapter.SetPreference("fps", _configuration.Fps);
        _platformAdapter.SetPreference("cadence", _configuration.Cadence);
        _platformAdapter.SetPreference("prompt-interval", _configuration.PromptInterval);
        _platformAdapter.SetPreference("video-length", _configuration.VideoLength);
    }

    public List<Keyframe> LoadKeyframes()
    {
        return _keyframesService.GetKeyframes(_configuration).ToList();
    }

    public Output LoadOutput(List<Keyframe> keyframes)
    {
        var result = _keyframesService.GetSchedules(keyframes);
        var output = new Output
        {
            TotalFrames = _keyframesService.GetTotalFrames(_configuration).ToString(),
            PromptsJson = JsonSerializer.Serialize(result.Prompts, options: new JsonSerializerOptions
            {
                WriteIndented = true
            }),
            StrengthSchedule = result.Strength.Select(s => new FrameSetting
            {
                FrameNumber = s.Key,
                FrameValue = string.IsNullOrEmpty(s.Value) ? 0 : double.Parse(s.Value)
            }).ToSchedule(),
            TranslationZ = result.Zoom.Select(s => new FrameSetting
            {
                FrameNumber = s.Key,
                FrameValue = string.IsNullOrEmpty(s.Value) ? 0 : double.Parse(s.Value)
            }).ToSchedule(),
            Rotation3DX = "",
            Rotation3DY = "",
            Rotation3DZ = ""
        };

        return output;
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

public class Output
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
