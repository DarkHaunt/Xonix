using Xonix.Entities.Enemies;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static StaticRandomizer;
    using static EnemyBehaviour;
    using static GridNodeSource;

    public class Enemy : Entity
    {
        private EnemyBehaviour _enemyBehaviour;



        public override void Move(GridNode node)
        {
            if (_enemyBehaviour.IsNodeHasBorderState(node))
                SetMoveDirection(_enemyBehaviour.GetBounceDirection(Position, MoveDirection));

            transform.Translate(MoveTranslation);

            if (node.State == NodeState.Trail)
                StepOnTrailNode();
        }

        public override void OnOutOfField() => SetMoveDirection(_enemyBehaviour.GetBounceDirection(Position, MoveDirection));

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
