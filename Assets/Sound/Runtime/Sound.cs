using System.Collections;
using System.Collections.Generic;
using DarkNaku.GOPool;
using UnityEngine;

namespace DarkNaku.Sound {
    public class Sound : MonoBehaviour {
        public static Sound Instance {
            get {
                if (_isDestroyed) return null;

                lock (_lock) {
                    if (_instance == null) {
                        var instances = FindObjectsByType<Sound>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                        if (instances.Length > 0) {
                            _instance = instances[0];

                            for (int i = 1; i < instances.Length; i++)
                            {
                                Debug.LogWarningFormat("[Sound] Instance Duplicated - {0}", instances[i].name);
                                Destroy(instances[i]);
                            }
                        } else {
                            _instance = new GameObject($"[Singleton] Sound").AddComponent<Sound>();
                        }

                        _instance.Initialize();
                    }

                    return _instance;
                }
            }
        }

        public static float VolumeSFX {
            get {
                var volume = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);
                return Mathf.Clamp01(volume);
            }
            set {
                PlayerPrefs.SetFloat(SFX_VOLUME, value);
                PlayerPrefs.Save();
                Instance.UpdateVolumeSFX(value);
            }
        }

        public static float VolumeBGM {
            get {
                var volume = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
                return Mathf.Clamp01(volume);
            }
            set {
                PlayerPrefs.SetFloat(BGM_VOLUME, value);
                PlayerPrefs.Save();
                Instance.UpdateVolumeBGM(value);
            }
        }

        private const string SFX_VOLUME = "SFX_VOLUME";
        private const string BGM_VOLUME = "BGM_VOLUME";
        private static readonly object _lock = new();
        private static Sound _instance;
        private static bool _isDestroyed;

        private AudioSource _bgmPlayer;
        private HashSet<AudioSource> _sfxPlayers = new();
        private HashSet<string> _playedClipInThisFrame = new();

        public static void Play(string clipName) => Instance._Play(clipName);
        public static void Stop(string clipName) => Instance._Stop(clipName);
        public static void PlayBGM(string clipName) => Instance._PlayBGM(clipName);
        public static void StopBGM() => Instance._StopBGM();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration() {
            _instance = null;
        }

        private void OnEnable() {
            StartCoroutine(CoPreventDuplication());
        }

        private void Initialize() {
            _sfxPlayers.Clear();
            _playedClipInThisFrame.Clear();

            _bgmPlayer = new GameObject("BGM Player").AddComponent<AudioSource>();
            _bgmPlayer.transform.parent = transform;

            DontDestroyOnLoad(gameObject);
        }

        private void _Play(string clipName) {
            if (Mathf.Approximately(VolumeSFX, 0f)) return;
            if (_playedClipInThisFrame.Contains(clipName)) return;

            AudioSource player = GetPlayer();

            var clip = GetClip(clipName);

            if (clip == null) return;

            player.clip = clip;
            player.loop = false;
            player.volume = VolumeSFX;
            player.Play();

            _playedClipInThisFrame.Add(clipName);
        }

        private void _Stop(string clipName) {
            foreach (var player in _sfxPlayers) {
                if (player.isPlaying && player.clip.name.Equals(clipName)) {
                    player.Stop();
                }
            }
        }

        private void _PlayBGM(string clipName) {
            if (Mathf.Approximately(VolumeBGM, 0f)) return;

            var clip = GetClip(clipName);

            if (clip == null) return;

            _bgmPlayer.clip = clip;
            _bgmPlayer.loop = true;
            _bgmPlayer.volume = VolumeBGM;
            _bgmPlayer.Play();
        }

        private void _StopBGM() {
            _bgmPlayer.Stop();
        }

        private AudioSource GetPlayer() {
            AudioSource player = null;

            foreach (var source in _sfxPlayers) {
                if (source.isPlaying) continue;
                player = source;
                break;
            }

            if (player == null) {
                player = new GameObject("SFX Player").AddComponent<AudioSource>();
                player.transform.parent = transform;
                _sfxPlayers.Add(player);
            }

            return player;
        }

        private AudioClip GetClip(string clipName) {
            if (SoundConfig.Clips.TryGetValue(clipName, out var clip)) {
                return clip;
            } else {
                Debug.LogErrorFormat("[Sound] GetClip : Can't found audio clip - {0}", clipName);
                return null;
            }
        }

        private void UpdateVolumeBGM(float volume) {
            _bgmPlayer.volume = volume;
        }

        private void UpdateVolumeSFX(float volume) {
            foreach (var player in _sfxPlayers) {
                player.volume = volume;
            }
        }

        private IEnumerator CoPreventDuplication() {
            var endOfFrame = new WaitForEndOfFrame();

            while (true) {
                yield return endOfFrame;

                _playedClipInThisFrame.Clear();
            }
        }
    }
}