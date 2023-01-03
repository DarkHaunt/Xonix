using System.Collections.Generic;
using Xonix.Grid;
using UnityEngine;



namespace Xonix.Entities.Enemies
{
    using static GridNodeSource;
    using static StaticData;

    public class EnemyBehaviour
    {
        private static readonly Dictionary<EnemyType, NodeState> BorderNodeType = new Dictionary<EnemyType, NodeState>()
        {
            [EnemyType.EarthEnemy] = NodeState.Sea,
            [EnemyType.SeaEnemy] = NodeState.Earth,
        };

        private readonly EnemyType _enemyType;



        public EnemyBehaviour(EnemyType enemyType)
        {
            _enemyType = enemyType;
        }



        public Vector2 GetMoveTranslation(Vector2 currentPosition, Vector2 currentMoveDirection)
        {
            var bounceNodePosition = currentPosition + (currentMoveDirection * CellSize);

            if (!XonixGame.TryToGetNodeWithPosition(bounceNodePosition, out GridNode nextNode) || IsNodeHasBorderState(nextNode))
                currentMoveDirection = GetBounceDirection(currentPosition, currentMoveDirection);
            else
            {
                if (IsNodeTrailState(nextNode))
                {
                    MonoBehaviour.print("Enemy touched trail");
                    XonixGame.PlayerLoseLevel();
                }
            }


            return currentMoveDirection * CellSize;
        }

        private Vector2 GetBounceDirection(Vector2 currentNodePosition, Vector2 currentDirection)
        {
            var firstNeighbourPosition = currentNodePosition + new Vector2(currentDirection.x, 0f);
            var secondNeighbourPosition = currentNodePosition + new Vector2(0f, currentDirection.y);

            var firstNeighbourNodeIsBorder = IsPositionBounceBorder(firstNeighbourPosition);
            var secondNeighbourNodeIsBorder = IsPositionBounceBorder(secondNeighbourPosition);

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

        private bool IsPositionBounceBorder(Vector2 position)
        {
            var isNodeOutOfGameField = !XonixGame.TryToGetNodeWithPosition(position, out GridNode node);

            if (!isNodeOutOfGameField)
                return IsNodeHasBorderState(node);

            return isNodeOutOfGameField;
        }

        private bool IsNodeHasBorderState(GridNode node) => BorderNodeType[_enemyType] == node.State;

        private bool IsNodeTrailState(GridNode node) => node.State == NodeState.Trail;
  


        public enum EnemyType
        {
            SeaEnemy,
            EarthEnemy
        }
    }
}
