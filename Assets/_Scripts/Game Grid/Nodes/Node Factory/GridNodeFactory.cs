using UnityEngine;



namespace Xonix.Grid
{
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
