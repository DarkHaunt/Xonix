using UnityEngine;
using Xonix.Grid;
using Xonix.Trail;



namespace Xonix.Entities
{
    using static GridNodeSource;
    using static StaticData;

    public class Player : Entity
    {
        [SerializeField] private GridNodeSource _trailNodeSource;

        // Marks tiles as trailed
        private TrailMarker _trailMarker;


        public override Vector3 MoveDirection
        {
            get
            {
                #region [TEST INPUT SYSTEM]

                if (Input.GetKey(KeyCode.W))
                    return Vector2.up;

                if (Input.GetKey(KeyCode.S))
                    return Vector2.down;

                if (Input.GetKey(KeyCode.D))
                    return Vector2.right;

                if (Input.GetKey(KeyCode.A))
                    return Vector2.left;

                return Vector2.zero;

                #endregion
            }
        }



        public override void Init()
        {
            _trailMarker = new TrailMarker(_trailNodeSource);
        }

        protected override void Move()
        {
            var movePosition = transform.position + (MoveDirection * CellSize);

            if (!XonixGame.TryToGetNodeWithPosition(movePosition, out GridNode node))
                return;

            if (node.State == NodeState.Sea)
                _trailMarker.MarkNodeAsTrail(node);

            base.Move();
        }
    }
}
