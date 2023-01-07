using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.Players
{
    using static XonixGrid;

    /// <summary>
    /// Marks zone without enemy by the 
    /// </summary>
    public class Corrupter
    {
        private const string EarthNodeSource = "Grid/NodeSource/EarthNodeSource";
        private const string SeaNodeSource = "Grid/NodeSource/SeaNodeSource";

        private readonly Vector2[] _neighboursTemplates = new Vector2[4] // Convenient node neighbours position
        {
            new Vector2(0f, CellSize),
            new Vector2(0f, -CellSize),
            new Vector2(CellSize, 0f),
            new Vector2(-CellSize, 0f)
        };

        private GridNodeSource _corruptedNodeSource;
        private GridNodeSource _nonCorruptedNodeSource;



        public Corrupter() { }



        public async Task InitNodeSourcesForCorruptionAsync()
        {
            var earthNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(EarthNodeSource).Task;
            var seaNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(SeaNodeSource).Task;

            await Task.WhenAll(earthNodeSourceLoadingTask, seaNodeSourceLoadingTask);

            _corruptedNodeSource = earthNodeSourceLoadingTask.Result;
            _nonCorruptedNodeSource = seaNodeSourceLoadingTask.Result;
        }

        /// <summary>
        /// Mark closed with corrupted or grid border zone with corruption source
        /// </summary>
        /// <param name="seedNodePosition">Corruption start node</param>
        /// <param name="checkedPositions">Set of already checked postions for optimization</param>
        /// <returns>Count of corrupted nodes</returns>
        public IEnumerable<GridNode> GetCorruptedZone(Vector2 seedNodePosition, ISet<Vector2> checkedPositions)
        {
            var uncheckedNodes = new Stack<GridNode>();
            var zoneNodes = new HashSet<GridNode>();

            var seedNode = GetNode(seedNodePosition);
            uncheckedNodes.Push(seedNode);

            int corruptedNodeCount = 0;

            while (uncheckedNodes.Count != 0)
            {
                var currentPickedNode = uncheckedNodes.Pop();

                if (checkedPositions.Contains(currentPickedNode.Position) ||
                    currentPickedNode.State == _corruptedNodeSource.State)
                    continue;

                checkedPositions.Add(currentPickedNode.Position);

                if (currentPickedNode.State == _nonCorruptedNodeSource.State)
                {
                    zoneNodes.Add(currentPickedNode);
                    corruptedNodeCount++;
                }

                foreach (var neighbourTemplate in _neighboursTemplates)
                    uncheckedNodes.Push(GetNode(currentPickedNode.Position + neighbourTemplate));
            }

            // If zone contains at least one enemy - zone won't be corrupted
            if (IsZoneFreeOfEnemies(checkedPositions))
            {
                foreach (var node in zoneNodes)
                    CorruptNode(node);
            }
            else
                zoneNodes.Clear();

            return zoneNodes;
        }

        /// <summary>
        /// Immidietry corruptes all nodes in collection
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns>Count of corrupted nodes</returns>
        public void CorruptNodes(IEnumerable<GridNode> nodes)
        {
            foreach (var node in nodes)
                CorruptNode(node);
        }

        /// <summary>
        /// Immidietry decorrupts all nodes in collection
        /// </summary>
        /// <param name="nodes"></param>
        public void DecorruptNodes(IEnumerable<GridNode> nodes)
        {
            foreach (var node in nodes)
                node.SetSource(_nonCorruptedNodeSource);
        }

        private void CorruptNode(GridNode node) => node.SetSource(_corruptedNodeSource);

        private bool IsZoneFreeOfEnemies(ISet<Vector2> zoneNodesPositions)
        {
            var nodesCount = zoneNodesPositions.Count;
            var enemiesPositions = GetEnemiesPositions();

            zoneNodesPositions.ExceptWith(enemiesPositions);

            return zoneNodesPositions.Count == nodesCount; // If not the same count of positions - enemies stay in the zone
        }

        /// <summary>
        /// Gets all positions of enemies, that hypothetically can be in the zone
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Vector2> GetEnemiesPositions()
        {
            foreach (var enemy in XonixGame.SeaEnemies)
                yield return enemy.Position;
        }

        private GridNode GetNode(Vector2 nodePosition)
        {
            if (!XonixGame.TryToGetNodeWithPosition(nodePosition, out GridNode node))
                throw new UnityException($"Node with position {nodePosition} doesn't exist");

            return node;
        }
    }
}
