using System.Collections;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.LevelHandling;
using Xonix.Entities;
using Xonix.Audio;
using Xonix.Grid;
using Xonix.UI;
using Xonix.Scenes;



namespace Xonix
{
    /// <summary>
    /// Class combiner for managing game elements
    /// </summary>
    public class XonixGame : MonoBehaviour
    {
        private const string GameOverSoundPath = "Audio/Game/GameOverSound";
        private const float GameOverDelaySeconds = 1f;

        public static event Action OnGameOver;


        [Header("--- Visual Elements ---")]
        [SerializeField] private PrintUIElements _printUI;
        [SerializeField] private Camera _mainCamera;

        [Header("--- Game Management ---")]
        [SerializeField] private XonixGrid _grid;
        [SerializeField] private LevelHandler _levelHandler;
        [SerializeField] private EntitiesHandler _entitiesHandler;


        private AudioClip _gameOverClip;

        private ScoreCounter _scoreCounter;



        private async void Init()
        {
            _scoreCounter = new ScoreCounter();

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

                _scoreCounter.AddScore(corruptedNodes.Count);
                _printUI.SetScoreNumber(_scoreCounter.Score);
            };

            #endregion


            #region [Print UI Init]

            _grid.OnSeaNodesPercentChange += _printUI.SetFillPercent;

            _printUI.SetLevelNumber(_levelHandler.CurrentLevel);
            _printUI.SetLivesNumber(_entitiesHandler.Player.Lives);
            _printUI.SetTimeSeconds(_levelHandler.TimeLeft);

            _entitiesHandler.Player.OnLivesCountChanged += _printUI.SetLivesNumber;
            _levelHandler.OnLevelChanged += _printUI.SetLevelNumber;

            #endregion


            #region [Level Handler Init]

            _grid.OnSeaNodesPercentChange += _levelHandler.CheckForLevelComplete;

            _levelHandler.LevelEndTimer.OnTimerEnded += EndGame;
            _levelHandler.LevelEndTimer.OnTickPassed += () => _printUI.SetTimeSeconds(_levelHandler.TimeLeft);

            #endregion

            var gameOverSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(GameOverSoundPath).Task;

            await gameOverSoundLoadingTask;

            _gameOverClip = gameOverSoundLoadingTask.Result;

            /// TODO: Не вызывается при выходе из уровня
            /// Нужно чтобы как минимум вызывалось обновление счёта;
            OnGameOver += _scoreCounter.TryToUpdateRecord;
        }

        private void EndGame()
        {
            OnGameOver?.Invoke();

            AudioManager2D.PlaySound(_gameOverClip);

            StartCoroutine(GameOverCoroutine());
        }

        private IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSeconds(GameOverDelaySeconds);

            SceneLoader.LoadMainMenu();
        }



        private void Awake()
        {
            Init();
        }

        private void OnApplicationQuit()
        {
            EndGame();
        }
    }
}
