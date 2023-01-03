using System.Collections.Generic;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.Player
{
    /// <summary>
    /// Marks tiles as trail ones
    /// </summary>
    public class TrailMarker
    {
        private readonly Dictionary<GridNode, Vector2> _nodesDirections = new Dictionary<GridNode, Vector2>();
        private readonly GridNodeSource _trailNodeSource;


        /// <summary>
        /// Key - Nodes
        /// <para>
        /// Values - Directions from where this node were entered
        /// </para>
        /// </summary>
        public IReadOnlyDictionary<GridNode, Vector2> TrailNodesDirections => _nodesDirections;



        public TrailMarker(GridNodeSource trailNodeSource)
        {
            _trailNodeSource = trailNodeSource;
        }



        public void MarkNodeAsTrail(GridNode gridNode, Vector2 nodeWalkDireaction)
        {
            _nodesDirections.Add(gridNode, nodeWalkDireaction);
            gridNode.SetSource(_trailNodeSource);
        }

        public void ClearTrail() => _nodesDirections.Clear();
    } 
}
