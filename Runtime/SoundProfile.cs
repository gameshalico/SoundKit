using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    [Serializable]
    public class SoundProfile : ISoundFactory
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioMixerGroup _outputAudioMixerGroup;

        [SerializeField] private bool _mute;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _pitch = 1f;
        [SerializeField] private int _priority = 128;
        [SerializeField] private float _panStereo;
        [SerializeField] private int _startSample;
        [SerializeField] private int _endSample = -1;
        [SerializeField] private int _loopStartSample;
        [SerializeField] private int _loopCount;
        [SerializeField] private bool _isLoopIntervalPreserved = true;
        [SerializeField] private SoundTimingMode _timingMode = SoundTimingMode.Immediate;
        [SerializeField] private double _timingValue;
        [SerializeField] private double _scheduledEndTime = -1d;

        public AudioClip Clip => _clip;
        public AudioMixerGroup OutputAudioMixerGroup => _outputAudioMixerGroup;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public int Priority => _priority;
        public float PanStereo => _panStereo;
        public int StartSample => _startSample;
        public int EndSample => _endSample;
        public int LoopStartSample => _loopStartSample;
        public int LoopCount => _loopCount;
        public bool IsLoopIntervalPreserved => _isLoopIntervalPreserved;
        public SoundTimingMode TimingMode => _timingMode;
        public double TimingValue => _timingValue;
        public double ScheduledEndTime => _scheduledEndTime;

        public SoundPlayUnit Create()
        {
            var endSample = _endSample < 0 ? _clip.samples : _endSample;

            return new SoundPlayUnit(_clip, endSample, _outputAudioMixerGroup, _mute, _volume, _pitch, _priority,
                _panStereo,
                _startSample, _loopStartSample, _loopCount, _isLoopIntervalPreserved, _timingMode,
                _timingValue, _scheduledEndTime);
        }

        public SoundPlayUnitBuilder ToBuilder()
        {
            return new SoundPlayUnitBuilder(_clip, _outputAudioMixerGroup, _volume, _pitch, _priority, _panStereo,
                _startSample, _endSample, _loopStartSample, _loopCount, _isLoopIntervalPreserved, _timingMode,
                _timingValue, _scheduledEndTime);
        }
    }
}