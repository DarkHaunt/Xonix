using System.Collections.Generic;
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
        private readonly Dictionary<GridNode, Direction> _nodesDirections = new Dictionary<GridNode, Direction>();

        private TrailMarker _trailMarker;
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
            _nodesDirections.Add(seaNode, MoveDirection);
            _isTrailing = true;
            _trailMarker.MarkNodeAsTrail(seaNode);
        }

        private void OnEarthNodeStep()
        {
            if (!_isTrailing)
                return;

            _isTrailing = false;

            GridNode firstNode = null;
            foreach (var nodesDirection in _nodesDirections)
            {
                if (firstNode == null)
                    firstNode = nodesDirection.Key;

                /*            print($"Node " + nodesDirection.Key + "   Direction " + nodesDirection.Value);
                            print($"Directions Perpendiculars  " + GetDirectionPerpendicularDirections(nodesDirection.Value));*/
            }

            GetCorruptedPolygon(firstNode.Position, firstNode.Position + Vector2.left);

            _nodesDirections.Clear();
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

        private List<Vector2> GetCorruptedPolygon(Vector2 startNodePosition, Vector2 firstPolygonNodePosition)
        {
            var polygon = new List<Vector2>();

            var trackman = new Trackman(startNodePosition, firstPolygonNodePosition);

            /*
                        do
                        {
                            var faceNodePosition = trackman.Position + trackman.WalkDirection * CellSize;

                            if (!XonixGame.TryToGetNodeWithPosition(faceNodePosition, out GridNode faceNode))
                                throw new UnityException($"Node with position {faceNodePosition} doesn't exist");

                            if (faceNode.State == NodeState.Earth)



                        } while (trackman.Position != startNodePosition);
            */
            return polygon;
        }




        private class NodePolygon
        {
            public readonly IReadOnlyList<GridNode> _vertices;


            public NodePolygon(IList<GridNode> vertices)
            {
                _vertices = vertices as IReadOnlyList<GridNode>;
            }
        }

        private struct Trackman
        {
            private Vector2 _leftHandPosition;
            private Vector2 _position;

            private Vector2 _currentWalkDirection;
            private Vector2 _leftHandDirectionRelativelyToPosition;


            public Vector2 Position => _position;
            public Vector2 LeftHandPosition => _leftHandPosition;
            public Vector2 WalkDirection => _currentWalkDirection;



            public Trackman(Vector2 leftHandPosition, Vector2 position)
            {
                _leftHandPosition = leftHandPosition;
                _position = position;

                _leftHandDirectionRelativelyToPosition = (_leftHandPosition - _position).normalized;

                var walkDirection = (_position - _leftHandPosition).normalized;
                _currentWalkDirection = walkDirection.RotateVector2CounterClockwise();
            }



            public override string ToString() => $"Position = {Position} Hand Position = {LeftHandPosition} Nad dir {_leftHandDirectionRelativelyToPosition} Move direction = {WalkDirection}";

            public void RotateClockwise()
            {
                _currentWalkDirection = _currentWalkDirection.RotateVector2Clockwise();
                _leftHandDirectionRelativelyToPosition = _leftHandDirectionRelativelyToPosition.RotateVector2Clockwise();

                _leftHandPosition = _position + (_leftHandDirectionRelativelyToPosition * CellSize);
            }

            public void RotateCounterClockwise()
            {
                _currentWalkDirection = _currentWalkDirection.RotateVector2CounterClockwise();
                _leftHandDirectionRelativelyToPosition = _leftHandDirectionRelativelyToPosition.RotateVector2CounterClockwise();

                _leftHandPosition = _position + (_leftHandDirectionRelativelyToPosition * CellSize);
            }
        }
    }
}
