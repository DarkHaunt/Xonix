using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Xonix.LevelHandling;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static EntitySpawner;

    /// <summary>
    /// Processes entity spawning and moving
    /// </summary>
    public class EntitiesHandler : MonoBehaviour
    {
        private const int IndexOfFirstSeaEnemy = 1; // For sea enemy collection get
        private const int StartCountOfSeaEnemies = 1;
        private const float EntitiesMoveTimeDelaySeconds = 0.03f;


        private LevelHandler _levelHandler;

        private EntitySpawner _entitySpawner;
        private List<Enemy> _enemies;
        private Player _player;

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
        }

        private void SpawnSeaEnemy() => SpawnEnemy(EnemyType.SeaEnemy);

        private void SpawnEnemy(EnemyType type)
        {
            var enemy = _entitySpawner.SpawnEnemy(type);
            _enemies.Add(enemy);

            enemy.OnTrailNodeStepped += _levelHandler.LoseLevel;
        }

        private void MoveAllEntities()
        {
            _player.Move();

            foreach (var entity in _enemies)
            {
                entity.Move();

                if (entity.Position == _player.Position)
                    _levelHandler.LoseLevel();
            }
        }



        private void OnEnable()
        {
            LevelHandler.OnLevelCompleted += SpawnSeaEnemy;
        }

        private void OnDisable()
        {
            LevelHandler.OnLevelCompleted -= SpawnSeaEnemy;
        }
    }
}
