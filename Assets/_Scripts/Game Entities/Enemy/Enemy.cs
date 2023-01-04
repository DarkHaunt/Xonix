using Xonix.Entities.Enemies;
using UnityEngine;



namespace Xonix.Entities
{
    using static StaticRandomizer;
    using static EnemyBehaviour;

    public class Enemy : Entity
    {
        private EnemyBehaviour _enemyBehaviour;



        protected override void Move()
        {
            var newMoveDirection = _enemyBehaviour.GetMoveDirection(Position, MoveDirection, Position + MoveTranslation);
            SetMoveDirection(newMoveDirection);

            transform.Translate(MoveTranslation);

            if (Position == XonixGame.PlayerPosition)
            {
                print("Enemy touched player");
                XonixGame.ReloadLevel();
            }
        }

        public void Init(EnemyType type, Vector2 initPosition, Sprite sprite)
        {
            _enemyBehaviour = new EnemyBehaviour(type);

            Init(initPosition, sprite);
        }

        public override void Init(Vector2 initPosition, Sprite sprite)
        {
            var randomMoveDirection = new Vector2(GetRandomSign(), GetRandomSign());
            SetMoveDirection(randomMoveDirection);

            base.Init(initPosition, sprite);
        }
    }
}
