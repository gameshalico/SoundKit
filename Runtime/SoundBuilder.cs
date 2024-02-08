using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    public static class SoundBuilder
    {
        public static SoundPlayUnitBuilder Create(AudioClip clip, AudioMixerGroup outputAudioMixerGroup = null,
            float volume = 1f, float pitch = 1f, int priority = 128, float panStereo = 0f,
            int startSample = 0, int endSample = -1, int loopStartSample = 0,
            int loopCount = 0, bool isLoopIntervalPreserved = true,
            SoundTimingMode timingMode = SoundTimingMode.Immediate, double timingValue = 0d,
            double scheduledEndTime = -1d)
        {
            return new SoundPlayUnitBuilder(clip, outputAudioMixerGroup, volume, pitch, priority, panStereo,
                startSample, endSample, loopStartSample, loopCount, isLoopIntervalPreserved, timingMode,
                timingValue, scheduledEndTime);
        }
    }
}