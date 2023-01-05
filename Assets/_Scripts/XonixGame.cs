using System.Collections.Generic;
using System.Collections;
using System;
using Xonix.Entities.Enemies;
using Xonix.Entities.Players;
using Xonix.PlayerInput;
using Xonix.Entities;
using Xonix.Grid;
using Xonix.UI;
using UnityEngine;



namespace Xonix
{
    using static EnemyBehaviour;

    public class XonixGame : MonoBehaviour
    {
        private static XonixGame _instance;

        private const int StartCountOfSeaEnemies = 3;
        private const float TargetFiledCorruptionPercent = 0.05f; // A percent of corrupted sea field, when level will be completed
        private const float GameEndTime = 90; // One and a half hour of max game time

        private readonly YieldInstruction OneMinuteYield = new WaitForSeconds(60f); // Cached for optimization

        public static event Action OnFieldReload;
        public static event Action OnLevelReloading;


        [SerializeField] private XonixGrid _grid;
        [SerializeField] private FourDirectionInputTranslator _inputSystem;
        [SerializeField] private GamePrintUI _printUI;
        [SerializeField] private Camera _mainCamera;


        private EntitySpawner _entitySpawner;

        private List<Enemy> _seaEnemies;
        private Player _player;

        [SerializeField] private int _score = 0;
        [SerializeField] private int _levelNumber = 1;
        [SerializeField] private float _minutesForLevelLeft = GameEndTime;



        public static IList<Enemy> SeaEnemies => _instance._seaEnemies;
        public static Vector2 PlayerPosition => _instance._player.Position;



        private void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID_API
            Application.Quit();
#endif
        }

        public static void ReloadLevel()
        {
            OnLevelReloading?.Invoke();
            OnFieldReload?.Invoke();
        }

        private void CheckForLevelComplete(float currentSeaCorruptionPercent)
        {
            _printUI.SetFillPercent(currentSeaCorruptionPercent);

            if (currentSeaCorruptionPercent >= TargetFiledCorruptionPercent)
            {
                print("Level Done");
                IncreaseLevel();
            }
        }

        private async void Init()
        {
            _grid.OnSeaNodesPercentChange += CheckForLevelComplete;

            _entitySpawner = new EntitySpawner(_grid);
            _seaEnemies = new List<Enemy>(StartCountOfSeaEnemies);

            await _entitySpawner.Init();

            SpawnEarthEnemy();

            for (int i = 0; i < StartCountOfSeaEnemies; i++)
                SpawnSeaEnemy();

            SpawnPlayer();

            _mainCamera.transform.position = _grid.GetGridCenter();
            _mainCamera.transform.position += new Vector3(0f, 0f, -10f);

            StartCoroutine(LevelTimerCoroutine());

            _printUI.SetTimeSeconds(_minutesForLevelLeft);
            _printUI.SetLevelNumber(_levelNumber);
            _printUI.SetLifesNumber(_player.Lifes);
        }

        private void IncreaseLevel()
        {
            _levelNumber++;

            _printUI.SetLevelNumber(_levelNumber);
            _printUI.SetFillPercent(0f);

            OnFieldReload?.Invoke();

            SpawnSeaEnemy();
        }

        private void SpawnSeaEnemy()
        {
            var seaEnemy = _entitySpawner.SpawnEnemy(EnemyType.SeaEnemy);
            _seaEnemies.Add(seaEnemy);

            OnFieldReload += () =>
            {
                seaEnemy.transform.position = _grid.GetRandomSeaFieldNodePosition();
            };
        }

        private void SpawnPlayer()
        {
            _player = _entitySpawner.SpawnPlayer(_inputSystem);

            OnFieldReload += () =>
            {
                _player.StopMoving();
                _player.transform.position = _grid.GetFieldTopCenterPosition();
                _printUI.SetLifesNumber(_player.Lifes);
            };

            _player.OnNodesCorrupted += (corruptedNodes) =>
            {
                _grid.RemoveSeaNodes(corruptedNodes);
                _score += corruptedNodes.Count;
                _printUI.SetScoreNumber(_score);
            };

            _player.OnLifesEnd += EndGame;
        }

        private void SpawnEarthEnemy()
        {
            var earthEnemy = _entitySpawner.SpawnEnemy(EnemyType.EarthEnemy);

            OnFieldReload += () =>
            {
                earthEnemy.transform.position = _grid.GetFieldBottomCenterPosition();
            };
        }

        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
        }

        private IEnumerator LevelTimerCoroutine()
        {
            while (_minutesForLevelLeft > 0)
            {
                yield return OneMinuteYield;

                _minutesForLevelLeft -= 1f;
                _printUI.SetTimeSeconds(_minutesForLevelLeft);
            }

            EndGame();
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
    }
}
