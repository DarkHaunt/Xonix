using Xonix.Entities.Enemies;
using UnityEngine;



namespace Xonix.Entities
{
    using static StaticData;
    using static EnemyBehaviour;

    public class Enemy : Entity
    {
        [SerializeField] private EnemyBehaviour _enemyBehaviour;
        private Vector2 _currentDirection;



        public override void Init(Vector2 initPosition, Sprite sprite)
        {
            _currentDirection = new Vector2(GetRandomSign(), GetRandomSign());

            base.Init(initPosition, sprite);
        }

        public void SetBehavior(EnemyType enemyType)
        {
            _enemyBehaviour = new EnemyBehaviour(enemyType);
        }

        protected override void Move()
        {
            _currentDirection = _enemyBehaviour.GetMoveTranslation(Position, _currentDirection);

            transform.Translate(_currentDirection * CellSize);

            if (Position == XonixGame.PlayerPosition)
            {
                print("Enemy touched player");
                XonixGame.PlayerLoseLevel();
            }
        }
    }
}
