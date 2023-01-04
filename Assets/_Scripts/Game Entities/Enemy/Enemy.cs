using Xonix.Entities.Enemies;
using UnityEngine;



namespace Xonix.Entities
{
    using static StaticData;
    using static EnemyBehaviour;

    public class Enemy : Entity
    {
        [SerializeField] private EnemyBehaviour _enemyBehaviour;


        protected override void Move()
        {
            var newMoveDirection = _enemyBehaviour.GetMoveDirection(Position, MoveDirection, Position + MoveTranslation);
            SetMoveDirection(newMoveDirection);

            transform.Translate(MoveTranslation);

            if (Position == XonixGame.PlayerPosition)
            {
                print("Enemy touched player");
                XonixGame.PlayerLoseLevel();
            }
        }

        public override void Init(Vector2 initPosition, Sprite sprite)
        {
            var randomMoveDirection = new Vector2(GetRandomSign(), GetRandomSign());
            SetMoveDirection(randomMoveDirection);

            base.Init(initPosition, sprite);
        }

        public void SetBehavior(EnemyType enemyType)
        {
            _enemyBehaviour = new EnemyBehaviour(enemyType);
        }
    }
}
