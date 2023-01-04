using UnityEngine;
using System.Collections;



namespace Xonix.Entities
{
    using static StaticData;

    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        private const float MoveTimeDelaySeconds = 0.03f;
        private static readonly YieldInstruction MoveTimeYield = new WaitForSeconds(MoveTimeDelaySeconds);


        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Vector2 _moveDirection;



        public Vector2 Position => transform.position;
        public Vector2 MoveDirection => _moveDirection;
        public Vector2 MoveTranslation => _moveDirection * CellSize;



        protected abstract void Move();

        public virtual void Init(Vector2 initPosition, Sprite sprite)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;

            transform.position = initPosition;

            StartCoroutine(MoveCoroutine());
        }

        protected void SetMoveDirection(Vector2 newDirection) => _moveDirection = newDirection;

        public void StopMoving() => _moveDirection = Vector2.zero;

        /// <summary>
        /// Controlls properly move speed of the entity
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveCoroutine()
        {
            while (true)
            {
                yield return MoveTimeYield;

                Move();
            }
        }
    }
}
