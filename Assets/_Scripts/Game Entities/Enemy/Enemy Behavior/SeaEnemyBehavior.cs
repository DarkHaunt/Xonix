using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.Enemies
{
    [CreateAssetMenu(fileName = "SeaEnemyBehavior", menuName = "Enemy/SeaEnemyBehavior", order = 52)]
    public class SeaEnemyBehavior : EnemyBehaviour
    {
        public override void TryToBounceFromNode(Vector2 nodePosition, Vector2 moveDirection, out Vector2 bounceDirection)
        {
            bounceDirection = GetNewMoveDirection(nodePosition, moveDirection);
        }

        protected override Vector2 GetNewMoveDirection(Vector2 walkNodePosition, Vector2 currentDirection)
        {
            var standingPosition = walkNodePosition - (currentDirection * StaticData.CellSize);

            var firstNeighbourNodeDirection = new Vector2(currentDirection.x, 0f);
            var secondNeighbourNodeDirection = new Vector2(0f, currentDirection.y);

            var firstNeighbourPosition = standingPosition + firstNeighbourNodeDirection;
            var secondNeighbourPosition = standingPosition + secondNeighbourNodeDirection;

            var firstNeighbourNodeIsBorder = IsPositionNodeBorder(firstNeighbourPosition);
            var secondNeighbourNodeIsBorder = IsPositionNodeBorder(secondNeighbourPosition);


            if (firstNeighbourNodeIsBorder && secondNeighbourNodeIsBorder) // If moving IN angle - bounce in opposite direction
                return currentDirection * new Vector2(-1f, -1f);

            if (!firstNeighbourNodeIsBorder && !secondNeighbourNodeIsBorder) // IF moving ON angle - bounce in opposite direction
                return currentDirection * new Vector2(-1f, -1f);

            if (firstNeighbourNodeIsBorder) // If second node is free - rotate on 90 degree acconrding the second node
                return new Vector2(-currentDirection.x, currentDirection.y);

            // If first node is free - rotate on 90 degree acconrding the first node
            return new Vector2(currentDirection.x, -currentDirection.y);
        }

        private bool IsPositionNodeBorder(Vector2 position)
        {
            var isNodeOutOfGameField = !XonixGame.TryToGetNodeWithPosition(position, out GridNode node);

            if (!isNodeOutOfGameField)
                return IsNodeHasBorderState(node);

            return isNodeOutOfGameField;
        }



        protected override void Awake()
        {
            SetEnemyType(Type.Sea);
        }
    }
}
