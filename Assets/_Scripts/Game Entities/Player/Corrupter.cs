using System.Collections;
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



        public void CorruptZone(Vector2 seedNodePosition, IEnumerable<Enemy> enemies)
        {
            var nodes = new Stack<GridNode>();
            var checkedPositions = new HashSet<Vector2>();

            var seedNode = GetNode(seedNodePosition);
            nodes.Push(seedNode);
            Action onZoneFreeOfEnemies = null;


            while (nodes.Count != 0)
            {
                var currentPickedNode = nodes.Pop();

                if (checkedPositions.Contains(currentPickedNode.Position) ||
                    currentPickedNode.State == NodeState.Earth)
                    continue;

                checkedPositions.Add(currentPickedNode.Position);

                if (currentPickedNode.State == NodeState.Sea)
                    onZoneFreeOfEnemies += () => CorruptNode(currentPickedNode);

                foreach (var neighbourTemplate in _neighboursTemplates)
                    nodes.Push(GetNode(currentPickedNode.Position + neighbourTemplate));
            }

            var nodesCount = checkedPositions.Count;
            var enemiesPositions = GetEnemiesPositions(enemies);

            checkedPositions.ExceptWith(enemiesPositions);

            if (checkedPositions.Count == nodesCount)
            {
                MonoBehaviour.print("Zone DON'T have an enemy");
                onZoneFreeOfEnemies?.Invoke();
            }
            else
                MonoBehaviour.print("Zone HAVE an enemy");
        }

        public void CorruptNode(GridNode node) => node.SetSource(_corruptedNodeSource);

        private GridNode GetNode(Vector2 vector2)
        {
            if (!XonixGame.TryToGetNodeWithPosition(vector2, out GridNode node))
                throw new UnityException($"Node with position {vector2} doesn't exist");

            return node;
        }

        private IEnumerable<Vector2> GetEnemiesPositions(IEnumerable<Enemy> enemies)
        {
            foreach (var enemy in enemies)
                yield return enemy.Position;
        }
    }
}
