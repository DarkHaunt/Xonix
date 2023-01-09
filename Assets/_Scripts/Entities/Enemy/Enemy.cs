using Xonix.Entities.EnemyComponents;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static StaticRandomizer;
    using static GridNodeSource;

    public class Enemy : Entity
    {
        private EnemyBehaviour _behaviour;



        public void Init(EntitySpawner.EnemySpawnData spawnData, XonixGrid grid)
        {
            _behaviour = spawnData.Behavior;

            Init(_behaviour.GetInitPosition(), spawnData.Sprite, grid);
        }

        public override void Init(Vector2 initPosition, Sprite sprite, XonixGrid grid)
        {
            var randomMoveDirection = new Vector2(GetRandomSign(), GetRandomSign());
            SetMoveDirection(randomMoveDirection);

            base.Init(initPosition, sprite, grid);
        }

        protected override void MoveIntoNode(GridNode node)
        {
            if (_behaviour.IsNodeHasBorderState(node))
                SetMoveDirection(_behaviour.GetBounceDirection(Position, MoveDirection));

            transform.Translate(MoveTranslation);

            if (node.State == NodeState.Trail)
                NotifyThatSteppedOnTrail();
        }

        protected override void OnOutField() => SetMoveDirection(_behaviour.GetBounceDirection(Position, MoveDirection));

        protected override void ResetPosition() => transform.position = _behaviour.GetInitPosition();
    }
}
