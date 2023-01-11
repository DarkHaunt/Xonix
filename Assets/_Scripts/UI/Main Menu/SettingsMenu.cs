using UnityEngine;
using UnityEngine.UI;
using Xonix.Audio;



namespace Xonix.UI
{
    using static AudioManager2D;

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle _soundOnToggle;
        [SerializeField] private Toggle _musicOnToggle;



        private void Init()
        {
            _soundOnToggle.onValueChanged.AddListener((isVolumeOffed) => SetAudioMixerVolume(isVolumeOffed, AudioMixerGroupType.Sound));
            _musicOnToggle.onValueChanged.AddListener((isVolumeOffed) => SetAudioMixerVolume(isVolumeOffed, AudioMixerGroupType.Music));
        }

        private void SetAudioMixerVolume(bool isVolumeOn, AudioMixerGroupType audioMixerGroupType)
        {
            if (isVolumeOn)
            {
                AudioManager2D.OnMixerGroupAudio(audioMixerGroupType);
                return;
            }

            AudioManager2D.OffMixerGroupAudio(audioMixerGroupType);
        }



        private void Awake()
        {
            Init();
        }
    }
}
