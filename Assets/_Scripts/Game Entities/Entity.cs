using UnityEngine;
using System.Collections;



namespace Xonix.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        private const float MoveTimeDelaySeconds = 0.03f;
        private static readonly YieldInstruction MoveTimeYield = new WaitForSeconds(MoveTimeDelaySeconds);

        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Vector2 _startPosition; // First position after initialization



        public Vector2 Position => transform.position;



        public virtual void Init(Vector2 initPosition, Sprite sprite)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = sprite;

            transform.position = initPosition;
            _startPosition = initPosition;

            StartCoroutine(MoveCoroutine());
        }

        protected abstract void Move();

        public void ResetPosition()
        {
            transform.position = _startPosition;
        }

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
