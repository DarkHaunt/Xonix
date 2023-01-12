using UnityEngine;
using UnityEngine.UI;
using Xonix.Audio;
using Xonix.Saving;
using Xonix.Saving.Core;



namespace Xonix.UI
{
    using static AudioManager2D;

    public class SettingsMenu : MonoBehaviour
    {
        private const string SavedSettingsFileName = "Settings.txt";


        [Header("--- Settings ---")]
        [SerializeField] private Toggle _soundOnToggle;
        [SerializeField] private Toggle _musicOnToggle;

        private ISaveSystem _saveSystem;



        private void Init()
        {
            _saveSystem = new JSONSaveSystem();

            _soundOnToggle.onValueChanged.AddListener((isVolumeOffed) => SetAudioMixerVolume(isVolumeOffed, AudioMixerGroupType.Sound));
            _musicOnToggle.onValueChanged.AddListener((isVolumeOffed) => SetAudioMixerVolume(isVolumeOffed, AudioMixerGroupType.Music));
        }

        private void SetAudioMixerVolume(bool isVolumeOn, AudioMixerGroupType audioMixerGroupType)
        {
            if (isVolumeOn)
            {
                OffMixerGroupAudio(audioMixerGroupType);
                return;
            }

            OnMixerGroupAudio(audioMixerGroupType);
        }

        private void LoadSettings()
        {
            var savedSettings = _saveSystem.Load<SavedSettings>(SavedSettingsFileName);

            if (savedSettings is default(SavedSettings))
                return;

            _soundOnToggle.isOn = savedSettings.IsSoundOff;
            _musicOnToggle.isOn = savedSettings.IsMusicOff;
        }

        private void SaveSettings()
        {
            var savedSettings = new SavedSettings()
            {
                IsMusicOff = _musicOnToggle.isOn,
                IsSoundOff = _soundOnToggle.isOn
            };

            _saveSystem.Save(savedSettings, SavedSettingsFileName);
        }



        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            // Mixer Groups should be inited in Start
            LoadSettings();
        }

        private void OnDestroy()
        {
            SaveSettings();
        }
    }
}
