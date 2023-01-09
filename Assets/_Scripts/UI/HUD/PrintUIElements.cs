using UnityEngine;
using TMPro;



namespace Xonix.UI
{
    /// <summary>
    /// All printed text UI elements in the game
    /// </summary>
    public class PrintUIElements : MonoBehaviour
    {
        [Header("--- Text Fields ---")]
        [SerializeField] private TextMeshProUGUI _scoreNumber;
        [SerializeField] private TextMeshProUGUI _levelNumber;
        [SerializeField] private TextMeshProUGUI _lifesNumber;
        [SerializeField] private TextMeshProUGUI _fillPercent;
        [SerializeField] private TextMeshProUGUI _timeLeftSeconds;



        public void SetScoreNumber(int score) => _scoreNumber.text = $"{score}";
        public void SetLivesNumber(int lifesNumber) => _lifesNumber.text = $"{lifesNumber}";
        public void SetLevelNumber(int levelNumber) => _levelNumber.text = $"{levelNumber}";
        public void SetTimeSeconds(float seconds) => _timeLeftSeconds.text = $"{(int)seconds}";
        public void SetFillPercent(float percent) => _fillPercent.text = $"{(int)(percent * 100)}%";
    } 
}
