namespace SoundKit
{
    public static class SoundPlayUnitExtensions
    {
        public static SoundPlaybackController Play(in this SoundPlayUnit soundPlayUnit)
        {
            return SoundPool.Instance.Play(soundPlayUnit);
        }

        public static SoundPlayUnitBuilder ToBuilder(in this SoundPlayUnit soundPlayUnit)
        {
            return new SoundPlayUnitBuilder(soundPlayUnit);
        }

        public static SoundPlayUnit AdjustVolumeForSimultaneousClips(in this SoundPlayUnit soundPlayUnit)
        {
            var volumeRate =
                SoundPlayUnitUtility.CalculateAdjustedVolumeRateForSimultaneousClips(soundPlayUnit.Clip,
                    soundPlayUnit.DspTime);
            return new SoundPlayUnit(soundPlayUnit.Clip, soundPlayUnit.EndSample, soundPlayUnit.OutputAudioMixerGroup,
                soundPlayUnit.Mute,
                soundPlayUnit.Volume * volumeRate,
                soundPlayUnit.Pitch, soundPlayUnit.Priority, soundPlayUnit.PanStereo, soundPlayUnit.StartSample,
                soundPlayUnit.LoopStartSample, soundPlayUnit.LoopCount,
                soundPlayUnit.IsLoopIntervalPreserved);
        }
    }
}