using UnityEngine;
using UnityEngine.UI;



namespace Xonix.UI
{
    public class PauseMenu : MonoBehaviour
    {
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
        }        
        
        private void Pause()
        {
            _pauseButton.gameObject.SetActive(false); // Show the pause button
            _pauseCanvas.gameObject.SetActive(true);

            Time.timeScale = 0;
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
