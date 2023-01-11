using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Xonix.Audio
{
    /// <summary>
    /// A simple one-flow audio manager
    /// </summary>
    public class AudioManager2D : MonoBehaviour
    {
        private static AudioManager2D _instance;

        // Default audio mixer volume parameterss
        private const float MinDbVolumeValue = -80f;
        private const float DefaultDbVolumeValue = 0f;

        // Controlled mixer group exposed patameter names
        private const string MusicVolumeExposedParameterName = "Music Volume";
        private const string SoundVolumeExposedParameterName = "Sound Volume";


        [Header("--- Mixer Groups ---")]
        [SerializeField] private AudioMixerGroup _soundMixerGroup;

        [Space(5f)]
        [SerializeField] private AudioMixerGroup _musicMixerGroup;

        [Header("--- Sound Sources ---")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundSource;

        private Dictionary<AudioMixerGroupType, AudioMixerGroupController> _audioMixerGroupControllers;



        public static void PlayMusic(AudioClip musicClip)
        {
            PlayAudio(musicClip, _instance._musicSource, AudioMixerGroupType.Music);
        }

        public static void PlaySound(AudioClip soundClip)
        {
            if (_instance._soundSource.isPlaying)
                return;

            PlayAudio(soundClip, _instance._soundSource, AudioMixerGroupType.Sound);
        }

        private static void PlayAudio(AudioClip clip, AudioSource source, AudioMixerGroupType mixerGroupType)
        {
            source.outputAudioMixerGroup = _instance._audioMixerGroupControllers[mixerGroupType].AudioGroup;
            source.clip = clip;
            source.Play();
        }

        public static void OffMixerGroupAudio(AudioMixerGroupType mixerGroupType)
        {
            _instance._audioMixerGroupControllers[mixerGroupType].ChangeVolume(MinDbVolumeValue);
        }

        public static void OnMixerGroupAudio(AudioMixerGroupType mixerGroupType)
        {
            _instance._audioMixerGroupControllers[mixerGroupType].ChangeVolume(DefaultDbVolumeValue);
        }

        private void Init()
        {
            _audioMixerGroupControllers = new Dictionary<AudioMixerGroupType, AudioMixerGroupController>()
            {
                [AudioMixerGroupType.Music] = new AudioMixerGroupController(MusicVolumeExposedParameterName, _musicMixerGroup),
                [AudioMixerGroupType.Sound] = new AudioMixerGroupController(SoundVolumeExposedParameterName, _soundMixerGroup),
            };

            DontDestroyOnLoad(gameObject);
        }



        private void Awake()
        {
            #region [Singleton]

            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            #endregion

            Init();
        }



        [SerializeField]
        public enum AudioMixerGroupType
        {
            Sound,
            Music
        }
    }
}
