using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Lean.Touch;
using Xonix.Entities.PlayerComponents;
using Xonix.LevelHandling;
using Xonix.Audio;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static GridNodeSource;

    public class Player : Entity
    {
        private const string DeathSoundPath = "Audio/Player/DeathSound";

        private const int StartLifesCount = 3;


        public event Action<ISet<GridNode>> OnNodesCorrupted;
        public event Action OnLivesEnd;
        public event Action<int> OnLivesCountChanged;

        private TrailMarker _trailMarker;
        private Corrupter _corrupter;

        private AudioClip _deathClip;
        private Vector2 _initPosition;

        private int _livesCount = StartLifesCount;
        private bool _isTrailing = false;



        public int Lives => _livesCount;



        public async Task InitAsync(Sprite sprite, XonixGrid grid, IEnumerable<Enemy> seaEnemeies)
        {
            _initPosition = grid.GetFieldTopCenterPosition();

            base.Init(_initPosition, sprite, grid);

            _trailMarker = new TrailMarker();
            _corrupter = new Corrupter(grid, seaEnemeies);

            var deathSoundLoadingTask = Addressables.LoadAssetAsync<AudioClip>(DeathSoundPath).Task;

            await Task.WhenAll(_trailMarker.InitTrailSource(), _corrupter.InitAsync(), deathSoundLoadingTask);

            _deathClip = deathSoundLoadingTask.Result;
        }

        protected override void MoveIntoNode(GridNode node)
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
                    NotifyThatSteppedOnTrail();
                    return; // Don't move if stepped on trail
                default:
                    break;
            }

            transform.Translate(MoveTranslation);
        }

        protected override void ResetPosition() => transform.position = _initPosition;

        protected override void OnOutField()
        {
            StopMoving();
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

        private void CancelTrailing()
        {
            _isTrailing = false;

            // Set trail marked nodes as non-disturbed
            _corrupter.DecorruptNodes(_trailMarker.TrailNodesDirections.Keys);
            _trailMarker.ClearTrail();
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
                    corruptedNodes.UnionWith(_corrupter.GetCorruptedNodes(firstNeighborNodePosition, checkedNodePositions));

                var secondNeighborNodePosition = node.Position + nodeWalkDirection.RotateFor90DegreeCounterClockwise();

                if (!checkedNodePositions.Contains(secondNeighborNodePosition))
                    corruptedNodes.UnionWith(_corrupter.GetCorruptedNodes(secondNeighborNodePosition, checkedNodePositions));
            }

            // Notify for each node, that has been corrupted
            OnNodesCorrupted?.Invoke(corruptedNodes);

            // Delete all trail data
            _trailMarker.ClearTrail();
        }

        private void DecreaseLivesCount()
        {
            _livesCount--;

            OnLivesCountChanged?.Invoke(_livesCount);

            if (_livesCount == 0)
            {
                OnLivesEnd?.Invoke();
                Destroy(gameObject);
                return;
            }

            AudioManager2D.PlaySound(_deathClip);
        }

        /// <summary>
        /// Sets player direction according to current swiped finger
        /// </summary>
        /// <param name="finger"></param>
        private void SetSwipedMoveDirection(LeanFinger finger)
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



        protected override void OnEnable()
        {
            base.OnEnable();

            LevelHandler.OnLevelLosen += ResetPosition;
            LevelHandler.OnLevelLosen += DecreaseLivesCount;
            LevelHandler.OnLevelLosen += CancelTrailing;
            LevelHandler.OnLevelLosen += StopMoving;

            LevelHandler.OnLevelCompleted += StopMoving;

            LeanTouch.OnFingerSwipe += SetSwipedMoveDirection;
            LeanTouch.OnFingerTap += PlayerStopMoving;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LevelHandler.OnLevelLosen -= ResetPosition;
            LevelHandler.OnLevelLosen -= DecreaseLivesCount;
            LevelHandler.OnLevelLosen -= CancelTrailing;
            LevelHandler.OnLevelLosen -= StopMoving;

            LevelHandler.OnLevelCompleted -= StopMoving;

            LeanTouch.OnFingerSwipe -= SetSwipedMoveDirection;
            LeanTouch.OnFingerTap -= PlayerStopMoving;
        }
    }
}
