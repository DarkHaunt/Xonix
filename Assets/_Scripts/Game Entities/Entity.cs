using UnityEngine;
using Xonix.Grid;
using System;



namespace Xonix.Entities
{
    using static XonixGrid;

    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        public event Action OnTrailNodeStepped;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Vector2 _moveDirection;



        public Vector2 Position => transform.position;
        public Vector2 NextPosition => Position + MoveTranslation;
        protected Vector2 MoveTranslation => _moveDirection * CellSize;
        protected Vector2 MoveDirection => _moveDirection;



        public abstract void Move(GridNode node);

        public abstract void OnOutOfField();

        public virtual void Init(Vector2 initPosition, Sprite sprite)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;

            transform.position = initPosition;
        }

        protected void StepOnTrailNode() => OnTrailNodeStepped?.Invoke();

        public void StopMoving() => SetMoveDirection(Vector2.zero);

        protected void SetMoveDirection(Vector2 newDirection) => _moveDirection = newDirection;
    }
}
