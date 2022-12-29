using System.Collections.Generic;
using UnityEngine;
using Xonix.Grid;
using System;



namespace Xonix.Entities.Player
{
    using static GridNodeSource;
    using static StaticData;


    public class Corrupter
    {
        private readonly GridNodeSource _corruptedNodeSource;
        private readonly Vector2[] _neighboursTemplates = new Vector2[4]
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
            {
                MonoBehaviour.print($"Zone with start node pos {seedNodePosition} DON'T have an enemy");
                onZoneFreeOfEnemies?.Invoke();
            }
            else
                MonoBehaviour.print("Zone HAVE an enemy");
        }

        public void CorruptNodes(IEnumerable<GridNode> nodes)
        {
            foreach (var node in nodes)
                CorruptNode(node);
        }

        private bool IsEnemyInZone(ISet<Vector2> zoneNodesPositions)
        {
            var nodesCount = zoneNodesPositions.Count;
            var enemiesPositions = GetEnemiesPositions();

            zoneNodesPositions.ExceptWith(enemiesPositions);

            return zoneNodesPositions.Count == nodesCount; // If not the same count of positions - enemies stay in the zone
        }

        private void CorruptNode(GridNode node) => node.SetSource(_corruptedNodeSource);

        private GridNode GetNode(Vector2 vector2)
        {
            if (!XonixGame.TryToGetNodeWithPosition(vector2, out GridNode node))
                throw new UnityException($"Node with position {vector2} doesn't exist");

            return node;
        }

        private IEnumerable<Vector2> GetEnemiesPositions()
        {
            foreach (var enemy in XonixGame.SeaEnemies)
                yield return enemy.Position;
        }
    }
}
