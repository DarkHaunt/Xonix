using Xonix.Entities.EnemyComponents;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static StaticRandomizer;
    using static EnemyBounceBehaviour;
    using static GridNodeSource;

    public class Enemy : Entity
    {
        private EnemyBounceBehaviour _bounceBehaviour;



        public override void MoveIntoNode(GridNode node)
        {
            if (_bounceBehaviour.IsNodeHasBorderState(node))
                SetMoveDirection(_bounceBehaviour.GetBounceDirection(Position, MoveDirection));

            transform.Translate(MoveTranslation);

            if (node.State == NodeState.Trail)
                NotifyThatSteppedOnTrail();
        }

        protected override void OnOutField() => SetMoveDirection(_bounceBehaviour.GetBounceDirection(Position, MoveDirection));

        public void Init(EnemyType type, Vector2 initPosition, Sprite sprite, XonixGrid grid)
        {
            _bounceBehaviour = new EnemyBounceBehaviour(type, grid);

            Init(initPosition, sprite, grid);
        }

        public override void Init(Vector2 initPosition, Sprite sprite, XonixGrid grid)
        {
            var randomMoveDirection = new Vector2(GetRandomSign(), GetRandomSign());
            SetMoveDirection(randomMoveDirection);

            base.Init(initPosition, sprite, grid);
        }
    }
}
