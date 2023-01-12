using System;
using UnityEngine.UI;
using UnityEngine;
using Xonix.Scenes;



namespace Xonix
{
    public class PauseMenu : MonoBehaviour
    {
        public static event Action OnPause;
        public static event Action OnResume;


        [Header("--- Canvas ---")]
        [SerializeField] private Canvas _pauseCanvas;

        [Header("--- Buttons ---")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _exitButton;



        private void Init()
        {
            _pauseCanvas.gameObject.SetActive(false);

            _pauseButton.onClick.AddListener(Pause);
            _resumeButton.onClick.AddListener(Resume);
            _exitButton.onClick.AddListener(ExitToMainMenu);
        }

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

        private void ExitToMainMenu()
        {
            _pauseButton.gameObject.SetActive(true); // Hide the pause button

            Resume();
            SceneLoader.LoadMainMenu();
        }

        private void CheckForOnFocusPause(bool isFocused)
        {
            if (!isFocused) // If player losesd application focus - pause the game
                Pause();
        }




        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            Application.focusChanged += CheckForOnFocusPause;
        }

        private void OnDisable()
        {
            Application.focusChanged -= CheckForOnFocusPause;
        }
    }
}
