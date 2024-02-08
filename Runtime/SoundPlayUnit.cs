using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    public enum SoundTimingMode
    {
        Immediate,
        Schedule,
        Delay
    }

    public readonly struct SoundPlayUnit
    {
        public readonly AudioClip Clip;
        public readonly AudioMixerGroup OutputAudioMixerGroup;
        public readonly bool Mute;
        public readonly float Volume;
        public readonly float Pitch;
        public readonly int Priority;
        public readonly float PanStereo;
        public readonly int StartSample;
        public readonly int EndSample;
        public readonly int LoopStartSample;
        public readonly int LoopCount;
        public readonly bool IsLoopIntervalPreserved;

        public readonly SoundTimingMode TimingMode;
        public readonly double TimingValue;

        public readonly double ScheduledEndTime;

        public bool IsScheduledEnd => ScheduledEndTime >= 0;

        public double DspTime => SoundPlayUnitUtility.EvaluateDspTime(TimingMode, TimingValue);

        public SoundPlayUnit(AudioClip clip, int endSample, AudioMixerGroup outputAudioMixerGroup = null,
            bool mute = false,
            float volume = 1f, float pitch = 1f, int priority = 128, float panStereo = 0f,
            int startSample = 0, int loopStartSample = 0,
            int loopCount = 0, bool isLoopIntervalPreserved = true,
            SoundTimingMode timingMode = SoundTimingMode.Immediate, double timingValue = 0d,
            double scheduledEndTime = -1d)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            if (endSample < startSample || endSample > clip.samples)
                throw new ArgumentOutOfRangeException(nameof(endSample));

            Clip = clip;
            OutputAudioMixerGroup = outputAudioMixerGroup;
            Mute = mute;
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
    }
}