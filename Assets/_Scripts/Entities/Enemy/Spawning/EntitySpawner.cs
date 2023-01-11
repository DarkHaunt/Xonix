using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xonix.Entities.EnemyComponents;
using Xonix.Grid;



namespace Xonix.Entities
{
    /// <summary>
    /// A spawner for all types of entities in the game
    /// </summary>
    public class EntitySpawner
    {
        private const string SeaEnemeySpritePath = "Sprites/Entities/Enemies/SeaEnemy";
        private const string EarthEnemeySpritePath = "Sprites/Entities/Enemies/EarthEnemy";
        private const string PlayerSpritePath = "Sprites/Entities/Player";

        private readonly XonixGrid _grid;

        private Sprite _seaEnemySprite;
        private Sprite _earthEnemySprite;
        private Sprite _playerSprite;



        /// <summary>
        /// Needs grid for entities initialization
        /// </summary>
        /// <param name="grid"></param>
        public EntitySpawner(XonixGrid grid)
        {
            _grid = grid;
        }



        public Enemy SpawnEnemy(EnemyType type)
        {
            var spawnData = GetEnemySpawnData(type);

            var enemy = new GameObject($"{spawnData.Behavior}").AddComponent<Enemy>();
            enemy.Init(spawnData , _grid);

            return enemy;
        }

        public async Task<Player> SpawnPlayer(IEnumerable<Enemy> seaEnemies)
        {
            var player = new GameObject($"Player").AddComponent<Player>();

            await player.InitAsync(_playerSprite, _grid, seaEnemies);

            return player;
        }

        public async Task InitAsync()
        {
            var seaEnemySpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(SeaEnemeySpritePath).Task;
            var earthEnemySpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(EarthEnemeySpritePath).Task;
            var playerSpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(PlayerSpritePath).Task;

            await Task.WhenAll(seaEnemySpriteLoadingTask, earthEnemySpriteLoadingTask, playerSpriteLoadingTask);

            _seaEnemySprite = seaEnemySpriteLoadingTask.Result;
            _earthEnemySprite = earthEnemySpriteLoadingTask.Result;
            _playerSprite = playerSpriteLoadingTask.Result;
        }

        private EnemySpawnData GetEnemySpawnData(EnemyType type)
        {
            return type switch
            {
                EnemyType.SeaEnemy => new EnemySpawnData(_seaEnemySprite, new SeaEnemyBehavior(_grid)),

                EnemyType.EarthEnemy => new EnemySpawnData(_earthEnemySprite, new EarthEnemyBehavior(_grid)),

                _ => throw new UnityException($"EnemySpawner can't spawn enemy of type {type}!"),
            };
        }



        public struct EnemySpawnData
        {
            public readonly Sprite Sprite;
            public readonly EnemyBehaviour Behavior;


            public EnemySpawnData(Sprite sprite, EnemyBehaviour behaviour)
            {
                Sprite = sprite;
                Behavior = behaviour;
            }
        }

        public enum EnemyType
        {
            SeaEnemy,
            EarthEnemy,
        }
    }
}
