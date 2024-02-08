using System;
using System.Linq;
using UnityEngine;

namespace SoundKit
{
    public static class SoundPlayUnitUtility
    {
        public static double EvaluateDspTime(SoundTimingMode timingMode, double timingValue)
        {
            return timingMode switch
            {
                SoundTimingMode.Immediate => AudioSettings.dspTime,
                SoundTimingMode.Schedule => timingValue,
                SoundTimingMode.Delay => AudioSettings.dspTime + timingValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }


        public static float CalculateAdjustedVolumeRateForSimultaneousClips(AudioClip clip, double dspTime)
        {
            var sameClipHandlersDspTime = SoundPool.GetActiveSoundPlayerHandlers()
                .Where(x => x.Clip == clip && x.Volume > 0).Select(x => x.PlayDspTime);

            var volumeRate = 1f;
            foreach (var handler in sameClipHandlersDspTime)
            {
                var diff = Mathf.Abs((float)(dspTime - handler));

                if (diff < 0.025f) return 0;
                if (diff < 0.05f) volumeRate *= 0.8f;
                else if (diff < 0.1f) volumeRate *= 0.9f;
            }

            return volumeRate;
        }
    }
}