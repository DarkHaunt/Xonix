using System.Collections.Generic;
using System;
using Xonix.Entities.Enemies;
using Xonix.Entities.Player;
using Xonix.PlayerInput;
using Xonix.Entities;
using Xonix.Grid;
using UnityEngine;



namespace Xonix
{
    using static EnemyBehaviour;

    public class XonixGame : MonoBehaviour
    {
        private static XonixGame _instance;

        private const int StartCountOfSeaEnemies = 3;
        // A percent of corrupted sea field, when level will be completed
        private const float TargetFiledCorruptionPercent = 0.05f;

        public static event Action OnFieldReload;
        public static event Action OnPlayerLoseLevel;


        [SerializeField] private XonixGrid _grid;
        [SerializeField] private FourDirectionInputTranslator _inputSystem;

        [SerializeField] private Sprite _seaEnemySprite;
        [SerializeField] private Sprite _earthEnemySprite;
        [SerializeField] private Sprite _playerSprite;

        // TODO: Create enemy spawner
        private EnemyFactory _enemeyFactory;

        private List<Enemy> _seaEnemies;
        private Player _player;

        [SerializeField] private int _score = 0;
        [SerializeField] private int _levelNumber = 0;



        public static IList<Enemy> SeaEnemies => _instance._seaEnemies;
        public static Vector2 PlayerPosition => _instance._player.Position;



        public static void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID_API
            Application.Quit();
#endif
        }

        public static void PlayerLoseLevel()
        {
            OnFieldReload?.Invoke();
            OnPlayerLoseLevel?.Invoke();
        }

        private void CheckForLevelComplete(float currentSeaCorruptionPercent)
        {
            if (currentSeaCorruptionPercent >= TargetFiledCorruptionPercent)
            {
                print("Reload");
                IncreaseLevel();
            }
        }

        private void IncreaseLevel()
        {
            _levelNumber++;

            OnFieldReload?.Invoke();

            _grid.ResetSeaField();

            var newEnemyPosition = _grid.GetRandomSeaFieldNodePosition();
            var newEnemy = _enemeyFactory.SpawnEnemy(EnemyType.SeaEnemy, newEnemyPosition, _seaEnemySprite);
            _seaEnemies.Add(newEnemy);

            OnFieldReload += () =>
            {
                newEnemy.transform.position = _grid.GetRandomSeaFieldNodePosition();
            };
        }

        private void SpawnEnemies()
        {
            var enemiesCount = StartCountOfSeaEnemies + _levelNumber;

            _seaEnemies = new List<Enemy>(enemiesCount);

            for (int i = 0; i < enemiesCount; i++)
            {
                var enemyPosition = _grid.GetRandomSeaFieldNodePosition();
                var enemy = _enemeyFactory.SpawnEnemy(EnemyType.SeaEnemy, enemyPosition, _seaEnemySprite);

                OnFieldReload += () => 
                { 
                    enemy.transform.position = _grid.GetRandomSeaFieldNodePosition(); 
                };

                _seaEnemies.Add(enemy);
            }

            // For one aerth enemy 
            var earthEnemyPosition = _grid.GetFieldBottomCenterPosition();
            var earthEnemy = _enemeyFactory.SpawnEnemy(EnemyType.EarthEnemy, earthEnemyPosition, _earthEnemySprite);

            OnFieldReload += () =>
            {
                earthEnemy.transform.position = _grid.GetFieldBottomCenterPosition();
            };
        }

        private void SpawnPlayer()
        {
            var playerPosition = _grid.GetFieldTopCenterPosition();

            _player = new GameObject($"Player").AddComponent<Player>();
            _player.Init(_inputSystem, playerPosition, _playerSprite);

            OnFieldReload += () =>
            {
                _player.StopMoving();
                _player.transform.position = _grid.GetFieldTopCenterPosition();
            };

            _player.OnNodesCountCorrupted += (int nodesCount) => _score += nodesCount;
            _player.OnNodeZoneCorrupted += _grid.RemoveSeaNodes;
        }

        private void Init()
        {
            _enemeyFactory = new EnemyFactory();

            SpawnPlayer();
            SpawnEnemies();

            _grid.OnSeaNodesPercentChange += CheckForLevelComplete;
        }

        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
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
