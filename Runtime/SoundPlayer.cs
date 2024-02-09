using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("")]
    public class SoundPlayer : MonoBehaviour
    {
        private readonly Subject<PlayEndType> _onPlayEnd = new();

        private AudioSource _audioSource;
        private bool _isUsing;

        public IObservable<PlayEndType> OnPlayEnd => _onPlayEnd;

        public AudioClip Clip => _audioSource.clip;
        public AudioMixerGroup OutputAudioMixerGroup => _audioSource.outputAudioMixerGroup;

        public float Time => _audioSource.time;

        public bool IsPlaying => _audioSource.isPlaying;

        public bool Mute
        {
            get => _audioSource.mute;
            set => _audioSource.mute = value;
        }

        public float Volume
        {
            get => _audioSource.volume;
            set => _audioSource.volume = value;
        }

        public float Pitch
        {
            get => _audioSource.pitch;
            set => _audioSource.pitch = value;
        }

        public float PanStereo
        {
            get => _audioSource.panStereo;
            set => _audioSource.panStereo = value;
        }

        public int LoopCount { get; set; }

        public int TimeSamples
        {
            get => _audioSource.timeSamples;
            set => _audioSource.timeSamples = value;
        }

        public int Priority
        {
            get => _audioSource.priority;
            set => _audioSource.priority = value;
        }

        public bool IsLoopIntervalPreserved { get; set; }

        public int LoopStartSample { get; set; }

        public int EndSample { get; set; }
        public double PlayDspTime { get; private set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }


        private void Update()
        {
            CheckPlayFinished();
        }

        private void OnDestroy()
        {
            if (_isUsing)
                PlayEnd(PlayEndType.Destroy);
        }

        private void CheckPlayFinished()
        {
            if (_audioSource.timeSamples < EndSample && _audioSource.isPlaying)
                return;
            if (LoopCount > 0)
                LoopCount--;
            if (LoopCount == 0)
            {
                PlayEnd(PlayEndType.Finish);
                return;
            }

            Loop();
        }

        private void Loop()
        {
            if (IsLoopIntervalPreserved && _audioSource.isPlaying)
            {
                var gap = _audioSource.timeSamples - EndSample;
                _audioSource.timeSamples = LoopStartSample + gap;
            }
            else
            {
                _audioSource.timeSamples = LoopStartSample;
            }

            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }

        public void Play(in SoundPlayUnit soundPlayUnit)
        {
            if (_isUsing)
                throw new InvalidOperationException("SoundPlayer is already using");

            _isUsing = true;
            SetUp(soundPlayUnit);

            PlayDspTime = SoundPlayUnitUtility.EvaluateDspTime(soundPlayUnit.TimingMode, soundPlayUnit.TimingValue);
            switch (soundPlayUnit.TimingMode)
            {
                case SoundTimingMode.Immediate:
                    _audioSource.Play();
                    break;
                case SoundTimingMode.Schedule:
                    _audioSource.PlayScheduled(soundPlayUnit.TimingValue);
                    break;
                case SoundTimingMode.Delay:
                    _audioSource.PlayDelayed((float)soundPlayUnit.TimingValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (soundPlayUnit.IsScheduledEnd)
                SetScheduledEndTime(soundPlayUnit.ScheduledEndTime);
        }

        private void SetUp(in SoundPlayUnit soundPlayUnit)
        {
            _audioSource.clip = soundPlayUnit.Clip;
            _audioSource.mute = soundPlayUnit.Mute;
            _audioSource.outputAudioMixerGroup = soundPlayUnit.OutputAudioMixerGroup;
            _audioSource.priority = soundPlayUnit.Priority;
            _audioSource.volume = soundPlayUnit.Volume;
            _audioSource.pitch = soundPlayUnit.Pitch;
            _audioSource.panStereo = soundPlayUnit.PanStereo;
            LoopCount = soundPlayUnit.LoopCount;
            _audioSource.timeSamples = soundPlayUnit.StartSample;
            EndSample = soundPlayUnit.EndSample;
            LoopStartSample = soundPlayUnit.LoopStartSample;
            IsLoopIntervalPreserved = soundPlayUnit.IsLoopIntervalPreserved;
        }

        private void PlayEnd(PlayEndType playEndType)
        {
            _audioSource.Stop();
            _onPlayEnd.OnNext(playEndType);
            _isUsing = false;
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public void UnPause()
        {
            _audioSource.UnPause();
        }

        public void SetScheduledStartTime(double time)
        {
            _audioSource.SetScheduledStartTime(time);
        }

        public void SetScheduledEndTime(double time)
        {
            _audioSource.SetScheduledEndTime(time);
        }

        public void Stop()
        {
            if (!_isUsing)
                return;

            _audioSource.Stop();
            PlayEnd(PlayEndType.Stop);
        }
    }
}