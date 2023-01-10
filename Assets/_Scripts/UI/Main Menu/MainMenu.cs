using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Xonix.Scenes;
using UnityEngine.SceneManagement;



namespace Xonix.UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string GameScenePath = "Scenes/Game";

        [Header("--- Buttons ---")]
        [SerializeField] private Button _playButton;

        [SerializeField] private TextMeshProUGUI _maxScoreNumberField;



        private void Init()
        {
            _playButton.onClick.AddListener(() => SceneLoader.LoadSceneWithTransition(GameScenePath, LoadSceneMode.Single));
        }



        private void Awake()
        {
            Init();
        }
    }
}
