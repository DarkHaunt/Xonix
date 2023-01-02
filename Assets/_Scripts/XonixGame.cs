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
        private const int InitCountOfSeaEnemies = 3;


        public event Action OnFieldReload;

        [SerializeField] private XonixGrid _grid;
        [SerializeField] private FourDirectionInputTranslator _inputSystem;

        [SerializeField] private Sprite _seaEnemySprite;
        [SerializeField] private Sprite _earthEnemySprite;
        [SerializeField] private Sprite _playerSprite;

        private EnemyFactory _enemeyFactory;

        private List<Enemy> _seaEnemies;
        private Player _player;

        private int _score = 0;
        private int _levelNumber = 0;



        public static IList<Enemy> SeaEnemies => _instance._seaEnemies;

        public static Vector2 PlayerPosition => _instance._player.Position;



        /// <summary>
        /// Tries to get grid node with parameter position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool TryToGetNodeWithPosition(Vector2 position, out GridNode node)
        {
            return _instance._grid.TryToGetNode(position, out node);
        }
        // TODO: Должен быть не конец игры, а проигрыш игрока на текущем уровне
        public static void PlayerLose() => _instance.OnFieldReload?.Invoke();

        private void GameOver()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID_API
            Application.Quit();
#endif
        }

        private void Init()
        {
            _instance.OnFieldReload += GameOver;

            _enemeyFactory = new EnemyFactory();

            SpawnPlayer();
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            var enemiesCount = InitCountOfSeaEnemies + _levelNumber;

            _seaEnemies = new List<Enemy>(enemiesCount);

            for (int i = 0; i < enemiesCount; i++)
            {
                var enemyPosition = _grid.GetRandomSeaFieldNodePosition();
                var enemy = _enemeyFactory.SpawnEnemy(EnemyType.SeaEnemy, enemyPosition, _seaEnemySprite);
                OnFieldReload += enemy.ResetPosition;

                _seaEnemies.Add(enemy);
            }

            // For one aerth enemy 
            var earthEnemyPosition = _grid.GetFieldBottomCenterPosition();
            var earthEnemy = _enemeyFactory.SpawnEnemy(EnemyType.EarthEnemy, earthEnemyPosition, _earthEnemySprite);

            OnFieldReload += earthEnemy.ResetPosition;
        }

        private void SpawnPlayer()
        {
            var playerPosition = _grid.GetFieldTopCenterPosition();

            _player = new GameObject($"Player").AddComponent<Player>();
            _player.Init(_inputSystem, playerPosition, _playerSprite);

            OnFieldReload += _player.ResetPosition;
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
