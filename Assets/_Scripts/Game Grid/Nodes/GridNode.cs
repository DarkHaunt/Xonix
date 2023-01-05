using UnityEngine;



namespace Xonix.Grid
{
    using NodeState = GridNodeSource.NodeState;

    [RequireComponent(typeof(SpriteRenderer))]
    public class GridNode : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private GridNodeSource _nodeSource;



        public Vector2 Position => transform.position;
        public NodeState State => _nodeSource.State;



        public void SetSource(GridNodeSource newNodeSource)
        {
            _nodeSource = newNodeSource;
            _spriteRenderer.sprite = _nodeSource.NodeSprite;
            _spriteRenderer.material = _nodeSource.Material;
        }



        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    } 
}
