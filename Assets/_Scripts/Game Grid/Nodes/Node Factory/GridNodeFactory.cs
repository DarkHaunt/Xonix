using UnityEngine;



namespace Xonix.Grid
{
    /// <summary>
    /// Simple factory for node creation
    /// </summary>
    public class GridNodeFactory
    {
        public GridNodeFactory() { }



        public GridNode CreateGridNode(Vector2 nodePosition)
        {
            var newNode = new GameObject($"Node {nodePosition}").AddComponent<GridNode>();
            newNode.transform.position = nodePosition;
            return newNode;
        }
    }
}
