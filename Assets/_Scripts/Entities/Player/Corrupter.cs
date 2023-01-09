using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.PlayerComponents
{
    using static XonixGrid;

    /// <summary>
    /// Corrupts zone of nodes without enemies, setting different node source
    /// </summary>
    public class Corrupter
    {
        private const string EarthNodeSource = "Grid/NodeSource/EarthNodeSource";
        private const string SeaNodeSource = "Grid/NodeSource/SeaNodeSource";


        private readonly Vector2[] _neighbourNodesPositionTemplates = new Vector2[4] // 4-direction neighbour node positions templates
        {
            new Vector2(0f, CellSize),
            new Vector2(0f, -CellSize),
            new Vector2(CellSize, 0f),
            new Vector2(-CellSize, 0f)
        };

        private readonly XonixGrid _grid;
        private readonly IEnumerable<Enemy> _seaEnemies;

        private GridNodeSource _corruptedNodeSource;
        private GridNodeSource _nonCorruptedNodeSource;



        public Corrupter(XonixGrid grid, IEnumerable<Enemy> seaEnemies)
        {
            _grid = grid;
            _seaEnemies = seaEnemies; // Earth enemy will never be in zone, so it can be ignored
        }



        public async Task InitAsync()
        {
            var earthNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(EarthNodeSource).Task;
            var seaNodeSourceLoadingTask = Addressables.LoadAssetAsync<GridNodeSource>(SeaNodeSource).Task;

            await Task.WhenAll(earthNodeSourceLoadingTask, seaNodeSourceLoadingTask);

            _corruptedNodeSource = earthNodeSourceLoadingTask.Result;
            _nonCorruptedNodeSource = seaNodeSourceLoadingTask.Result;
        }

        /// <summary>
        /// Corrupts all non-corrupted nodes in enclosed zone with seed node position
        /// </summary>
        /// <param name="seedNodePosition">Corruption start node</param>
        /// <param name="checkedPositions">Set of already checked postions for optimization</param>
        /// <returns>Count of corrupted nodes</returns>
        public IEnumerable<GridNode> GetCorruptedNodes(Vector2 seedNodePosition, ISet<Vector2> checkedPositions)
        {
            // Realization is a simple non-recursive flood fiil algorithm.
            // Non-checked 4-side neighbours get pushed in stack and will be checked for corruption
            // Cycle will repeat until there will be no non-corrupted neighbours

            var uncheckedNodes = new Stack<GridNode>();
            var zoneNodes = new HashSet<GridNode>();

            var seedNode = GetNode(seedNodePosition);
            uncheckedNodes.Push(seedNode);

            while (uncheckedNodes.Count != 0)
            {
                var currentPickedNode = uncheckedNodes.Pop();

                if (checkedPositions.Contains(currentPickedNode.Position) ||
                    currentPickedNode.State == _corruptedNodeSource.State)
                    continue;

                checkedPositions.Add(currentPickedNode.Position);

                if (currentPickedNode.State == _nonCorruptedNodeSource.State)
                    zoneNodes.Add(currentPickedNode);

                foreach (var neighbourTemplate in _neighbourNodesPositionTemplates)
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
            foreach (var enemy in _seaEnemies)
                yield return enemy.Position;
        }

        private GridNode GetNode(Vector2 nodePosition)
        {
            if (!_grid.TryToGetNodeWithPosition(nodePosition, out GridNode node))
                throw new UnityException($"Node with position {nodePosition} doesn't exist");

            return node;
        }
    }
}
