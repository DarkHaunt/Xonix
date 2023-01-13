using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Xonix.Scenes;
using Xonix.Audio;



namespace Xonix.UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string GameScenePath = "Scenes/Game";

        [Header("--- UI ---")]
        [SerializeField] private Button _playButton;

        [SerializeField] private TextMeshProUGUI _recordScoreNumberField;

        [Header("--- Audio ---")]
        [SerializeField] private AudioClip _mainMusic;



        private void Init()
        {
            AudioManager2D.PlayMusic(_mainMusic);

            _playButton.onClick.AddListener(() => SceneLoader.LoadSceneWithTransition(GameScenePath, LoadSceneMode.Single));

            _recordScoreNumberField.text = $"{ScoreCounter.GetRecordScore()}";
        }



        private void Awake()
        {
            Init();
        }
    }
}
