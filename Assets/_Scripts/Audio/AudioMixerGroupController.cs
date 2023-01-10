using UnityEngine.Audio;



namespace Xonix.Audio
{
    /// <summary>
    /// Confortable tool for mixer group exposed parameters controlling
    /// </summary>
    internal class AudioMixerGroupController
    {
        private readonly string _mixerVolumeParameterName;
        private readonly AudioMixerGroup _audioMixerGroup;



        public AudioMixerGroup AudioGroup => _audioMixerGroup;



        internal AudioMixerGroupController(string mixerVolumeParameterName, AudioMixerGroup audioMixerGroup)
        {
            _mixerVolumeParameterName = mixerVolumeParameterName;
            _audioMixerGroup = audioMixerGroup;
        }



        internal void ChangeVolume(float volumeValue) => _audioMixerGroup.audioMixer.SetFloat(_mixerVolumeParameterName, volumeValue);
    }
}