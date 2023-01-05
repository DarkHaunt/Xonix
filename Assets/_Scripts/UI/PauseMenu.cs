using UnityEngine;
using UnityEngine.UI;



namespace Xonix.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Canvas _pauseCanvas;

        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;



        private void Resume()
        {
            _pauseCanvas.gameObject.SetActive(false);
            Time.timeScale = 1;
        }        
        
        private void Pause()
        {
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
