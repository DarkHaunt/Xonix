using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.PlayerInput;
using Xonix.Grid;



namespace Xonix.Entities.Player
{
    using static GridNodeSource;

    public class Player : Entity
    {
        public event Action<IEnumerable<GridNode>> OnNodeZoneCorrupted;
        public event Action<int> OnNodesCountCorrupted;

        private const int StartLifesCount = 3;

        private const string EarthNodeSource = "Grid/NodeSource/EarthNodeSource";
        private const string SeaNodeSource = "Grid/NodeSource/SeaNodeSource";
        private const string TrailNodeSource = "Grid/NodeSource/TrailNodeSource";


        private TrailMarker _trailMarker;
        private Corrupter _corrupter;

        private int _lifesCount = StartLifesCount;
        [SerializeField] private bool _isTrailing = false;



        public async void Init(FourDirectionInputTranslator inputTranslator, Vector2 initPosition, Sprite sprite)
        {
            var earthNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(EarthNodeSource).Task;
            var trailNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(TrailNodeSource).Task;
            var seaNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(SeaNodeSource).Task;

            // Set direction relevant to holded button
            inputTranslator.UpArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.up);
            inputTranslator.DownArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.down);
            inputTranslator.LeftArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.left);
            inputTranslator.RightArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.right);

            // On unhold stop movement
            inputTranslator.UpArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.DownArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.LeftArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.RightArrowButton.OnHoldEnd += StopMoving;

            Init(initPosition, sprite);

            await Task.WhenAll(earthNodeSourceLoadingTask, trailNodeSourceLoadingTask, seaNodeSourceLoadingTask);

            _trailMarker = new TrailMarker(trailNodeSourceLoadingTask.Result);
            _corrupter = new Corrupter(earthNodeSourceLoadingTask.Result, seaNodeSourceLoadingTask.Result);

            XonixGame.OnPlayerLoseLevel += DecreaseLifeCount;
            XonixGame.OnPlayerLoseLevel += () =>
            {
                _isTrailing = false;

                // Set trail marked nodes as non-disturbed
                _corrupter.ReleaseNodes(_trailMarker.TrailNodesDirections.Keys);
                _trailMarker.ClearTrail();
            };
        }

        protected override void Move()
        {
            if (MoveDirection == Vector2.zero)
                return;

            var nextPosition = Position + MoveTranslation;

            // If out of field
            if (!XonixGame.TryToGetNodeWithPosition(nextPosition, out GridNode node))
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
                    return; // Shouldn't walk if losed
                default:
                    break;
            }

            transform.Translate(MoveTranslation);
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

        private void OnTrailNodeStep()
        {
            print("Player loses because stepped on his trail");

            XonixGame.PlayerLoseLevel();
        }

        /// <summary>
        /// Corrupts all zones, that attached to current trail
        /// <para>
        /// Zone shouldn't have any enemy inside to be corrupted
        /// </para>
        /// </summary>
        private void CorruptZonesAttachedToTrail()
        {
            // Corrupt all trail nodes for first
            var corruptedNodesCount = _corrupter.CorruptNodes(_trailMarker.TrailNodesDirections.Keys);
            var corruptedNodes = new HashSet<GridNode>(_trailMarker.TrailNodesDirections.Keys);

            // Init a collection for remembering checked nodes
            var checkedNodePositions = new HashSet<Vector2>();

            foreach (var nodeKeyDirectionValue in _trailMarker.TrailNodesDirections)
            {
                var node = nodeKeyDirectionValue.Key;
                var nodeWalkDirection = nodeKeyDirectionValue.Value;


                var firstNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeClockwise();

                if (!checkedNodePositions.Contains(firstNeighborNodePosition))
                    corruptedNodes.UnionWith(_corrupter.CorruptZone(firstNeighborNodePosition, checkedNodePositions));

                var secondNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeCounterClockwise();

                if (!checkedNodePositions.Contains(secondNeighborNodePosition))
                    corruptedNodes.UnionWith(_corrupter.CorruptZone(secondNeighborNodePosition, checkedNodePositions));
            }

            // Invoke all data update events
            OnNodesCountCorrupted?.Invoke(corruptedNodes.Count);
            OnNodeZoneCorrupted?.Invoke(corruptedNodes);

            // Delete all trail data
            _trailMarker.ClearTrail();
        }

        private void DecreaseLifeCount()
        {
            _lifesCount--;

            if (_lifesCount == 0)
                XonixGame.EndGame();
        }



        #region [TEST INPUT SYSTEM]

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                SetMoveDirection(Vector2.up);
                return;
            }

            if (Input.GetKey(KeyCode.S))
            {
                SetMoveDirection(Vector2.down);
                return;
            }

            if (Input.GetKey(KeyCode.A))
            {
                SetMoveDirection(Vector2.left);
                return;
            }

            if (Input.GetKey(KeyCode.D))
            {
                SetMoveDirection( Vector2.right);
                return;
            }


            SetMoveDirection(Vector2.zero);

        }

        #endregion
    }
}
