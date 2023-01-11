using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Xonix.Scenes;



namespace Xonix.UI
{
    public class MainMenu : MonoBehaviour
    {
        private const string GameScenePath = "Scenes/Game";


        [Header("--- Buttons ---")]
        [SerializeField] private Button _playButton;

        [SerializeField] private TextMeshProUGUI _recordScoreNumberField;



        private void Init()
        {
            _playButton.onClick.AddListener(() => SceneLoader.LoadSceneWithTransition(GameScenePath, LoadSceneMode.Single));

            _recordScoreNumberField.text = $"{ScoreCounter.GetRecordScore()}";
        }



        private void Awake()
        {
            Init();
        }
    }
}
