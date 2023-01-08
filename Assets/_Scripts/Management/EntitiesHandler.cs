using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Xonix.Entities.EnemyComponents;
using Xonix.LevelHandling;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static EnemyBounceBehaviour;

    public class EntitiesHandler : MonoBehaviour
    {
        private const int IndexOfFirstSeaEnemy = 1; // For sea enemy collection get
        private const int StartCountOfSeaEnemies = 3;
        private const float EntitiesMoveTimeDelaySeconds = 0.03f;

        public event Action OnEnemyTouchPlayer;


        private EntitySpawner _entitySpawner;
        private List<Enemy> _enemies;
        private Player _player;

        private LevelHandler _levelHandler;

        private Timer _entitiesMovingDelayTimer;



        public Player Player => _player;

        public IEnumerable<Enemy> SeaEnemies
        {
            get
            {
                for (int i = IndexOfFirstSeaEnemy; i < _enemies.Count; i++)
                    yield return _enemies[i];
            }
        }



        public async Task InitAsync(XonixGrid grid, LevelHandler levelHandler)
        {
            _levelHandler = levelHandler;
            _entitySpawner = new EntitySpawner(grid);

            await _entitySpawner.InitAsync();

            _enemies = new List<Enemy>(StartCountOfSeaEnemies + 1); // Sea enemies + one earth enemy;
            _entitiesMovingDelayTimer = new Timer(EntitiesMoveTimeDelaySeconds);

            await SpawnPlayer();

            SpawnEnemy(EnemyType.EarthEnemy);

            for (int i = 0; i < StartCountOfSeaEnemies; i++)
                SpawnEnemy(EnemyType.SeaEnemy);


            LevelHandler.OnLevelCompleted += () => SpawnEnemy(EnemyType.SeaEnemy);
            OnEnemyTouchPlayer += _levelHandler.LoseLevel;


            _entitiesMovingDelayTimer.OnTickPassed += MoveAllEntities;
#pragma warning disable CS4014 // “ак как этот вызов не ожидаетс€, выполнение существующего метода продолжаетс€ до тех пор, пока вызов не будет завершен
            _entitiesMovingDelayTimer.Start();
#pragma warning restore CS4014 // “ак как этот вызов не ожидаетс€, выполнение существующего метода продолжаетс€ до тех пор, пока вызов не будет завершен
        }

        private async Task SpawnPlayer()
        {
            var playerSpawnTask = _entitySpawner.SpawnPlayer(SeaEnemies);
            await playerSpawnTask;

            _player = playerSpawnTask.Result;
            _player.OnTrailNodeStepped += _levelHandler.LoseLevel;

            XonixGame.OnGameOver += _player.StopMoving;
            LevelHandler.OnLevelLosen += _player.StopMoving;
            LevelHandler.OnLevelCompleted += _player.StopMoving;
        }

        private Enemy SpawnEnemy(EnemyType type)
        {
            var enemy = _entitySpawner.SpawnEnemy(type);
            _enemies.Add(enemy);

            XonixGame.OnGameOver += enemy.StopMoving;
            enemy.OnTrailNodeStepped += _levelHandler.LoseLevel;

            return enemy;
        }

        private void MoveAllEntities()
        {
            _player.Move();

            foreach (var entity in _enemies)
            {
                entity.Move();

                if (entity.Position == _player.Position)
                    OnEnemyTouchPlayer?.Invoke();
            }
        }
    }
}
