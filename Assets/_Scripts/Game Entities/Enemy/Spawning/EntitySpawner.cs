using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Xonix.Entities.Players;
using Xonix.Entities.Enemies;
using Xonix.PlayerInput;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static EnemyBehaviour;

    /// <summary>
    /// A spawner for all types of entities in the game
    /// </summary>
    public class EntitySpawner
    {
        private const string SeaEnemeySpritePath = "Sprites/Entities/Enemies/SeaEnemy";
        private const string EarthEnemeySpritePath = "Sprites/Entities/Enemies/EarthEnemy";
        private const string PlayerSpritePath = "Sprites/Entities/Player";

        private readonly XonixGrid _grid;

        private Dictionary<EnemyType, Sprite> SpritesOfEnemyWithType;
        private Dictionary<EnemyType, Func<Vector2>> GetPositionOfEnemyWithType;

        private Sprite _seaEnemySprite;
        private Sprite _earthEnemySprite;
        private Sprite _playerSprite;



        /// <summary>
        /// Needs grid for calculate positions of entities
        /// </summary>
        /// <param name="grid"></param>
        public EntitySpawner(XonixGrid grid)
        {
            _grid = grid;
        }



        public Enemy SpawnEnemy(EnemyType type)
        {
            var enemyPosition = GetPositionOfEnemyWithType[type].Invoke();
            var enemySprite = SpritesOfEnemyWithType[type];

            var enemy = new GameObject($"{type}").AddComponent<Enemy>();
            enemy.Init(type, enemyPosition, enemySprite);

            return enemy;
        }

        public Player SpawnPlayer(FourDirectionInputTranslator inputSystem)
        {
            var playerPosition = _grid.GetFieldTopCenterPosition();

            var player = new GameObject($"Player").AddComponent<Player>();
            player.Init(inputSystem, playerPosition, _playerSprite);

            return player;
        }

        public async Task Init()
        {
            var seaEnemySpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(SeaEnemeySpritePath).Task;
            var earthEnemySpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(EarthEnemeySpritePath).Task;
            var playerSpriteLoadingTask = Addressables.LoadAssetAsync<Sprite>(PlayerSpritePath).Task;

            await Task.WhenAll(seaEnemySpriteLoadingTask, earthEnemySpriteLoadingTask, playerSpriteLoadingTask);

            _seaEnemySprite = seaEnemySpriteLoadingTask.Result;
            _earthEnemySprite = earthEnemySpriteLoadingTask.Result;
            _playerSprite = playerSpriteLoadingTask.Result;


            SpritesOfEnemyWithType = new Dictionary<EnemyType, Sprite>(2)
            {
                [EnemyType.SeaEnemy] = _seaEnemySprite,
                [EnemyType.EarthEnemy] = _earthEnemySprite,
            };

            GetPositionOfEnemyWithType = new Dictionary<EnemyType, Func<Vector2>>(2)
            {
                [EnemyType.SeaEnemy] = _grid.GetRandomSeaFieldNodePosition,
                [EnemyType.EarthEnemy] = _grid.GetFieldBottomCenterPosition
            };
        }
    }
}