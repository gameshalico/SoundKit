namespace SoundKit
{
    public static class SoundPlayUnitFactoryExtensions
    {
        public static SoundPlaybackController Play(this ISoundFactory soundPlayUnitFactory)
        {
            return SoundPool.Instance.Play(soundPlayUnitFactory.Create());
        }
    }
}