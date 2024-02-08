using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace SoundKit
{
    public class SoundKitSettings : ScriptableObject
    {
        private static SoundKitSettings _instance;

        [SerializeField] private int _initialSoundPlayerCount;
        [SerializeField] private int _maxSoundPlayerCount = -1;
        [SerializeField] private bool _isAutoGenerate = true;
        [SerializeField] private bool _isDontDestroyOnLoad = true;

        public int InitialSoundPlayerCount => _initialSoundPlayerCount;
        public int MaxSoundPlayerCount => _maxSoundPlayerCount;
        public bool IsAutoGenerate => _isAutoGenerate;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;


        public static SoundKitSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    var asset = PlayerSettings.GetPreloadedAssets().OfType<SoundKitSettings>().FirstOrDefault();
                    _instance = asset != null ? asset : CreateInstance<SoundKitSettings>();
                }

                return _instance;

#else
                if (_instance == null) _instance = CreateInstance<SoundKitSettings>();

                return _instance;
#endif
            }
        }

        private void OnEnable()
        {
            _instance = this;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/SoundKit Settings")]
        private static void Create()
        {
            var assetPath =
                EditorUtility.SaveFilePanelInProject($"Save {nameof(SoundKitSettings)}", nameof(SoundKitSettings),
                    "asset", "", "Assets");

            if (string.IsNullOrEmpty(assetPath))
                return;

            var instance = CreateInstance<SoundKitSettings>();
            AssetDatabase.CreateAsset(instance, assetPath);
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(x => x is SoundKitSettings);
            preloadedAssets.Add(instance);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }
#endif
    }
}