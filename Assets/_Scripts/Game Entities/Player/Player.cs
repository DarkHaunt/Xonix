using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Lean.Touch;
using Xonix.Audio;
using Xonix.Grid;



namespace Xonix.Entities.Players
{
    using static GridNodeSource;

    public class Player : Entity
    {
        private const string DeathSoundPath = "Audio/Player/DeathSound";
        private const int StartLifesCount = 3;


        public event Action<ISet<GridNode>> OnNodesCorrupted;
        public event Action OnLivesEnd;

        private TrailMarker _trailMarker;
        private Corrupter _corrupter;

        private AudioClip _deathClip;

        private int _lifesCount = StartLifesCount;
        private bool _isTrailing = false;



        public int Lifes => _lifesCount;



        public async Task InitAsync(Vector2 initPosition, Sprite sprite)
        {
            base.Init(initPosition, sprite);

            _trailMarker = new TrailMarker();
            _corrupter = new Corrupter();

            var deathSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(DeathSoundPath).Task;

            await Task.WhenAll(_trailMarker.InitTrailSource(), _corrupter.InitNodeSourcesForCorruptionAsync(), deathSoundLoadingTask);

            _deathClip = deathSoundLoadingTask.Result;

            XonixGame.OnLevelLosen += DecreaseLifeCount;
            XonixGame.OnLevelReloaded += () =>
            {
                _isTrailing = false;

                // Set trail marked nodes as non-disturbed
                _corrupter.DecorruptNodes(_trailMarker.TrailNodesDirections.Keys);
                _trailMarker.ClearTrail();
            };

            XonixGame.OnGameOver += OnDisable;
        }

        public override void Move(GridNode node)
        {
            // For optimization
            if (MoveDirection == Vector2.zero)
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

        public override void OnOutOfField() => StopMoving();

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

            StepOnTrailNode();
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
                    corruptedNodes.UnionWith(_corrupter.GetCorruptedZone(firstNeighborNodePosition, checkedNodePositions));

                var secondNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeCounterClockwise();

                if (!checkedNodePositions.Contains(secondNeighborNodePosition))
                    corruptedNodes.UnionWith(_corrupter.GetCorruptedZone(secondNeighborNodePosition, checkedNodePositions));
            }

            // Notify for each node, that has been corrupted
            OnNodesCorrupted?.Invoke(corruptedNodes);

            // Delete all trail data
            _trailMarker.ClearTrail();
        }

        private void DecreaseLifeCount()
        {
            _lifesCount--;

            if (_lifesCount == 0)
            {
                OnLivesEnd?.Invoke();
                return;
            }

            SoundManager.PlayClip(_deathClip);
        }

        private void GetSwipedMoveDirection(LeanFinger finger)
        {
            var direction = (finger.LastScreenPosition - finger.StartScreenPosition).normalized;


            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.y = 0;
                direction.x = Mathf.Round(direction.x);
            }
            else
            {
                direction.x = 0;
                direction.y = Mathf.Round(direction.y);
            }

            SetMoveDirection(direction);
        }

        private void PlayerStopMoving(LeanFinger finger) => StopMoving();



        private void OnEnable()
        {
            LeanTouch.OnFingerSwipe += GetSwipedMoveDirection;
            LeanTouch.OnFingerTap += PlayerStopMoving;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerSwipe -= GetSwipedMoveDirection;
            LeanTouch.OnFingerTap -= PlayerStopMoving;
        }
    }
}
