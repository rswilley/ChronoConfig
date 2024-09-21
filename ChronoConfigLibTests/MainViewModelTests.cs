using ChronoConfigLib;

namespace ChronoConfigLibTests
{
    public class MainViewModelTests
    {
        [Fact]
        public void Validate_HasNoBpm_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix();

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Bpm is required").Value);
        }

        [Fact]
        public void Validate_BpmIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Bpm = "abc"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Bpm must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoFps_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix();

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Fps is required").Value);
        }

        [Fact]
        public void Validate_FpsIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Fps = "abc"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Fps must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoCadence_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix();

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Cadence is required").Value);
        }

        [Fact]
        public void Validate_CadenceIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Cadence = "abc"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Cadence must be a number").Value);
        }

        [Fact]
        public void Validate_HasNoPromptInterval_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix();

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "PromptInterval is required").Value);
        }

        [Fact]
        public void Validate_PromptIntervalIsNotANumber_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                PromptInterval = "abc"
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "PromptInterval must be a number").Value);
        }

        [Fact]
        public void Validate_NoTracks_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix();

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "At least one track is required").Value);
        }

        [Fact]
        public void Validate_SectionWithTypeNotSet_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
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
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "All sections must have their Type set").Value);
        }

        [Fact]
        public void Validate_TrackWithNoSections_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Tracks =
                [
                    new()
                ]
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "At least one section is required").Value);
        }

        [Fact]
        public void Validate_NoStartingSection_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Must have one starting section").Value);
        }

        [Fact]
        public void Validate_MoreThanOneStartingSection_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                Type = TrackSectionType.START
                                },
                            new() {
                                Type = TrackSectionType.START
                                }
                            ]
                        }
                ]
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Must have only one starting section").Value);
        }

        [Fact]
        public void Validate_NoEndingSection_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                Type = TrackSectionType.START
                                }
                            ]
                        }
                ]
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Must have one ending section").Value);
        }

        [Fact]
        public void Validate_MoreThanOneEndingSection_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
            {
                Tracks =
                [
                    new() {
                        Sections = [
                            new() {
                                Type = TrackSectionType.END
                                },
                            new() {
                                Type = TrackSectionType.END
                                }
                            ]
                        }
                ]
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Must have only one ending section").Value);
        }

        [Fact]
        public void Validate_SectionHasInvalidStartTime_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
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
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Invalid Start Time").Value);
        }

        [Fact]
        public void Validate_HasSectionStartTimeLessThanPrevious_ReturnsError()
        {
            var subject = GetSubject();
            subject.Mix = new Mix
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
            };

            var errors = subject.Validate();

            Assert.NotNull(errors.FirstOrDefault(e => e.Value == "Start Time is less than previous").Value);
        }

        [Fact]
        public void UpdateSectionTimes_TimeDidNotChange_ReturnsSameTime()
        {
            var subject = GetSubject();
            subject.Mix.Tracks[0].Sections = [
                new() {
                    Number = 1,
                    StartTime = "00:00:15"
                    }
            ];

            subject.UpdateSectionTimes(subject.Mix.Tracks[0], subject.Mix.Tracks[0].Sections[0], "00:00:15");

            Assert.Equal("00:00:15", subject.Mix.Tracks[0].Sections[0].StartTime);
        }

        [Fact]
        public void UpdateSectionTimes_TimeIncreasedByFiveSeconds_ShouldIncreaseTimes()
        {
            var subject = GetSubject();
            subject.Mix.Tracks[0].Sections = [
                new() {
                    Number = 1,
                    StartTime = "00:00:15"
                    },
                new() {
                    Number = 2,
                    StartTime = "00:00:30"
                    },
                new() {
                    Number = 3,
                    StartTime = "00:00:45"
                    }
            ];

            var changedTime = "00:00:35";


            subject.UpdateSectionTimes(subject.Mix.Tracks[0], subject.Mix.Tracks[0].Sections[1], changedTime);

            Assert.Equal("00:00:15", subject.Mix.Tracks[0].Sections[0].StartTime);
            Assert.Equal(changedTime, subject.Mix.Tracks[0].Sections[1].StartTime);
            Assert.Equal("00:00:50", subject.Mix.Tracks[0].Sections[2].StartTime);
        }

        [Fact]
        public void UpdateSectionTimes_TimeDecreasedByFiveSeconds_ShouldDecreaseTimes()
        {
            var subject = GetSubject();
            subject.Mix.Tracks[0].Sections = [
                new() {
                    Number = 1,
                    StartTime = "00:00:15"
                    },
                new() {
                    Number = 2,
                    StartTime = "00:00:30"
                    },
                new() {
                    Number = 3,
                    StartTime = "00:00:45"
                    }
            ];

            var changedTime = "00:00:25";


            subject.UpdateSectionTimes(subject.Mix.Tracks[0], subject.Mix.Tracks[0].Sections[1], changedTime);

            Assert.Equal("00:00:15", subject.Mix.Tracks[0].Sections[0].StartTime);
            Assert.Equal(changedTime, subject.Mix.Tracks[0].Sections[1].StartTime);
            Assert.Equal("00:00:40", subject.Mix.Tracks[0].Sections[2].StartTime);
        }

        private static MainViewModel GetSubject() => new();
    }
}
