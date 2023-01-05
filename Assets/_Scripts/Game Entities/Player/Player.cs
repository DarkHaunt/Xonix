using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Xonix.PlayerInput;
using Xonix.Grid;



namespace Xonix.Entities.Players
{
    using static GridNodeSource;

    public class Player : Entity
    {
        public event Action<ISet<GridNode>> OnNodesCorrupted;
        public event Action OnLifesEnd;

        private const int StartLifesCount = 3;

        private TrailMarker _trailMarker;
        private Corrupter _corrupter;

        private int _lifesCount = StartLifesCount;
        private bool _isTrailing = false;



        public int Lifes => _lifesCount;



        public async void Init(FourDirectionInputTranslator inputTranslator, Vector2 initPosition, Sprite sprite)
        {
            InitMovingSystem(inputTranslator);
            Init(initPosition, sprite);
            
            _trailMarker = new TrailMarker();
            _corrupter = new Corrupter();

            await Task.WhenAll(_trailMarker.InitTrailSource(), _corrupter.InitNodeSourcesForCorruptionAsync());

            XonixGame.OnLevelReloading += DecreaseLifeCount;
            XonixGame.OnLevelReloading += () =>
            {
                _isTrailing = false;

                // Set trail marked nodes as non-disturbed
                _corrupter.DecorruptNodes(_trailMarker.TrailNodesDirections.Keys);
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

            XonixGame.ReloadLevel();
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
            _corrupter.CorruptNodes(_trailMarker.TrailNodesDirections.Keys);

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

            // Notify for each node, that has been corrupted
            OnNodesCorrupted?.Invoke(corruptedNodes);

            // Delete all trail data
            _trailMarker.ClearTrail();
        }

        private void InitMovingSystem(FourDirectionInputTranslator inputTranslator)
        {
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
        }

        private void DecreaseLifeCount()
        {
            _lifesCount--;
            print("Pla " + _lifesCount);

            if (_lifesCount == 0)
                OnLifesEnd?.Invoke();
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
