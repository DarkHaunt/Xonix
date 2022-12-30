using System.Collections.Generic;
using Xonix.Grid;
using UnityEngine;


namespace Xonix.Entities.Enemies
{
    using static GridNodeSource;

    public abstract class EnemyBehaviour : ScriptableObject
    {
        private static readonly Dictionary<Type, NodeState> BorderNodeStates = new Dictionary<Type, NodeState>()
        {
            [Type.Earth] = NodeState.Sea,
            [Type.Sea] = NodeState.Earth,
        };

        private Type _enemyType;



        public abstract void TryToBounceFromNode(Vector2 nodePosition, Vector2 moveDirection, out Vector2 bounceDirection);

        protected abstract Vector2 GetNewMoveDirection(Vector2 walkNodePosition, Vector2 currentDirection);

        public bool IsNodeHasBorderState(GridNode node) => BorderNodeStates[_enemyType] == node.State;

        protected void SetEnemyType(Type enemyType) => _enemyType = enemyType;




        protected virtual void Awake()
        {
            
        }


        public enum Type
        {
            Sea,
            Earth
        }
    } 
}
