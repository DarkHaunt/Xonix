using System.Collections.Generic;
using UnityEngine;
using Xonix.Grid;
using System;



namespace Xonix.Entities.Player
{
    using static GridNodeSource;
    using static StaticData;

    /// <summary>
    /// Marks zone without enemy by the 
    /// </summary>
    public class Corrupter
    {
        private readonly GridNodeSource _corruptedNodeSource;
        private readonly Vector2[] _neighboursTemplates = new Vector2[4] // Convenient node neighbours position
        {
            new Vector2(0f, CellSize),
            new Vector2(0f, -CellSize),
            new Vector2(CellSize, 0f),
            new Vector2(-CellSize, 0f),
        };



        public Corrupter(GridNodeSource corruptedNodeSource)
        {
            _corruptedNodeSource = corruptedNodeSource;
        }



        /// <summary>
        /// Mark closed with corrupted or grid border zone with corruption source
        /// </summary>
        /// <param name="seedNodePosition">Corruption start node</param>
        /// <param name="checkedPositions">Set of already checked postions for optimization</param>
        public void CorruptZone(Vector2 seedNodePosition, ISet<Vector2> checkedPositions)
        {
            var uncheckedNodes = new Stack<GridNode>();

            var seedNode = GetNode(seedNodePosition);
            uncheckedNodes.Push(seedNode);

            Action onZoneFreeOfEnemies = null;

            while (uncheckedNodes.Count != 0)
            {
                var currentPickedNode = uncheckedNodes.Pop();

                if (checkedPositions.Contains(currentPickedNode.Position) ||
                    currentPickedNode.State == NodeState.Earth)
                    continue;

                checkedPositions.Add(currentPickedNode.Position);

                if (currentPickedNode.State == NodeState.Sea)
                    onZoneFreeOfEnemies += () => CorruptNode(currentPickedNode);

                foreach (var neighbourTemplate in _neighboursTemplates)
                    uncheckedNodes.Push(GetNode(currentPickedNode.Position + neighbourTemplate));
            }

            if (IsEnemyInZone(checkedPositions))
                onZoneFreeOfEnemies?.Invoke();
        }

        /// <summary>
        /// Immidietry corruptes all nodes in collection
        /// </summary>
        /// <param name="nodes"></param>
        public void CorruptNodes(IEnumerable<GridNode> nodes)
        {
            foreach (var node in nodes)
                CorruptNode(node);
        }

        private void CorruptNode(GridNode node) => node.SetSource(_corruptedNodeSource);

        private bool IsEnemyInZone(ISet<Vector2> zoneNodesPositions)
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

        private GridNode GetNode(Vector2 vector2)
        {
            if (!XonixGame.TryToGetNodeWithPosition(vector2, out GridNode node))
                throw new UnityException($"Node with position {vector2} doesn't exist");

            return node;
        }
    }
}
