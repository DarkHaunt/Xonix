using System.Collections;
using System;
using Xonix.LevelHandling;
using Xonix.Entities;
using Xonix.Audio;
using Xonix.Grid;
using Xonix.UI;
using UnityEngine.AddressableAssets;
using UnityEngine;



namespace Xonix
{
    public class XonixGame : MonoBehaviour
    {
        private const string GameOverSoundPath = "Audio/Game/GameOverSound";
        private const float GameOverDelaySeconds = 2f;

        public static event Action OnGameOver;


        [Header("--- Visual Elements ---")]
        [SerializeField] private PrintUIElements _printUI;
        [SerializeField] private Camera _mainCamera;

        [Header("--- Game Management ---")]
        [SerializeField] private XonixGrid _grid;
        [SerializeField] private LevelHandler _levelHandler;
        [SerializeField] private EntitiesHandler _entitiesHandler;

        private int _score = 0;



        private async void Init()
        {
            // Camera alignment
            _mainCamera.transform.position = _grid.GetGridCenter();
            _mainCamera.transform.position += new Vector3(0f, 0f, -10f);

            await _levelHandler.InitAsync();

            await _entitiesHandler.InitAsync(_grid, _levelHandler);

            #region [Entities Init]

            _entitiesHandler.Player.OnLivesEnd += EndGame;
            _entitiesHandler.Player.OnNodesCorrupted += (corruptedNodes) =>
            {
                _grid.RemoveSeaNodes(corruptedNodes);
                _score += corruptedNodes.Count;
                _printUI.SetScoreNumber(_score);
            };

            #endregion

            _grid.OnSeaNodesPercentChange += _printUI.SetFillPercent;
            _grid.OnSeaNodesPercentChange += _levelHandler.CheckForLevelComplete;

            _levelHandler.LevelEndTimer.OnTimerEnded += EndGame;
            _levelHandler.LevelEndTimer.OnTickPassed += () => _printUI.SetTimeSeconds(_levelHandler.TimeLeft);

            #region [Print UI Init]

            _printUI.SetLevelNumber(_levelHandler.CurrentLevel);
            _printUI.SetLivesNumber(_entitiesHandler.Player.Lives);
            _printUI.SetTimeSeconds(_levelHandler.TimeLeft);

            LevelHandler.OnLevelCompleted += () => _printUI.SetLevelNumber(_levelHandler.CurrentLevel);
            LevelHandler.OnLevelLosen += () => _printUI.SetLivesNumber(_entitiesHandler.Player.Lives);
            LevelHandler.OnLevelCompleted += () => _printUI.SetFillPercent(0f);

            #endregion

            var gameOverSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(GameOverSoundPath).Task;

            await gameOverSoundLoadingTask;

            OnGameOver += () => SoundManager.PlayClip(gameOverSoundLoadingTask.Result);
        }

        private void EndGame()
        {
            OnGameOver?.Invoke();

            StartCoroutine(GameOverCoroutine());
        }

        private IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSeconds(GameOverDelaySeconds);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID_API
            Application.Quit();
#endif
        }



        private void Awake()
        {
            Init();
        }

#if UNITY_EDITOR
        // For correct testing
        private void OnApplicationQuit()
        {
            EndGame();
        }
#endif
    }
}
