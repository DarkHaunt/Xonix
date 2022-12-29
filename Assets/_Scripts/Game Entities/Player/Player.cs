using System.Collections.Generic;
using Xonix.Trail;
using Xonix.Grid;
using UnityEngine;



namespace Xonix.Entities.Player
{
    using static GridNodeSource;
    using static StaticData;

    public class Player : Entity
    {
        [SerializeField] private GridNodeSource _trailNodeSource;
        [SerializeField] private GridNodeSource _earthNodeSource;


        private TrailMarker _trailMarker;
        private Corrupter _corrupter;
        private bool _isTrailing = false;



        public override Direction MoveDirection
        {
            get
            {
                #region [TEST INPUT SYSTEM]

                if (Input.GetKey(KeyCode.W))
                    return Direction.Top;

                if (Input.GetKey(KeyCode.S))
                    return Direction.Down;

                if (Input.GetKey(KeyCode.D))
                    return Direction.Right;

                if (Input.GetKey(KeyCode.A))
                    return Direction.Left;

                return Direction.Zero;

                #endregion
            }
        }



        public override void Init()
        {
            _trailMarker = new TrailMarker(_trailNodeSource);
            _corrupter = new Corrupter(_earthNodeSource);
        }

        protected override void Move()
        {
            var direction = GetDirectionValue(MoveDirection);
            var movePosition = Position + (direction * CellSize);

            // TEST
            if (MoveDirection == Direction.Zero)
                return;

            if (!XonixGame.TryToGetNodeWithPosition(movePosition, out GridNode node))
                return;

            switch (node.State)
            {
                case NodeState.Sea:
                    OnSeaNodeStep(node);
                    break;
                case NodeState.Earth:
                    OnEarthNodeStep();
                    break;
                case NodeState.Trail:
                    OnTrailNodeStep();
                    break;
                default:
                    break;
            }


            base.Move();
        }

        private void OnSeaNodeStep(GridNode seaNode)
        {
            _isTrailing = true;
            _trailMarker.MarkNodeAsTrail(seaNode, MoveDirection);
        }

        private void OnEarthNodeStep()
        {
            if (!_isTrailing)
                return;

            _isTrailing = false;


            // Corrupt trail node anyway
            _corrupter.CorruptNodes(_trailMarker.TrailNodesDirections.Keys);

            // Init a collection for remembering checked nodes
            var checkedNodePositions = new HashSet<Vector2>();

            foreach (var nodeDirection in _trailMarker.TrailNodesDirections)
            {
                var perpendicularDirections = GetDirectionPerpendicularDirections(nodeDirection.Value);

                var firstNeighborNodePosition = nodeDirection.Key.Position + perpendicularDirections.FirstDirectionPerpendicular;
                var secondNeighborNodePosition = nodeDirection.Key.Position + perpendicularDirections.SecondDirectionPerpendicular;

                _corrupter.CorruptZone(firstNeighborNodePosition, checkedNodePositions);
                _corrupter.CorruptZone(secondNeighborNodePosition, checkedNodePositions);
            }
        }

        private void OnTrailNodeStep()
        {
            print("End game");

            /*#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif

            #if UNITY_STANDALONE
                        Application.Quit();
            #endif*/
        }



        private class NodePolygon
        {
            public readonly IReadOnlyList<GridNode> _vertices;


            public NodePolygon(IList<GridNode> vertices)
            {
                _vertices = vertices as IReadOnlyList<GridNode>;
            }
        }
    }
}
