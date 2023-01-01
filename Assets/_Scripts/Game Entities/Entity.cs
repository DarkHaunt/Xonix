using UnityEngine;
using System.Collections;



namespace Xonix.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        private const float MoveTimeDelaySeconds = 0.02f;
        private static readonly YieldInstruction MoveTimeYield = new WaitForSeconds(MoveTimeDelaySeconds);

        [SerializeField] private SpriteRenderer _spriteRenderer;



        public Vector2 Position => transform.position;



        public abstract void Init();

        protected abstract void Move();

        /// <summary>
        /// Controlls properly move speed of the entity
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveCoroutine()
        {
            while (true)
            {
                Move();

                yield return MoveTimeYield;
            }
        }



        private void Awake()
        {
            Init();

            StartCoroutine(MoveCoroutine());
        }
    }
}
