using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    public class SoundPlayUnitBuilder : ISoundFactory
    {
        public SoundPlayUnitBuilder(AudioClip clip, AudioMixerGroup outputAudioMixerGroup = null,
            float volume = 1f, float pitch = 1f, int priority = 128, float panStereo = 0f,
            int startSample = 0, int endSample = -1, int loopStartSample = 0,
            int loopCount = 0, bool isLoopIntervalPreserved = true,
            SoundTimingMode timingMode = SoundTimingMode.Immediate, double timingValue = 0d,
            double scheduledEndTime = -1d)
        {
            Clip = clip;
            OutputAudioMixerGroup = outputAudioMixerGroup;
            Volume = volume;
            Pitch = pitch;
            Priority = priority;
            PanStereo = panStereo;
            StartSample = startSample;
            EndSample = endSample;
            LoopStartSample = loopStartSample;
            LoopCount = loopCount;
            IsLoopIntervalPreserved = isLoopIntervalPreserved;
            TimingMode = timingMode;
            TimingValue = timingValue;
            ScheduledEndTime = scheduledEndTime;
        }

        public SoundPlayUnitBuilder(SoundPlayUnit soundPlayUnit)
        {
            Clip = soundPlayUnit.Clip;
            OutputAudioMixerGroup = soundPlayUnit.OutputAudioMixerGroup;
            Volume = soundPlayUnit.Volume;
            Pitch = soundPlayUnit.Pitch;
            Priority = soundPlayUnit.Priority;
            PanStereo = soundPlayUnit.PanStereo;
            StartSample = soundPlayUnit.StartSample;
            EndSample = soundPlayUnit.EndSample;
            LoopStartSample = soundPlayUnit.LoopStartSample;
            LoopCount = soundPlayUnit.LoopCount;
            IsLoopIntervalPreserved = soundPlayUnit.IsLoopIntervalPreserved;
            TimingMode = soundPlayUnit.TimingMode;
            TimingValue = soundPlayUnit.TimingValue;
            ScheduledEndTime = soundPlayUnit.ScheduledEndTime;
        }

        public AudioClip Clip { get; }
        public AudioMixerGroup OutputAudioMixerGroup { get; private set; }
        public bool Mute { get; private set; }
        public float Volume { get; private set; }
        public float Pitch { get; private set; }
        public int Priority { get; private set; }
        public float PanStereo { get; private set; }
        public int StartSample { get; private set; }
        public int EndSample { get; private set; }
        public int LoopStartSample { get; private set; }
        public int LoopCount { get; private set; }
        public bool IsLoopIntervalPreserved { get; private set; }
        public SoundTimingMode TimingMode { get; private set; }
        public double TimingValue { get; private set; }
        public double ScheduledEndTime { get; private set; }

        public double DspTime => SoundPlayUnitUtility.EvaluateDspTime(TimingMode, TimingValue);

        public SoundPlayUnit Create()
        {
            var endSample = EndSample < 0 ? Clip.samples : EndSample;
            return new SoundPlayUnit(
                Clip,
                endSample,
                OutputAudioMixerGroup,
                Mute,
                Volume,
                Pitch,
                Priority,
                PanStereo,
                StartSample,
                LoopStartSample,
                LoopCount,
                IsLoopIntervalPreserved);
        }

        public SoundPlayUnitBuilder SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            OutputAudioMixerGroup = audioMixerGroup;
            return this;
        }

        public SoundPlayUnitBuilder SetMute(bool mute = true)
        {
            Mute = mute;
            return this;
        }

        public SoundPlayUnitBuilder SetVolume(float volume)
        {
            Volume = volume;
            return this;
        }

        public SoundPlayUnitBuilder SetPitch(float pitch)
        {
            Pitch = pitch;
            return this;
        }

        public SoundPlayUnitBuilder SetPriority(int priority)
        {
            Priority = priority;
            return this;
        }

        public SoundPlayUnitBuilder SetPanStereo(float panStereo)
        {
            PanStereo = panStereo;
            return this;
        }

        public SoundPlayUnitBuilder SetStartSample(int startSample)
        {
            StartSample = startSample;
            return this;
        }

        public SoundPlayUnitBuilder SetEndSample(int endSample)
        {
            EndSample = endSample;
            return this;
        }

        public SoundPlayUnitBuilder SetLoopStartSample(int loopStartSample)
        {
            LoopStartSample = loopStartSample;
            return this;
        }

        public SoundPlayUnitBuilder SetLoopCount(int loopCount)
        {
            LoopCount = loopCount;
            return this;
        }

        public SoundPlayUnitBuilder SetLoopIntervalPreserved(bool isLoopIntervalPreserved)
        {
            IsLoopIntervalPreserved = isLoopIntervalPreserved;
            return this;
        }

        public SoundPlayUnitBuilder SetDelay(double delay)
        {
            TimingMode = SoundTimingMode.Delay;
            TimingValue = delay;
            return this;
        }

        public SoundPlayUnitBuilder SetSchedule(double time)
        {
            TimingMode = SoundTimingMode.Schedule;
            TimingValue = time;
            return this;
        }

        public SoundPlayUnitBuilder SetImmediate()
        {
            TimingMode = SoundTimingMode.Immediate;
            TimingValue = 0d;
            return this;
        }

        public SoundPlayUnitBuilder SetScheduledEndTime(double scheduledEndTime)
        {
            ScheduledEndTime = scheduledEndTime;
            return this;
        }
    }
}