using UnityEngine;



namespace Xonix.Grid
{
    [CreateAssetMenu(fileName = "Node Source", menuName = "Grid/NodeSource", order = 51)]
    public class GridNodeSource : ScriptableObject
    {
        [SerializeField] private Sprite _nodeSprite;

        public Sprite NodeSprite => _nodeSprite;
    } 
}
