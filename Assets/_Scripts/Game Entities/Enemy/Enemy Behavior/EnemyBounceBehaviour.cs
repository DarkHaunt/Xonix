using System.Collections.Generic;
using Xonix.Grid;
using UnityEngine;



namespace Xonix.Entities.EnemyComponents
{
    using static GridNodeSource;

    /// <summary>
    /// Controlls enemy bouncing from border
    /// </summary>
    public class EnemyBounceBehaviour
    {
        /// <summary>
        /// A node state, that this enemy type perceives as border, and bounce from it
        /// </summary>
        private static readonly Dictionary<EnemyType, NodeState> EnemyTypeBorderNodeState = new Dictionary<EnemyType, NodeState>(2)
        {
            [EnemyType.EarthEnemy] = NodeState.Sea,
            [EnemyType.SeaEnemy] = NodeState.Earth,
        };

        private readonly EnemyType _enemyType;



        public EnemyBounceBehaviour(EnemyType enemyType)
        {
            _enemyType = enemyType;
        }



        public Vector2 GetBounceDirection(Vector2 currentNodePosition, Vector2 currentDirection)
        {
            var firstNeighbourPosition = currentNodePosition + new Vector2(currentDirection.x, 0f);
            var secondNeighbourPosition = currentNodePosition + new Vector2(0f, currentDirection.y);

            var firstNeighbourNodeIsBorder = IsBorderInPosition(firstNeighbourPosition);
            var secondNeighbourNodeIsBorder = IsBorderInPosition(secondNeighbourPosition);

            // If moving IN angle - bounce in opposite direction
            if (firstNeighbourNodeIsBorder && secondNeighbourNodeIsBorder)
                return currentDirection * new Vector2(-1f, -1f);

            // IF moving ON angle - bounce in opposite direction
            if (!firstNeighbourNodeIsBorder && !secondNeighbourNodeIsBorder)
                return currentDirection * new Vector2(-1f, -1f);

            // If second node is free - rotate on 90 degree acconrding the second node
            if (firstNeighbourNodeIsBorder)
                return new Vector2(-currentDirection.x, currentDirection.y);

            // If first node is free - rotate on 90 degree acconrding the first node
            return new Vector2(currentDirection.x, -currentDirection.y);
        }

        public bool IsNodeHasBorderState(GridNode node) => EnemyTypeBorderNodeState[_enemyType] == node.State;

        private bool IsBorderInPosition(Vector2 position)
        {
            var isNodeOutOfGameField = !XonixGame.TryToGetNodeWithPosition(position, out GridNode node);

            if (!isNodeOutOfGameField)
                return IsNodeHasBorderState(node);

            return isNodeOutOfGameField;
        }



        public enum EnemyType
        {
            SeaEnemy,
            EarthEnemy
        }
    }
}
