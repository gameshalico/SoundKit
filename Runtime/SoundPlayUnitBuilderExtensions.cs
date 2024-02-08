using UnityEngine;

namespace SoundKit
{
    public static class SoundPlayUnitBuilderExtensions
    {
        public static SoundPlayUnitBuilder SetVolumeRandom(this SoundPlayUnitBuilder builder, float min,
            float max)
        {
            return builder.SetVolume(Random.Range(min, max));
        }

        public static SoundPlayUnitBuilder SetPitchRandom(this SoundPlayUnitBuilder builder, float min,
            float max)
        {
            return builder.SetPitch(Random.Range(min, max));
        }


        public static SoundPlayUnitBuilder AdjustVolumeForSimultaneousClips(this SoundPlayUnitBuilder builder)
        {
            return builder.SetVolume(builder.Volume *
                                     SoundPlayUnitUtility.CalculateAdjustedVolumeRateForSimultaneousClips(builder.Clip,
                                         builder.DspTime));
        }
    }
}