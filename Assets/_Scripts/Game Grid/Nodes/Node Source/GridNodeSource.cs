using UnityEngine;



namespace Xonix.Grid
{
    /// <summary>
    /// The init data for nodes
    /// </summary>
    [CreateAssetMenu(fileName = "Node Source", menuName = "Grid/NodeSource", order = 51)]
    public class GridNodeSource : ScriptableObject
    {
        [SerializeField] private Material _nodeMaterial;
        [SerializeField] private Sprite _nodeSprite;
        [SerializeField] private NodeState _nodeState = NodeState.Trail;



        public Sprite NodeSprite => _nodeSprite;
        public NodeState State => _nodeState;
        public Material Material => _nodeMaterial;



        public enum NodeState
        {
            Sea = 0x001,
            Earth = 0x010,
            Trail = 0x100
        }
    }
}
