using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace SoundKit
{
    public class SoundPool : MonoBehaviour
    {
        private static SoundPool s_instance;
        private readonly LinkedList<SoundPlaybackController> _soundPlayerHandlers = new();
        private readonly Queue<SoundPlayer> _soundPlayerQueue = new();

        public static SoundPool Instance
        {
            get
            {
                if (s_instance == null)
                    if (SoundKitSettings.Instance.IsAutoGenerate)
                        CreateInstance();
                    else
                        throw new InvalidOperationException(
                            "SoundManager is not created. Please create SoundManager manually or set SoundKitSettings.IsAutoGenerate to true.");

                return s_instance;
            }
        }

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;

            for (var i = 0; i < SoundKitSettings.Instance.InitialSoundPlayerCount; i++)
                _soundPlayerQueue.Enqueue(CreatePlayer());

            if (SoundKitSettings.Instance.IsDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            s_instance = null;
        }

        public static IReadOnlyCollection<SoundPlaybackController> GetActiveSoundPlayerHandlers()
        {
            return Instance._soundPlayerHandlers;
        }

        public SoundPlaybackController Play(in SoundPlayUnit soundPlayUnit)
        {
            var soundPlayer = RentPlayer();

            if (soundPlayer == null)
                return null;

            soundPlayer.Play(soundPlayUnit);
            return CreateHandler(soundPlayer);
        }

        private SoundPlaybackController CreateHandler(SoundPlayer soundPlayer)
        {
            var soundPlayerHandler = new SoundPlaybackController(soundPlayer);
            soundPlayer.OnPlayEnd.Take(1).Subscribe(_ => _soundPlayerHandlers.Remove(soundPlayerHandler))
                .AddTo(soundPlayer);
            _soundPlayerHandlers.AddFirst(soundPlayerHandler);

            return soundPlayerHandler;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (SoundKitSettings.Instance.IsAutoGenerate && s_instance == null) CreateInstance();
        }

        private static void CreateInstance()
        {
            var _ = new GameObject("SoundPool", typeof(SoundPool));
        }

        private SoundPlayer RentPlayer()
        {
            var soundPlayer = GetOrCreatePlayer();
            if (soundPlayer == null)
                return null;

            soundPlayer.gameObject.SetActive(true);

            return soundPlayer;
        }


        private SoundPlayer GetOrCreatePlayer()
        {
            if (_soundPlayerQueue.Count > 0)
                return _soundPlayerQueue.Dequeue();

            if (SoundKitSettings.Instance.MaxSoundPlayerCount < 0 ||
                _soundPlayerQueue.Count < SoundKitSettings.Instance.MaxSoundPlayerCount)
                return CreatePlayer();

            return null;
        }

        private SoundPlayer CreatePlayer()
        {
            var soundPlayerGameObject = new GameObject("SoundPlayer", typeof(AudioSource));
            soundPlayerGameObject.transform.SetParent(transform);

            var soundPlayer = soundPlayerGameObject.AddComponent<SoundPlayer>();
            soundPlayerGameObject.SetActive(false);

            soundPlayer.OnPlayEnd.Subscribe(_ =>
            {
                _soundPlayerQueue.Enqueue(soundPlayer);
                soundPlayer.gameObject.SetActive(false);
            }).AddTo(soundPlayer);

            return soundPlayer;
        }
    }
}