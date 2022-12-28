using System.Collections;
using UnityEngine;


namespace Xonix.Entities
{
    // Using CellSize as move step for properly move on the grid
    using static StaticData;

    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Entity : MonoBehaviour
    {
        private const float WalkDelaySeconds = 0.05f;
        private static readonly WaitForSeconds _walkCooldown = new WaitForSeconds(WalkDelaySeconds);


        [SerializeField] private SpriteRenderer _spriteRenderer;



        public Vector2 Position => transform.position;

        public abstract Direction MoveDirection { get; }



        public abstract void Init();

        protected virtual void Move()
        {
            var direction = GetDirectionValue(MoveDirection);

            transform.Translate(direction * CellSize);
        }

        private IEnumerator MoveCoroutine()
        {
            while (true)
            {
                Move();

                yield return _walkCooldown; 
            }
        }



        private void Start()
        {
            Init();

            StartCoroutine(MoveCoroutine());
        }

    }
}
