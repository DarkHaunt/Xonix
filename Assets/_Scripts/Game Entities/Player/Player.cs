using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.PlayerInput;
using Xonix.Grid;



namespace Xonix.Entities.Player
{
    using static GridNodeSource;
    using static StaticData;

    public class Player : Entity
    {
        private const int StartLifesCount = 3;
        private const string EarthNodeSource = "Grid/NodeSource/EarthNodeSource";
        private const string TrailNodeSource = "Grid/NodeSource/TrailNodeSource";


        private TrailMarker _trailMarker;
        private Corrupter _corrupter;

        private Vector2 _moveDirection;

        private int _lifesCount = StartLifesCount;
        [SerializeField] private bool _isTrailing = false;



        public async void Init(FourDirectionInputTranslator inputTranslator, Vector2 initPosition, Sprite sprite)
        {
            var earthNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(EarthNodeSource).Task;
            var trailNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(TrailNodeSource).Task;

            // Set direction relevant to holded button
            inputTranslator.UpArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.up);
            inputTranslator.DownArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.down);
            inputTranslator.LeftArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.left);
            inputTranslator.RightArrowButton.OnHoldStart += () => SetMoveDirection(Vector2.right);

            // On unhiold stop movement
            inputTranslator.UpArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.DownArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.LeftArrowButton.OnHoldEnd += StopMoving;
            inputTranslator.RightArrowButton.OnHoldEnd += StopMoving;

            Init(initPosition, sprite);

            await Task.WhenAll(earthNodeSourceLoadingTask, trailNodeSourceLoadingTask);

            _trailMarker = new TrailMarker(trailNodeSourceLoadingTask.Result);
            _corrupter = new Corrupter(earthNodeSourceLoadingTask.Result);

            XonixGame.OnPlayerLoseLevel += DecreaseLifeCount;
            XonixGame.OnPlayerLoseLevel += () =>
            {
                _isTrailing = false;

                _trailMarker.ResetTrail();
            };
        }

        protected override void Move()
        {
            if (_moveDirection == Vector2.zero)
                return;

            var movePosition = Position + (_moveDirection * CellSize);

            // If out of field
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
                    return; // Shouldn't walk if losed
                default:
                    break;
            }

            transform.position = movePosition;
        }

        private void OnSeaNodeStep(GridNode seaNode)
        {
            _isTrailing = true;
            _trailMarker.MarkNodeAsTrail(seaNode, _moveDirection);
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

            // Init a collection for remembering checked nodes
            var checkedNodePositions = new HashSet<Vector2>();


            foreach (var nodeKeyDirectionValue in _trailMarker.TrailNodesDirections)
            {
                var node = nodeKeyDirectionValue.Key;
                var nodeWalkDirection = nodeKeyDirectionValue.Value;


                var firstNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeClockwise();

                if (!checkedNodePositions.Contains(firstNeighborNodePosition))
                    corruptedNodesCount += _corrupter.CorruptZone(firstNeighborNodePosition, checkedNodePositions);

                var secondNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeCounterClockwise();

                if (!checkedNodePositions.Contains(secondNeighborNodePosition))
                    corruptedNodesCount += _corrupter.CorruptZone(secondNeighborNodePosition, checkedNodePositions);
            }

            XonixGame.AddScore(corruptedNodesCount);
            // Delete all trail data
            _trailMarker.ResetTrail();
        }

        private void DecreaseLifeCount()
        {
            _lifesCount--;

            if (_lifesCount == 0)
                XonixGame.EndGame();
        }

        private void SetMoveDirection(Vector2 newMoveDirection) => _moveDirection = newMoveDirection;

        private void StopMoving() => _moveDirection = Vector2.zero;


        #region [TEST INPUT SYSTEM]

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                _moveDirection = Vector2.up;
                return;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _moveDirection = Vector2.down;
                return;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _moveDirection = Vector2.left;
                return;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _moveDirection = Vector2.right;
                return;
            }


            _moveDirection = Vector2.zero;
            
        } 

        #endregion
    }
}
