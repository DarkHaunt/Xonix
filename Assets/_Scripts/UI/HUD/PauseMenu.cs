using UnityEngine;
using System;
using UnityEngine.UI;



namespace Xonix
{
    public class PauseMenu : MonoBehaviour
    {
        public static event Action OnPause;
        public static event Action OnResume;

        [Header("--- Canvases ---")]
        [SerializeField] private Canvas _pauseCanvas;

        [Header("--- Buttons ---")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;



        private void Resume()
        {
            _pauseButton.gameObject.SetActive(true); // Hide the pause button
            _pauseCanvas.gameObject.SetActive(false);

            Time.timeScale = 1;

            OnResume?.Invoke();
        }

        private void Pause()
        {
            _pauseButton.gameObject.SetActive(false); // Show the pause button
            _pauseCanvas.gameObject.SetActive(true);

            Time.timeScale = 0;

            OnPause?.Invoke();
        }

        private void Init()
        {
            _pauseCanvas.gameObject.SetActive(false);

            _pauseButton.onClick.AddListener(Pause);
            _resumeButton.onClick.AddListener(Resume);
        }



        private void Awake()
        {
            Init();
        }
    }
}
