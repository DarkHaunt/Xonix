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



        private Direction MoveDirection
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

                // TODO: For test
                return Direction.Top;

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

            transform.position = movePosition;
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

            CorruptZonesAttachedToTrail();
        }

        private void CorruptZonesAttachedToTrail()
        {
            // Corrupt all trail nodes for first
            _corrupter.CorruptNodes(_trailMarker.TrailNodesDirections.Keys);

            // Init a collection for remembering checked nodes
            var checkedNodePositions = new HashSet<Vector2>();


            foreach (var nodeKeyDirectionValue in _trailMarker.TrailNodesDirections)
            {
                var nodeDirection = GetDirectionValue(nodeKeyDirectionValue.Value);

                var firstNeighborNodePosition = nodeKeyDirectionValue.Key.Position + nodeDirection.RotateFor90DegreeClockwise();
                var secondNeighborNodePosition = nodeKeyDirectionValue.Key.Position + nodeDirection.RotateFor90DegreeCounterClockwise();

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
