using UnityEngine;



namespace Xonix.Grid
{
    [CreateAssetMenu(fileName = "Node Source", menuName = "Grid/NodeSource", order = 51)]
    public class GridNodeSource : ScriptableObject
    {
        [SerializeField] private Sprite _nodeSprite;
        [SerializeField] private NodeState _nodeState;



        public Sprite NodeSprite => _nodeSprite;
        public NodeState State => _nodeState;



        public enum NodeState
        {
            Sea = 0x001,
            Earth = 0x010,
            Trail = 0x100
        }
    }
}
