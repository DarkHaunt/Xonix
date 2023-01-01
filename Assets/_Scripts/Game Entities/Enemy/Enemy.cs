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


        public override void Init()
        {
            _enemyBehaviour = new EnemyBehaviour(EnemyType.SeaEnemy);

            // Because GetRandomSign returns or 1 or -1 it is perfectly sets random dioganal direction
            _currentDirection = new Vector2(GetRandomSign(), GetRandomSign());
        }

        protected override void Move()
        {
            _currentDirection = _enemyBehaviour.GetMoveTranslation(Position, _currentDirection);

            transform.Translate(_currentDirection * CellSize);
        }
    }
}
