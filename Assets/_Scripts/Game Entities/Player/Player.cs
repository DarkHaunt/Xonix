using System.Collections.Generic;
using System.Linq;
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

        // Marks tiles as trailed
        private readonly Dictionary<GridNode, Direction> _nodesDirections = new Dictionary<GridNode, Direction>();

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
            foreach (var node in _nodesDirections.Keys)
            {
                if (firstNode == null)
                    firstNode = node;

                _corrupter.CorruptNode(node);
                
                /*            print($"Node " + nodesDirection.Key + "   Direction " + nodesDirection.Value);
                            print($"Directions Perpendiculars  " + GetDirectionPerpendicularDirections(nodesDirection.Value));*/
            }

           // GetCorruptedPolygon(firstNode.Position, firstNode.Position + Vector2.left);
            GetCorruptedPolygon(firstNode.Position + Vector2.left);

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

        private List<Vector2> GetCorruptedPolygon(Vector2 seedPosition)
        {
            var polygon = new List<Vector2>();



            _corrupter.CorruptZone(seedPosition, XonixGame.SeaEnemies);
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
    }
}
