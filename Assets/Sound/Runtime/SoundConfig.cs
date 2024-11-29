using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkNaku.GOPool
{
    public class SoundConfig : ScriptableObject {
        [SerializeField] private List<AudioClip> _clips;

        public static SoundConfig Instance {
            get {
                if (_instance == null) {
                    var assetName = typeof(SoundConfig).Name;

                    _instance = Resources.Load<SoundConfig>(assetName);

                    if (_instance == null) {
                        _instance = CreateInstance<SoundConfig>();

#if UNITY_EDITOR
                        var assetPath = "Resources";
                        var resourcePath = System.IO.Path.Combine(Application.dataPath, assetPath);

                        if (System.IO.Directory.Exists(resourcePath) == false) {
                            UnityEditor.AssetDatabase.CreateFolder("Assets", assetPath);
                        }

                        UnityEditor.AssetDatabase.CreateAsset(_instance, $"Assets/{assetPath}/{assetName}.asset");
#endif
                    }

                    _instance.Initialize();
                }

                return _instance;
            }
        }

        public static IReadOnlyDictionary<string, AudioClip> Clips => Instance._clipTable;
    
        private static SoundConfig _instance;

        private Dictionary<string, AudioClip> _clipTable;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Sound Config")]
        private static void SelectConfig() {
            UnityEditor.Selection.activeObject = Instance;
        }
#endif

        private void Initialize() {
            _clipTable = new Dictionary<string, AudioClip>();

            foreach (var clip in _clips) {
                _clipTable.TryAdd(clip.name, clip);
            }
        }
    }
}