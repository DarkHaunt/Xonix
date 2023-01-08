using System.Threading.Tasks;
using System.Collections.Generic;
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
        private static XonixGame _instance;

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

        private IEnumerable<Enemy> _enemies;
        private int _score = 0;


        public static IEnumerable<Enemy> SeaEnemies => _instance._enemies;



        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNodeWithPosition(position, out node);
        }

        private async void Init()
        {
            // Camera alignment
            _mainCamera.transform.position = _grid.GetGridCenter();
            _mainCamera.transform.position += new Vector3(0f, 0f, -10f);

            await Task.WhenAll(_levelHandler.InitAsync(), _entitiesHandler.InitAsync(_grid));

            _enemies = _entitiesHandler.SeaEnemies;

            #region [Entities Init]

            _entitiesHandler.EarthEnemy.OnTrailNodeStepped += _levelHandler.LoseLevel;

            foreach (var enemy in _entitiesHandler.SeaEnemies)
                enemy.OnTrailNodeStepped += _levelHandler.LoseLevel;

            _entitiesHandler.Player.OnTrailNodeStepped += _levelHandler.LoseLevel;
            _entitiesHandler.Player.OnLivesEnd += EndGame;
            _entitiesHandler.Player.OnNodesCorrupted += (corruptedNodes) =>
            {
                _grid.RemoveSeaNodes(corruptedNodes);
                _score += corruptedNodes.Count;
                _printUI.SetScoreNumber(_score);
            };

            #endregion

            _grid.OnSeaNodesPercentChange += _levelHandler.CheckForLevelComplete;

            _levelHandler.LevelEndTimer.OnTimerEnded += EndGame;
            _levelHandler.LevelEndTimer.OnTickPassed += () => _printUI.SetTimeSeconds(_levelHandler.TimeLeft);

            #region [Print UI Init]

            _grid.OnSeaNodesPercentChange += _printUI.SetFillPercent;

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
            #region [Singleton]

            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            #endregion

            Init();
        }

#if UNITY_EDITOR
        // TEST
        private void OnApplicationQuit()
        {
            EndGame();
        }
#endif
    }
}
