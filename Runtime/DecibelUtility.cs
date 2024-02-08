using UnityEngine;

namespace SoundKit
{
    public static class DecibelUtility
    {
        public static float LinearToDecibel(float linear)
        {
            return linear <= 0 ? -80f : 20f * Mathf.Log10(linear);
        }

        public static float DecibelToLinear(float decibel)
        {
            return Mathf.Pow(10f, decibel / 20f);
        }
    }
}