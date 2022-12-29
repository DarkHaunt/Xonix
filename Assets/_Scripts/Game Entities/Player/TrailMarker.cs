using System.Collections.Generic;
using Xonix.Grid;



namespace Xonix.Trail
{
    using static StaticData;

    /// <summary>
    /// Marks tiles as trail one
    /// </summary>
    public class TrailMarker
    {
        private readonly Dictionary<GridNode, Direction> _nodesDirections = new Dictionary<GridNode, Direction>();
        private readonly GridNodeSource _trailNodeSource;



        public TrailMarker(GridNodeSource trailNodeSource)
        {
            _trailNodeSource = trailNodeSource;
        }


        public IReadOnlyDictionary<GridNode, Direction> TrailNodesDirections => _nodesDirections;



        public void MarkNodeAsTrail(GridNode gridNode, Direction nodeDireactio)
        {
            _nodesDirections.Add(gridNode, nodeDireactio);
            gridNode.SetSource(_trailNodeSource);
        }

        public void ResetTrail()
        {
            _nodesDirections.Clear();
        }
    } 
}
