using UnityEngine;



namespace Xonix.Grid
{
    /// <summary>
    /// An init data for nodes in the game
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
            Sea,
            Earth,
            Trail
        }
    }
}
