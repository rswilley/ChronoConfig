using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChronoConfigLib
{
    public class ConfigGenerator
    {
        public ConfigResult Create(Mix mix)
        {
            var errors = Validate(mix);
            if (errors.Count != 0)
            {
                return new ConfigResult
                {
                    IsValid = false,
                    ErrorMessages = errors
                };
            }

            return new ConfigResult();
        }

        private static Dictionary<string, string> Validate(Mix mix)
        {
            var errors = new Dictionary<string, string>();
            
            ValidateIsNumber(nameof(mix.Bpm), mix.Bpm, errors);
            ValidateIsNumber(nameof(mix.Fps), mix.Fps, errors);
            ValidateIsNumber(nameof(mix.Cadence), mix.Cadence, errors);
            ValidateIsNumber(nameof(mix.PromptInterval), mix.PromptInterval, errors);

            if (mix.Tracks?.Count == 0)
            {
                errors.Add("Tracks", "At least one track is required");
            } else if (mix.Tracks?.Any(t => t.Sections?.Count == 0) == true)
            {
                errors.Add("Sections", "At least one section is required");
            }

            var hasSectionTypeUnset = mix.Tracks?.Any(t => t.Sections.Any(s => s.Type == TrackSectionType.NONE)) == true;
            if (hasSectionTypeUnset)
            {
                errors.Add("SectionType", "All sections must have their Type set");
            }

            var hasNoEndingSection = mix.Tracks?.Any(t => t.Sections.Any(s => s.Type == TrackSectionType.END)) == false;
            if (hasNoEndingSection)
            {
                errors.Add("EndingSection", "Must have an ending section");
            }

            var previousMs = 0d;
            var startTime = new TimeSpan();
            foreach (var track in mix.Tracks!)
            {
                foreach (var section in track.Sections)
                {
                    if (!TimeSpan.TryParse(section.StartTime, out startTime))
                    {
                        errors.Add("StartTime", "Invalid Start Time");
                        break;
                    }
                    else if (startTime.TotalMilliseconds < previousMs)
                    {
                        errors.Add("StartTime", "Start Time is less than previous");
                        break;
                    }

                    previousMs = startTime.TotalMilliseconds;
                }
            }

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
    }

    public class ConfigResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, string> ErrorMessages { get; set; } = [];
    }
}
