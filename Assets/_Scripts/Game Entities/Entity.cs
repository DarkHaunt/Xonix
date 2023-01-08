using UnityEngine;
using Xonix.Grid;
using System;



namespace Xonix.Entities
{
    using static XonixGrid;
    using static GridNodeSource;

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



        public abstract void MoveIntoNode(GridNode node);

        protected abstract void OnOutField();

        public void Move()
        {
            if (!_grid.TryToGetNodeWithPosition(NextPosition, out GridNode node))
            {
                OnOutField();
                return;
            }

            MoveIntoNode(node);
        }

        protected void NotifyThatSteppedOnTrail() => OnTrailNodeStepped?.Invoke();

        public void StopMoving() => SetMoveDirection(Vector2.zero);

        public virtual void Init(Vector2 initPosition, Sprite sprite, XonixGrid grid)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;
            _grid = grid;

            transform.position = initPosition;
        }

        protected void SetMoveDirection(Vector2 newDirection) => _moveDirection = newDirection;
    }
}
