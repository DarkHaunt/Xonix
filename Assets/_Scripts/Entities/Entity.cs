using System;
using UnityEngine;
using Xonix.LevelHandling;
using Xonix.Grid;



namespace Xonix.Entities
{
    using static XonixGrid;

    /// <summary>
    /// Base class of all entities in the game
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        public event Action OnTrailNodeStepped;

        private XonixGrid _grid;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _moveDirection;


        public Vector2 Position => transform.position;
        protected Vector2 MoveTranslation => _moveDirection * CellSize;
        protected Vector2 MoveDirection => _moveDirection;
        private Vector2 NextPosition => Position + MoveTranslation;



        protected abstract void MoveIntoNode(GridNode node);

        protected abstract void OnOutField();

        protected abstract void ResetPosition();

        public virtual void Init(Vector2 initPosition, Sprite sprite, XonixGrid grid)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            _grid = grid;

            transform.position = initPosition;

            LevelHandler.OnLevelCompleted += ResetPosition;
        }

        public void Move()
        {
            if (!_grid.TryToGetNodeWithPosition(NextPosition, out GridNode node))
            {
                OnOutField();
                return;
            }

            MoveIntoNode(node);
        }

        public void StopMoving() => SetMoveDirection(Vector2.zero);

        protected void SetMoveDirection(Vector2 newDirection) => _moveDirection = newDirection;

        protected void NotifyThatSteppedOnTrail() => OnTrailNodeStepped?.Invoke();
    }
}
