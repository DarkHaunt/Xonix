using UnityEngine;



namespace Xonix.Audio
{
    /// <summary>
    /// A simple one-flow sound manager
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;


        private AudioSource _audioSource;



        public static void PlayClip(AudioClip audioClip)
        {
            if (_instance._audioSource.isPlaying)
                return;

            _instance._audioSource.clip = audioClip;
            _instance._audioSource.Play();
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

            _audioSource = GetComponent<AudioSource>();
        }
    } 
}
