using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundKit
{
    public class SoundPlaybackController : IDisposable
    {
        private readonly IDisposable _playEndSubscription;
        private readonly SoundPlayer _soundPlayer;

        private CancellationTokenSource _fadeCts;

        public SoundPlaybackController(SoundPlayer soundPlayer)
        {
            _soundPlayer = soundPlayer;
            IsReleased = false;
            _playEndSubscription = _soundPlayer.OnPlayEnd.Subscribe(OnPlayEnd);
        }

        public bool IsReleased { get; private set; }

        public bool IsPlaying
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.IsPlaying;
            }
        }

        public AudioClip Clip
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Clip;
            }
        }

        public AudioMixerGroup OutputAudioMixerGroup
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.OutputAudioMixerGroup;
            }
        }

        public float Time
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Time;
            }
        }

        public bool Mute
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Mute;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.Mute = value;
            }
        }

        public float Volume
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Volume;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.Volume = value;
            }
        }

        public float Pitch
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Pitch;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.Pitch = value;
            }
        }

        public float PanStereo
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.PanStereo;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.PanStereo = value;
            }
        }

        public int LoopCount
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.LoopCount;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.LoopCount = value;
            }
        }

        public int TimeSamples
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.TimeSamples;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.TimeSamples = value;
            }
        }

        public int Priority
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.Priority;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.Priority = value;
            }
        }

        public bool MaintainLoopGap
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.IsLoopIntervalPreserved;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.IsLoopIntervalPreserved = value;
            }
        }

        public int LoopStartSample
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.LoopStartSample;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.LoopStartSample = value;
            }
        }

        public int EndSample
        {
            get
            {
                ThrowIfReleased();
                return _soundPlayer.EndSample;
            }
            set
            {
                ThrowIfReleased();
                _soundPlayer.EndSample = value;
            }
        }

        public double PlayDspTime => _soundPlayer.PlayDspTime;

        public void Dispose()
        {
            _playEndSubscription.Dispose();
            IsReleased = true;
        }

        public async UniTask FadeVolumeInDecibelAsync(float targetVolume, float duration,
            CancellationToken cancellationToken = default)
        {
            _fadeCts?.Cancel();
            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(_soundPlayer.destroyCancellationToken);
            var startVolumeDb = DecibelUtility.LinearToDecibel(_soundPlayer.Volume);
            var targetVolumeDb = DecibelUtility.LinearToDecibel(targetVolume);
            var elapsedTime = 0f;


            while (elapsedTime < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (IsReleased) return;
                elapsedTime += UnityEngine.Time.unscaledDeltaTime;
                _soundPlayer.Volume = DecibelUtility.DecibelToLinear(
                    Mathf.Lerp(startVolumeDb, targetVolumeDb, elapsedTime / duration));
                await UniTask.Yield();
            }

            _soundPlayer.Volume = targetVolume;
        }

        public async UniTask FadeVolumeInLinearAsync(float targetVolume, float duration,
            CancellationToken cancellationToken = default)
        {
            _fadeCts?.Cancel();
            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(_soundPlayer.destroyCancellationToken);
            var startVolume = _soundPlayer.Volume;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (IsReleased) return;
                elapsedTime += UnityEngine.Time.unscaledDeltaTime;
                _soundPlayer.Volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
                await UniTask.Yield();
            }

            _soundPlayer.Volume = targetVolume;
        }

        public async UniTask CrossFadeBySinAsync(SoundPlaybackController other, float duration,
            float targetVolume = 1.0f,
            CancellationToken cancellationToken = default)
        {
            _fadeCts?.Cancel();
            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(_soundPlayer.destroyCancellationToken,
                other._soundPlayer.destroyCancellationToken);
            other._fadeCts?.Cancel();
            other._fadeCts = _fadeCts;

            var startVolume = _soundPlayer.Volume;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (IsReleased) return;
                elapsedTime += UnityEngine.Time.unscaledDeltaTime;

                var t = Mathf.Sin(elapsedTime / duration * Mathf.PI / 2);

                _soundPlayer.Volume = Mathf.Lerp(startVolume, 0, t);
                other.Volume = Mathf.Lerp(0, targetVolume, t);
                await UniTask.Yield();
            }

            _soundPlayer.Volume = 0;
            other.Volume = targetVolume;
        }

        public async UniTask CrossFadeBySqrtAsync(SoundPlaybackController other, float duration,
            float targetVolume = 1.0f,
            CancellationToken cancellationToken = default)
        {
            _fadeCts?.Cancel();
            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(_soundPlayer.destroyCancellationToken,
                other._soundPlayer.destroyCancellationToken);
            other._fadeCts?.Cancel();
            other._fadeCts = _fadeCts;

            var startVolume = _soundPlayer.Volume;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (IsReleased) return;
                elapsedTime += UnityEngine.Time.unscaledDeltaTime;

                var t = elapsedTime / duration;

                _soundPlayer.Volume = Mathf.Lerp(startVolume, 0, 1 - Mathf.Sqrt(1 - t));
                other.Volume = Mathf.Lerp(0, targetVolume, Mathf.Sqrt(t));
                await UniTask.Yield();
            }

            _soundPlayer.Volume = 0;
            other.Volume = targetVolume;
        }

        public void Pause()
        {
            ThrowIfReleased();
            _soundPlayer.Pause();
        }

        public void UnPause()
        {
            ThrowIfReleased();
            _soundPlayer.UnPause();
        }

        public void Stop()
        {
            ThrowIfReleased();
            _soundPlayer.Stop();
        }

        public SoundPlaybackController SetScheduledStartTime(double time)
        {
            ThrowIfReleased();
            _soundPlayer.SetScheduledStartTime(time);
            return this;
        }

        public SoundPlaybackController SetScheduledEndTime(double time)
        {
            ThrowIfReleased();
            _soundPlayer.SetScheduledEndTime(time);
            return this;
        }

        public async UniTask<PlayEndType> ToUniTask()
        {
            ThrowIfReleased();
            return await _soundPlayer.OnPlayEnd.First().ToUniTask();
        }

        private void ThrowIfReleased()
        {
            if (IsReleased) throw new ObjectDisposedException(nameof(SoundPlaybackController));
        }

        private void OnPlayEnd(PlayEndType playEndType)
        {
            Dispose();
        }
    }
}