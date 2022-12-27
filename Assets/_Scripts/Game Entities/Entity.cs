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



        public abstract Vector3 MoveDirection { get; }



        protected virtual void Move() => transform.Translate(MoveDirection * CellSize);

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
            StartCoroutine(MoveCoroutine());
        }

    }
}
