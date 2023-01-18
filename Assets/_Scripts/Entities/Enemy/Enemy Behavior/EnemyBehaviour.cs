using UnityEngine;
using Xonix.Grid;



namespace Xonix.Entities.EnemyComponents
{
    using static GridNodeSource;

    /// <summary>
    /// Controlls enemy bouncing from border and positioning
    /// </summary>
    public abstract class EnemyBehaviour
    {
        private readonly NodeState _walkNodeState;
        private readonly XonixGrid _grid;


        protected XonixGrid Grid => _grid;


        public EnemyBehaviour(NodeState walkNodeState, XonixGrid grid)
        {
            _walkNodeState = walkNodeState;
            _grid = grid;
        }



        /// <summary>
        /// Position of initialization on grid
        /// </summary>
        /// <returns></returns>
        public abstract Vector2 GetInitPosition();

        public Vector2 GetBounceDirection(Vector2 currentNodePosition, Vector2 currentDirection)
        {
            var firstNeighbourPosition = currentNodePosition + new Vector2(currentDirection.x, 0f);
            var secondNeighbourPosition = currentNodePosition + new Vector2(0f, currentDirection.y);

            var firstNeighbourNodeIsBorder = IsBorderInPosition(firstNeighbourPosition);
            var secondNeighbourNodeIsBorder = IsBorderInPosition(secondNeighbourPosition);

            // If moving IN angle - bounce in opposite direction
            if (firstNeighbourNodeIsBorder && secondNeighbourNodeIsBorder)
                return currentDirection * new Vector2(-1f, -1f);

            // If moving ON angle - bounce in opposite direction
            if (!firstNeighbourNodeIsBorder && !secondNeighbourNodeIsBorder)
                return currentDirection * new Vector2(-1f, -1f);

            // If second node is free - rotate on 90 degree acconrding the second node
            if (firstNeighbourNodeIsBorder)
                return new Vector2(-currentDirection.x, currentDirection.y);

            // If first node is free - rotate on 90 degree acconrding the first node
            return new Vector2(currentDirection.x, -currentDirection.y);
        }

        public bool IsNodeHasBorderState(GridNode node) => node.State != _walkNodeState;

        private bool IsBorderInPosition(Vector2 position)
        {
            var isNodeOutOfGameField = !_grid.TryToGetNodeWithPosition(position, out GridNode node);

            if (!isNodeOutOfGameField)
                return IsNodeHasBorderState(node);

            return isNodeOutOfGameField;
        }
    }
}
