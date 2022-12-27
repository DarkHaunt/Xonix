using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Xonix.Grid
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridNode : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private GridNodeSource _nodeSource;



        public Vector2 Position => transform.position;



        public void SetSource(GridNodeSource newNodeSource)
        {
            _nodeSource = newNodeSource;
            _spriteRenderer.sprite = _nodeSource.NodeSprite;
        }



        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    } 
}
