using UnityEngine;



namespace Xonix.Entities.Enemies
{
    using static EnemyBehaviour;

    public class EnemyFactory
    {
        public EnemyFactory() { }



        public Enemy SpawnEnemy(EnemyType enemyType, Vector2 position, Sprite enemySprite)
        {
            var enemy = new GameObject($"{enemyType} Enemy").AddComponent<Enemy>();

            enemy.SetBehavior(enemyType);
            enemy.Init(position, enemySprite);

            return enemy;
        }
    }
}
