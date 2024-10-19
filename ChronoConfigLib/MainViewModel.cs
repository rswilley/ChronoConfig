using ChronoConfigLib.Extensions;
using System.Text.Json;

namespace ChronoConfigLib
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            var initialtrack = CreateEmptyTrack(1);
            initialtrack.IsLast = true;
            initialtrack.Sections[0].IsLast = true;

            Mix.Tracks = [initialtrack];
        }

        public Mix Mix { get; set; } = new();
        public ViewModes ViewMode { get; set; } = ViewModes.Step1;
        public string TotalFrames { get; set; } = string.Empty;
        public List<Prompt> Prompts { get; set; } = [];
        public MovementSchedule? MovementSchedule { get; set; }
        public Step3Model Step3Model { get; set; } = new();

        public void AddTrack()
        {
            Mix.Tracks.Add(CreateEmptyTrack(Mix.Tracks.Count + 1));
            SetLastTrack();
        }

        public void DeleteTrack(Track deletedTrack)
        {
            Mix.Tracks.Remove(deletedTrack);
            SetLastTrack();
        }

        public void AddSection(Track currentTrack)
        {
            var currentSection = currentTrack.Sections[currentTrack.Sections.Count - 1];
            currentTrack.Sections.Add(CreateEmptySection(currentTrack.Sections.Count + 1, currentSection.StartTime));
            SetLastSection(currentTrack);
        }

        public void DeleteSection(Track currentTrack, TrackSection deletedSection)
        {
            var trackIndex = Mix.Tracks.FindIndex(t => t.Number == currentTrack.Number);
            var track = Mix.Tracks[trackIndex];

            track.Sections.Remove(deletedSection);
            SetLastSection(track);
        }

        public void UpdateSectionTimes(Track currentTrack, TrackSection currentSection, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) 
                return;

            if (!TimeSpan.TryParse(newValue, out TimeSpan changedTime))
            {
                return;
            }

            double millisecondsToChange = 0;
            foreach (var track in Mix.Tracks)
            {
                foreach (var section in track.Sections)
                {
                    var currentSectionStartTime = TimeSpan.Parse(section.StartTime);

                    if (track.Number == currentTrack.Number && section.Number == currentSection.Number)
                    {
                        if (changedTime != currentSectionStartTime)
                        {
                            millisecondsToChange = (changedTime - currentSectionStartTime).TotalMilliseconds;
                        }
                    }

                    if (millisecondsToChange != 0)
                    {
                        section.StartTime = currentSectionStartTime.Add(new TimeSpan(0, 0, 0, 0, (int)millisecondsToChange)).ToString();
                    }
                }
            }
        }

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            ValidateIsNumber(nameof(Mix.Bpm), Mix.Bpm, errors);
            ValidateIsNumber(nameof(Mix.Fps), Mix.Fps, errors);
            ValidateIsNumber(nameof(Mix.Cadence), Mix.Cadence, errors);
            ValidateIsNumber(nameof(Mix.PromptInterval), Mix.PromptInterval, errors);

            if (Mix.Tracks?.Count == 0)
            {
                errors.Add("Tracks", "At least one track is required");
            }
            else if (Mix.Tracks?.Any(t => t.Sections?.Count == 0) == true)
            {
                errors.Add("Sections", "At least one section is required");
            }

            var hasSectionTypeUnset = Mix.Tracks?.Any(t => t.Sections.Any(s => s.Type == TrackSectionType.NONE)) == true;
            if (hasSectionTypeUnset)
            {
                errors.Add("SectionType", "All sections must have their Type set");
            }

            var startingSections = Mix.Tracks?.SelectMany(t => t.Sections.Where(s => s.Type == TrackSectionType.START));
            if (startingSections?.Count() == 0)
            {
                errors.Add("SectionStart", "Must have one starting section");
            }
            else if (startingSections?.Count() > 1)
            {
                errors.Add("SectionStart", "Must have only one starting section");
            }

            var endingSections = Mix.Tracks?.SelectMany(t => t.Sections.Where(s => s.Type == TrackSectionType.END));
            if (endingSections?.Count() == 0)
            {
                errors.Add("SectionEnd", "Must have one ending section");
            }
            else if (endingSections?.Count() > 1)
            {
                errors.Add("SectionEnd", "Must have only one ending section");
            }

            var previousMs = 0d;
            var startTime = new TimeSpan();
            foreach (var track in Mix.Tracks!)
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

        public void SetPromptJson(Dictionary<string, string> prompts)
        {
            Step3Model.TotalFrames = TotalFrames;
            Step3Model.PromptsJson = JsonSerializer.Serialize(prompts, options: new JsonSerializerOptions
            {
                WriteIndented = true
            });

            if (MovementSchedule != null)
            {
                Step3Model.StrengthSchedule = MovementSchedule.StrengthSchedule.ToSchedule();
                Step3Model.TranslationZ = MovementSchedule.TranslationZ.ToSchedule();
                Step3Model.Rotation3DX = MovementSchedule.Rotation3DX.ToSchedule();
                Step3Model.Rotation3DY = MovementSchedule.Rotation3DY.ToSchedule();
                Step3Model.Rotation3DZ = MovementSchedule.Rotation3DZ.ToSchedule();
            }
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

        private void SetLastTrack()
        {
            for (var i = 0; i < Mix.Tracks.Count; i++)
            {
                if (i == Mix.Tracks.Count - 1)
                {
                    Mix.Tracks[i].IsLast = true;
                }
                else
                {
                    Mix.Tracks[i].IsLast = false;
                }
            }
        }

        private static void SetLastSection(Track currentTrack)
        {
            for (var i = 0; i < currentTrack.Sections.Count; i++)
            {
                if (i == currentTrack.Sections.Count - 1)
                {
                    currentTrack.Sections[i].IsLast = true;
                }
                else
                {
                    currentTrack.Sections[i].IsLast = false;
                }
            }
        }

        private static Track CreateEmptyTrack(int number)
        {
            return new Track
            {
                Number = number,
                Name = "",
                Sections =
            [
                CreateEmptySection(1, "00:00:00")
            ]
            };
        }

        private static TrackSection CreateEmptySection(int number, string startTime)
        {
            return new TrackSection
            {
                Number = number,
                Comment = "",
                StartTime = startTime,
                Type = TrackSectionType.START
            };
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
