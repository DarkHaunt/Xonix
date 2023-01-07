using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.Players
{
    /// <summary>
    /// Marks tiles as trail ones
    /// </summary>
    public class TrailMarker
    {
        private const string TrailNodeSource = "Grid/NodeSource/TrailNodeSource";

        private readonly Dictionary<GridNode, Vector2> _nodesDirections = new Dictionary<GridNode, Vector2>();
        private GridNodeSource _trailNodeSource;

        
        
        /// <summary>
        /// Key - Nodes
        /// <para>
        /// Values - Directions from where this node were entered
        /// </para>
        /// </summary>
        public IReadOnlyDictionary<GridNode, Vector2> TrailNodesDirections => _nodesDirections;

        

        public TrailMarker() { }

        

        public async Task InitTrailSource()
        {
            var trailNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(TrailNodeSource).Task;

            await trailNodeSourceLoadingTask;

            _trailNodeSource = trailNodeSourceLoadingTask.Result;
        }

        public void MarkNodeAsTrail(GridNode gridNode, Vector2 nodeWalkDireaction)
        {
            _nodesDirections.Add(gridNode, nodeWalkDireaction);
            gridNode.SetSource(_trailNodeSource);
        }

        public void ClearTrail() => _nodesDirections.Clear();
    } 
}
